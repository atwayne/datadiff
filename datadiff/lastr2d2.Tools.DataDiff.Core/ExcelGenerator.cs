using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.IO;
using System.Linq;
using ClosedXML.Excel;
using DataTable = System.Data.DataTable;

namespace lastr2d2.Tools.DataDiff.Core
{
    public class ExcelGenerator : IReportGenerator
    {
        public XLWorkbook Export(DataTable dataTable, string sheetName = null, string path = null)
        {
            Contract.Requires(dataTable != null);
            Contract.Requires(!string.IsNullOrWhiteSpace(dataTable.TableName) || !string.IsNullOrWhiteSpace(sheetName));
            sheetName = sheetName ?? dataTable.TableName;
            XLWorkbook workbook;

            workbook = string.IsNullOrEmpty(path) || !File.Exists(path) ?
                    new XLWorkbook() : new XLWorkbook(path);

            if (string.IsNullOrEmpty(sheetName))
            {
                workbook.Worksheets.Add(dataTable);
            }
            else
            {
                workbook.Worksheets.Add(dataTable, sheetName);
            }
            return workbook;
        }

        public void Highlight(IXLWorksheet worksheet, string leftSourceSuffix, string rightSourceSuffix, string gapColumnSuffix = "_Gap", string compareColumnSuffix = "_Compare")
        {
            var headerRow = worksheet.FirstRowUsed();
            var compareRelatedColumns = new[] { leftSourceSuffix, rightSourceSuffix, gapColumnSuffix, compareColumnSuffix }.SelectMany(suffix =>
                FindColumnsBySuffix(headerRow, suffix)
            ).ToList();

            var compareRelatedColumnNames = compareRelatedColumns.Select(l => l.Key);

            // hide gap & compare column
            foreach (var column in compareRelatedColumns)
            {
                if (column.Key.EndsWith(gapColumnSuffix, StringComparison.OrdinalIgnoreCase)
                    || column.Key.EndsWith(compareColumnSuffix, StringComparison.OrdinalIgnoreCase))
                {
                    worksheet.Column(column.Value.ColumnNumber).Hide();
                }
            }

            // create formula
            var cellValueMissingFormulaFormats = new Dictionary<string, string>();
            var cellValueNotEqualFormulaFormats = new Dictionary<string, string>();
            var cellValueSimilarFormulaFormats = new Dictionary<string, string>();
            var cellValueEqualFormulaFormats = new Dictionary<string, string>();

            var underlyingColumns = FindUnderlyingColumns(compareRelatedColumnNames, leftSourceSuffix, rightSourceSuffix, gapColumnSuffix, compareColumnSuffix);

            foreach (var underlyingColumn in underlyingColumns)
            {
                var leftColumnName = string.Format("{0}{1}", underlyingColumn, leftSourceSuffix);
                var rightColumnName = string.Format("{0}{1}", underlyingColumn, rightSourceSuffix);
                var gapColumnName = string.Format("{0}{1}", underlyingColumn, gapColumnSuffix);
                var compareColumnName = string.Format("{0}{1}", underlyingColumn, compareColumnSuffix);

                var leftColumn =
                    compareRelatedColumns.FirstOrDefault(l => leftColumnName.Equals(l.Key, StringComparison.OrdinalIgnoreCase));
                var rightColumn =
                    compareRelatedColumns.FirstOrDefault(l => rightColumnName.Equals(l.Key, StringComparison.OrdinalIgnoreCase));
                var gapColumn =
                    compareRelatedColumns.FirstOrDefault(l => gapColumnName.Equals(l.Key, StringComparison.OrdinalIgnoreCase));
                var compareColumn =
                    compareRelatedColumns.FirstOrDefault(l => compareColumnName.Equals(l.Key, StringComparison.OrdinalIgnoreCase));

                CreateFormula(leftColumn, rightColumn, gapColumn, compareColumn, cellValueMissingFormulaFormats, cellValueSimilarFormulaFormats, cellValueEqualFormulaFormats, cellValueNotEqualFormulaFormats);
            }

            var contentRows = worksheet.RowsUsed().Skip(1).ToList();

            ApplyFormula(contentRows, compareColumnSuffix,
                cellValueEqualFormulaFormats, cellValueMissingFormulaFormats, cellValueSimilarFormulaFormats, cellValueNotEqualFormulaFormats);
        }

