using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using ClosedXML.Excel;
using LastR2D2.Tools.DataDiff.Core.Interfaces;

namespace LastR2D2.Tools.DataDiff.Core
{
    public class ExcelHighlighter : IExcelHighlighter
    {
        private readonly IColumnNameBuilder columnNameBuilder;

        public ExcelHighlighter(IColumnNameBuilder columnNameBuilder)
        {
            this.columnNameBuilder = columnNameBuilder;
        }

        public void Highlight(IXLWorksheet worksheet, HighlightOptions options)
        {
            if (worksheet == null)
                throw new ArgumentNullException("worksheet");
            var headerRow = worksheet.FirstRowUsed();
            var compareRelatedColumns =
                FindColumnsBySuffix(headerRow, options.NameOfLeftDataSource, options.NameOfRightDataSource)
                    .ToList();

            var compareRelatedColumnNames = compareRelatedColumns.Select(l => l.Key);

            // hide gap & compare result column
            foreach (var column in compareRelatedColumns)
            {
                if (columnNameBuilder.IsGapColumn(column.Key) || columnNameBuilder.IsCompareResultColumn(column.Key))
                {
                    worksheet.Column(column.Value.ColumnNumber).Hide();
                }
            }

            // create formula
            var cellValueMissingFormulaFormats = new Dictionary<string, string>();
            var cellValueNotEqualFormulaFormats = new Dictionary<string, string>();
            var cellValueSimilarFormulaFormats = new Dictionary<string, string>();
            var cellValueEqualFormulaFormats = new Dictionary<string, string>();

            var underlyingColumns = columnNameBuilder.GetUnderlyingColumnNames(compareRelatedColumnNames, new[] { options.NameOfLeftDataSource, options.NameOfRightDataSource });

            foreach (var underlyingColumn in underlyingColumns)
            {
                var leftColumnName = columnNameBuilder.BuildColumName(options.NameOfLeftDataSource, underlyingColumn);
                var rightColumnName = columnNameBuilder.BuildColumName(options.NameOfRightDataSource, underlyingColumn);
                var gapColumnName = columnNameBuilder.BuildGapColumnName(underlyingColumn);
                var compareColumnName = columnNameBuilder.BuildCompareResultColumnName(underlyingColumn);

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

            ApplyFormula(contentRows,
                cellValueEqualFormulaFormats, cellValueMissingFormulaFormats, cellValueSimilarFormulaFormats, cellValueNotEqualFormulaFormats);
        }

        private static void CreateFormula(
            KeyValuePair<string, IXLAddress> leftColumn, KeyValuePair<string, IXLAddress> rightColumn, KeyValuePair<string, IXLAddress> gapColumn, KeyValuePair<string, IXLAddress> compareColumn,
            IDictionary<string, string> missingFormulaFormats, IDictionary<string, string> similarFormulaFormats,
            IDictionary<string, string> equalFormulaFormats, IDictionary<string, string> notEqualFormulaFormats)
        {
            var missingFormulaFormat = string.Format(
                CultureInfo.CurrentCulture,
                //OR(ISBLANK($B4),ISBLANK($A4))
                @"OR(
                    ISBLANK(${0}{{0}}),
                    ISBLANK(${1}{{0}})
                )",
                leftColumn.Value.ColumnLetter,
                rightColumn.Value.ColumnLetter);

            missingFormulaFormats.Add(compareColumn.Value.ColumnLetter, missingFormulaFormat);

            var similarFormulaFormat = string.Format(
                CultureInfo.CurrentCulture,
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
                CultureInfo.CurrentCulture,
                //=AND(NOT($F2),$A2=$B2)
                @"AND(
                    NOT({0}),
                    ${1}{{0}}=${2}{{0}}
                )",
                missingFormulaFormat,
                leftColumn.Value.ColumnLetter,
                rightColumn.Value.ColumnLetter);

