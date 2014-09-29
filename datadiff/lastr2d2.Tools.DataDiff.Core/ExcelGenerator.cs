using ClosedXML.Excel;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.IO;
using System.Linq;
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
            XLWorkbook workbook = string.IsNullOrEmpty(path) || !File.Exists(path) ?
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

        public void Highlight(IXLWorksheet worksheet, string leftSuffix, string rightSuffix, string gapSuffix = "_Gap", string compareSuffix = "_Compare")
        {
            var headerRow = worksheet.FirstRowUsed();
            var sourceColumns = new[] { leftSuffix, rightSuffix, gapSuffix, compareSuffix }.SelectMany(suffix =>
                FindColumnsBySuffix(headerRow, suffix)
            ).ToList();

            var columnNames = sourceColumns.Select(l => l.Key);

            // hide gap & compare column
            foreach (var column in sourceColumns)
            {
                if (column.Key.EndsWith(gapSuffix, StringComparison.OrdinalIgnoreCase)
                    || column.Key.EndsWith(compareSuffix, StringComparison.OrdinalIgnoreCase))
                {
                    worksheet.Column(column.Value.ColumnNumber).Hide();
                }
            }

            // create formula
            var missingFormulaFormats = new Dictionary<string, string>();
            var notEqualFormulaFormats = new Dictionary<string, string>();
            var similarFormulaFormats = new Dictionary<string, string>();
            var equalFormulaFormats = new Dictionary<string, string>();

            var underlyingColumns = FindUnderlyingColumns(columnNames, leftSuffix, rightSuffix, gapSuffix, compareSuffix);

            foreach (var underlyingColumn in underlyingColumns)
            {
                var leftName = string.Format("{0}{1}", underlyingColumn, leftSuffix);
                var rightName = string.Format("{0}{1}", underlyingColumn, rightSuffix);
                var gapName = string.Format("{0}{1}", underlyingColumn, gapSuffix);
                var compareName = string.Format("{0}{1}", underlyingColumn, compareSuffix);

                var leftColumn =
                    sourceColumns.FirstOrDefault(l => leftName.Equals(l.Key, StringComparison.OrdinalIgnoreCase));
                var rightColumn =
                    sourceColumns.FirstOrDefault(l => rightName.Equals(l.Key, StringComparison.OrdinalIgnoreCase));
                var gapColumn =
                    sourceColumns.FirstOrDefault(l => gapName.Equals(l.Key, StringComparison.OrdinalIgnoreCase));
                var compareColumn =
                    sourceColumns.FirstOrDefault(l => compareName.Equals(l.Key, StringComparison.OrdinalIgnoreCase));

                CreateFormula(leftColumn, rightColumn, gapColumn, compareColumn, missingFormulaFormats, similarFormulaFormats, equalFormulaFormats, notEqualFormulaFormats);
            }

            var contentRows = worksheet.RowsUsed().Skip(1).ToList();

            ApplyFormula(contentRows, compareSuffix,
                equalFormulaFormats, missingFormulaFormats, similarFormulaFormats, notEqualFormulaFormats);

        }

        private static void ApplyFormula(IList<IXLRow> contentRows, string compareSuffix,
            Dictionary<string, string> equalFormulaFormats, Dictionary<string, string> missingFormulaFormats,
            Dictionary<string, string> similarFormulaFormats, Dictionary<string, string> notEqualFormulaFormats)
        {
            // ReSharper disable once PossibleNullReferenceException
            var worksheet = contentRows.FirstOrDefault().Worksheet;
            var left = worksheet.FirstColumnUsed().ColumnNumber();
            var right = worksheet.LastColumnUsed().ColumnNumber();

            var compareColumnLetters = equalFormulaFormats.Keys.ToList();

            foreach (var row in contentRows)
            {
                var rowNumber = row.RowNumber();

                ApplyFormulaWithCompareColumn(equalFormulaFormats, missingFormulaFormats, similarFormulaFormats, notEqualFormulaFormats,
                    worksheet, compareColumnLetters, rowNumber);
            }

            compareColumnLetters.ForEach(columnLetter =>
            {
                missingFormulaFormats[columnLetter] = string.Format("${0}{{0}}=\"MISSING\"", columnLetter);
                equalFormulaFormats[columnLetter] = string.Format("${0}{{0}}=\"EQUAL\"", columnLetter);
                similarFormulaFormats[columnLetter] = string.Format("${0}{{0}}=\"SIMILAR\"", columnLetter);
                notEqualFormulaFormats[columnLetter] = string.Format("${0}{{0}}=\"DIFFERENT\"", columnLetter);
            });

            foreach (var row in contentRows)
            {
                var rowNumber = row.RowNumber();

                var equalFormulaFormat = string.Format("AND({0})", string.Join(",", equalFormulaFormats.Values));
                var missingFormulaFormat = string.Format("AND({0})", string.Join(",", missingFormulaFormats.Values));
                var notEqualFormulaFormat = string.Format("OR({0})", string.Join(",", notEqualFormulaFormats.Values));
                var similarFormulaFormat = string.Format("AND(NOT({0}),OR({1}))", notEqualFormulaFormat,
                    string.Join(",", similarFormulaFormats.Values));

                var range = worksheet.Range(row.RowNumber(), left, row.RowNumber(), right);

                range.AddConditionalFormat().WhenIsTrue(
                    PrepareFormula(string.Format(equalFormulaFormat, rowNumber)))
                    .Fill.SetBackgroundColor(XLColor.Green);

                range.AddConditionalFormat().WhenIsTrue(
                    PrepareFormula(string.Format(missingFormulaFormat, rowNumber)))
                    .Fill.SetBackgroundColor(XLColor.Red);

                range.AddConditionalFormat().WhenIsTrue(
                    PrepareFormula(string.Format(similarFormulaFormat, rowNumber)))
                    .Fill.SetBackgroundColor(XLColor.GreenRyb);

                range.AddConditionalFormat().WhenIsTrue(
                    PrepareFormula(string.Format(notEqualFormulaFormat, rowNumber)))
                    .Fill.SetBackgroundColor(XLColor.Yellow);
            }
        }

        private static void ApplyFormulaWithCompareColumn(Dictionary<string, string> equalFormulaFormat, Dictionary<string, string> missingFormulaFormat, Dictionary<string, string> similarFormulaFormat, Dictionary<string, string> notEqualFormulaFormat, IXLWorksheet worksheet, List<string> compareColumnLetters, int rowNumber)
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
                                ABS(${1}{{0}}-${2}{{0}})/${1}{{0}}<${3}{{0}}
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