        private static void ApplyFormula(IList<IXLRow> contentRows, string compareSuffix,
            Dictionary<string, string> cellValueEqualFormulaFormats, Dictionary<string, string> cellValueMissingFormulaFormats,
            Dictionary<string, string> cellValueSimilarFormulaFormats, Dictionary<string, string> cellValueNotEqualFormulaFormats)
        {
            // ReSharper disable once PossibleNullReferenceException
            var worksheet = contentRows.FirstOrDefault().Worksheet;

            var compareColumnLetters = cellValueEqualFormulaFormats.Keys.ToList();

            foreach (var row in contentRows)
            {
                var rowNumber = row.RowNumber();

                ApplyFormulaToCompareResultColumns(cellValueEqualFormulaFormats, cellValueMissingFormulaFormats, cellValueSimilarFormulaFormats, cellValueNotEqualFormulaFormats,
                    worksheet, compareColumnLetters, rowNumber);
            }

            var rowValueSimilarFormulaList = new List<string>();
            compareColumnLetters.ForEach(columnLetter =>
            {
                cellValueMissingFormulaFormats[columnLetter] = string.Format("${0}{{0}}=\"MISSING\"", columnLetter);
                cellValueEqualFormulaFormats[columnLetter] = string.Format("${0}{{0}}=\"EQUAL\"", columnLetter);
                cellValueSimilarFormulaFormats[columnLetter] = string.Format("${0}{{0}}=\"SIMILAR\"", columnLetter);
                cellValueNotEqualFormulaFormats[columnLetter] = string.Format("${0}{{0}}=\"DIFFERENT\"", columnLetter);

                rowValueSimilarFormulaList.Add(string.Format("OR({0},{1})", cellValueEqualFormulaFormats[columnLetter], cellValueSimilarFormulaFormats[columnLetter]));
            });

            var formattingRange = worksheet.Range(worksheet.FirstCellUsed().Address, worksheet.LastCellUsed().Address);

            var equalFormulaFormat = string.Format("AND({0})"
                , string.Join(",", cellValueEqualFormulaFormats.Values)); // all cells equals to its pair

            var missingFormulaFormat = string.Format("AND({0})"
                , string.Join(",", cellValueMissingFormulaFormats.Values)); // all cells are missing

            // all equal or similar (at least one similar)
            var similarFormulaFormat = string.Format("AND(AND({0}), OR({1}))"
                , string.Join(",", rowValueSimilarFormulaList) // all cells are either equal or similar to its pair
                , string.Join(",", cellValueSimilarFormulaFormats.Values)); // at least one cell is similar to its pair

            // not equal means
            var notEqualFormulaFormat = string.Format("OR({0})", string.Join(",", cellValueNotEqualFormulaFormats.Values));
            notEqualFormulaFormat = string.Format("OR({0},{1})" // either
                , string.Join(",", cellValueNotEqualFormulaFormats.Values) // any cell is different with its pair or
                , string.Format("AND(OR({0}),NOT(AND({0})))", string.Join(",", cellValueMissingFormulaFormats.Values)) // at least one cell (not all) is missing
                );

            var top = worksheet.FirstRowUsed().RowNumber();
            formattingRange.AddConditionalFormat().WhenIsTrue(
                    PrepareFormula(string.Format(equalFormulaFormat, top)))
                    .Fill.SetBackgroundColor(XLColor.Green);

            formattingRange.AddConditionalFormat().WhenIsTrue(
                PrepareFormula(string.Format(missingFormulaFormat, top)))
                .Fill.SetBackgroundColor(XLColor.Red);

            formattingRange.AddConditionalFormat().WhenIsTrue(
                PrepareFormula(string.Format(similarFormulaFormat, top)))
                .Fill.SetBackgroundColor(XLColor.GreenRyb);

            formattingRange.AddConditionalFormat().WhenIsTrue(
                PrepareFormula(string.Format(notEqualFormulaFormat, top)))
                .Fill.SetBackgroundColor(XLColor.Yellow);
        }

        private static void ApplyFormulaToCompareResultColumns(Dictionary<string, string> equalFormulaFormat, Dictionary<string, string> missingFormulaFormat, Dictionary<string, string> similarFormulaFormat, Dictionary<string, string> notEqualFormulaFormat, IXLWorksheet worksheet, List<string> compareColumnLetters, int rowNumber)
        {
            foreach (var compareColumnLetter in compareColumnLetters)
            {
                var compareColum = worksheet.Cell(rowNumber, compareColumnLetter);
                var formulaFormat = string.Format("IF({0},\"MISSING\",IF({1},\"EQUAL\",IF({2},\"SIMILAR\",\"DIFFERENT\")))",
                    missingFormulaFormat[compareColumnLetter],
                    equalFormulaFormat[compareColumnLetter],
                    similarFormulaFormat[compareColumnLetter],
                    notEqualFormulaFormat[compareColumnLetter]);
                var formula = PrepareFormula(string.Format(formulaFormat, rowNumber));
                compareColum.SetFormulaA1(formula);
            }
        }