            var notEqualFormulaFormat = string.Format(
                CultureInfo.CurrentCulture,
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


        private static void ApplyFormula(IList<IXLRow> contentRows,
            IDictionary<string, string> cellValueEqualFormulaFormats, IDictionary<string, string> cellValueMissingFormulaFormats,
            IDictionary<string, string> cellValueSimilarFormulaFormats, IDictionary<string, string> cellValueNotEqualFormulaFormats)
        {
            // ReSharper disable once PossibleNullReferenceException
            var worksheet = contentRows.FirstOrDefault().Worksheet;

            var compareColumnLetters = cellValueEqualFormulaFormats.Keys.ToList();

            foreach (var row in contentRows)
            {
                var rowNumber = row.RowNumber();

                ApplyFormulaToCompareResultColumns(cellValueEqualFormulaFormats, cellValueMissingFormulaFormats, cellValueSimilarFormulaFormats,
                    worksheet, compareColumnLetters, rowNumber);
            }

            var rowValueSimilarFormulaList = new List<string>();
            compareColumnLetters.ForEach(columnLetter =>
            {
                cellValueMissingFormulaFormats[columnLetter] = string.Format(CultureInfo.CurrentCulture, "${0}{{0}}=\"MISSING\"", columnLetter);
                cellValueEqualFormulaFormats[columnLetter] = string.Format(CultureInfo.CurrentCulture, "${0}{{0}}=\"EQUAL\"", columnLetter);
                cellValueSimilarFormulaFormats[columnLetter] = string.Format(CultureInfo.CurrentCulture, "${0}{{0}}=\"SIMILAR\"", columnLetter);
                cellValueNotEqualFormulaFormats[columnLetter] = string.Format(CultureInfo.CurrentCulture, "${0}{{0}}=\"DIFFERENT\"", columnLetter);

                rowValueSimilarFormulaList.Add(string.Format(CultureInfo.CurrentCulture, "OR({0},{1})", cellValueEqualFormulaFormats[columnLetter], cellValueSimilarFormulaFormats[columnLetter]));
            });

            var formattingRange = worksheet.Range(worksheet.FirstCellUsed().Address, worksheet.LastCellUsed().Address);

            var equalFormulaFormat = string.Format(CultureInfo.CurrentCulture, "AND({0})"
                , string.Join(",", cellValueEqualFormulaFormats.Values)); // all cells equals to its pair

            var missingFormulaFormat = string.Format(CultureInfo.CurrentCulture, "AND({0})"
                , string.Join(",", cellValueMissingFormulaFormats.Values)); // all cells are missing

            // all equal or similar (at least one similar)
            var similarFormulaFormat = string.Format(CultureInfo.CurrentCulture, "AND(AND({0}), OR({1}))"
                , string.Join(",", rowValueSimilarFormulaList) // all cells are either equal or similar to its pair
                , string.Join(",", cellValueSimilarFormulaFormats.Values)); // at least one cell is similar to its pair

            // not equal means
            var notEqualFormulaFormat = string.Format(CultureInfo.CurrentCulture, "OR({0},{1})" // either
                , string.Join(",", cellValueNotEqualFormulaFormats.Values) // any cell is different with its pair or
                , string.Format(CultureInfo.CurrentCulture, "AND(OR({0}),NOT(AND({0})))", string.Join(",", cellValueMissingFormulaFormats.Values)) // at least one cell (not all) is missing
                );

            ApplyConditionalFormatting(worksheet, formattingRange, equalFormulaFormat, missingFormulaFormat, similarFormulaFormat, notEqualFormulaFormat);
        }

        private static void ApplyFormulaToCompareResultColumns(IDictionary<string, string> equalFormulaFormat, IDictionary<string, string> missingFormulaFormat
            , IDictionary<string, string> similarFormulaFormat, IXLWorksheet worksheet, IEnumerable<string> compareColumnLetters, int rowNumber)
        {
            foreach (var compareColumnLetter in compareColumnLetters)
            {
                var compareColum = worksheet.Cell(rowNumber, compareColumnLetter);
                var formulaFormat = string.Format(CultureInfo.CurrentCulture, "IF({0},\"MISSING\",IF({1},\"EQUAL\",IF({2},\"SIMILAR\",\"DIFFERENT\")))",
                    missingFormulaFormat[compareColumnLetter],
                    equalFormulaFormat[compareColumnLetter],
                    similarFormulaFormat[compareColumnLetter]);
                var formula = PrepareFormula(string.Format(CultureInfo.CurrentCulture, formulaFormat, rowNumber));
                compareColum.SetFormulaA1(formula);
            }
        }

        private static string PrepareFormula(string formula)
        {
            formula = formula.TrimWhiteSpaces();
            return formula;
        }

        private static void ApplyConditionalFormatting(IXLWorksheet worksheet, IXLRange formattingRange, string equalFormulaFormat, string missingFormulaFormat, string similarFormulaFormat, string notEqualFormulaFormat)
        {
            var top = worksheet.FirstRowUsed().RowNumber();
            formattingRange.AddConditionalFormat().WhenIsTrue(
                    PrepareFormula(string.Format(CultureInfo.CurrentCulture, equalFormulaFormat, top)))
                    .Fill.SetBackgroundColor(XLColor.Green);

            formattingRange.AddConditionalFormat().WhenIsTrue(
                PrepareFormula(string.Format(CultureInfo.CurrentCulture, missingFormulaFormat, top)))
                .Fill.SetBackgroundColor(XLColor.Red);

            formattingRange.AddConditionalFormat().WhenIsTrue(
                PrepareFormula(string.Format(CultureInfo.CurrentCulture, similarFormulaFormat, top)))
                .Fill.SetBackgroundColor(XLColor.GreenRyb);

            formattingRange.AddConditionalFormat().WhenIsTrue(
                PrepareFormula(string.Format(CultureInfo.CurrentCulture, notEqualFormulaFormat, top)))
                .Fill.SetBackgroundColor(XLColor.Yellow);
        }

        private IEnumerable<KeyValuePair<string, IXLAddress>> FindColumnsBySuffix(IXLRow row, string nameOfLeftDataSource, string nameOfRightDataSource)
        {
            var cells = row.CellsUsed();
            var result = new Dictionary<string, IXLAddress>();
            cells.ForEach(cell =>
            {
                var columnName = cell.Value.ToString();
                if (columnNameBuilder.IsGeneratedColumn(columnName, new[] { nameOfLeftDataSource, nameOfRightDataSource }))
                {
                    result.Add(columnName, cell.Address);
                }
            });
            return result;
        }
    }
}