        private static string PrepareFormula(string formula)
        {
            formula = formula.TrimWhiteSpaces();
            return formula;
        }

        private static void CreateFormula(
            KeyValuePair<string, IXLAddress> leftColumn, KeyValuePair<string, IXLAddress> rightColumn, KeyValuePair<string, IXLAddress> gapColumn, KeyValuePair<string, IXLAddress> compareColumn,
            Dictionary<string, string> missingFormulaFormats, Dictionary<string, string> similarFormulaFormats,
            Dictionary<string, string> equalFormulaFormats, Dictionary<string, string> notEqualFormulaFormats)
        {
            var missingFormulaFormat = string.Format(
                //OR(ISBLANK($B4),ISBLANK($A4))
                @"OR(
                    ISBLANK(${0}{{0}}),
                    ISBLANK(${1}{{0}})
                )",
                leftColumn.Value.ColumnLetter,
                rightColumn.Value.ColumnLetter);

            missingFormulaFormats.Add(compareColumn.Value.ColumnLetter, missingFormulaFormat);

            var similarFormulaFormat = string.Format(
                //AND(NOT($F2),$C2<>0,$A2<>$B2,ABS($A2-$B2)<$C2)
                @"AND(
                    NOT({0}),
                    IF(
                        AND(
                            ISNUMBER(${1}{{0}}),
                            ISNUMBER(${2}{{0}})
                        ),
                        AND(
                            ${3}{{0}}<>0,
                            ${1}{{0}}<>${2}{{0}},
                            IF(
                                ${1}{{0}}=0,
                                FALSE,
                                ABS(${1}{{0}}-${2}{{0}})/ABS(${1}{{0}})<${3}{{0}}
                            )
                        ),
                        FALSE
                    )
                )",
                missingFormulaFormat,
                leftColumn.Value.ColumnLetter,
                rightColumn.Value.ColumnLetter,
                gapColumn.Value.ColumnLetter);

            var equalFormulaFormat = string.Format(
                //=AND(NOT($F2),$A2=$B2)
                @"AND(
                    NOT({0}),
                    ${1}{{0}}=${2}{{0}}
                )",
                missingFormulaFormat,
                leftColumn.Value.ColumnLetter,
                rightColumn.Value.ColumnLetter);

            var notEqualFormulaFormat = string.Format(
                @"AND(
                    NOT({0}),
                    NOT({1}),
                    NOT({2})
                )",
                missingFormulaFormat,
                similarFormulaFormat,
                equalFormulaFormat);

            similarFormulaFormats.Add(compareColumn.Value.ColumnLetter, similarFormulaFormat);
            equalFormulaFormats.Add(compareColumn.Value.ColumnLetter, equalFormulaFormat);
            notEqualFormulaFormats.Add(compareColumn.Value.ColumnLetter, notEqualFormulaFormat);
        }

        private static IEnumerable<string> FindUnderlyingColumns(IEnumerable<string> columns, string leftSuffix, string rightSuffix, string gapSuffix, string compareSuffix)
        {
            var suffixes = new[] { leftSuffix, rightSuffix, gapSuffix, compareSuffix };
            var names = columns.Select(l => TrimEnd(l, suffixes))
                .Distinct()
                // filter columns we don't want to compare
                .Where(name =>
                    suffixes.All(suffix =>
                        columns.Contains(string.Format("{0}{1}", name, suffix), StringComparer.OrdinalIgnoreCase)));
            return names.ToList();
        }

        private static string TrimEnd(string source, IEnumerable<string> suffixes)
        {
            foreach (var suffix in suffixes)
            {
                if (source.EndsWith(suffix, StringComparison.InvariantCultureIgnoreCase))
                {
                    return source.Substring(0, source.Length - suffix.Length);
                }
            }
            return source;
        }

        private IEnumerable<KeyValuePair<string, IXLAddress>> FindColumnsBySuffix(IXLRow row, string suffix)
        {
            var cells = row.CellsUsed();
            var result = new Dictionary<string, IXLAddress>();
            cells.ForEach(cell =>
            {
                var text = cell.Value.ToString();
                if (text.EndsWith(suffix, StringComparison.InvariantCultureIgnoreCase))
                    result.Add(text, cell.Address);
            });
            return result;
        }
    }
}