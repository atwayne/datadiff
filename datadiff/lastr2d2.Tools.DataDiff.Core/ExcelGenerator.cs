using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System;
using System.IO;
using System.Linq;
using ClosedXML.Excel;
using DocumentFormat.OpenXml.Spreadsheet;
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

        public void Highlight(IXLWorksheet worksheet, string leftSuffix, string rightSuffix, string gapSuffix = "_Gap")
        {
            var headerRow = worksheet.FirstRowUsed();
            var sourceColumns = new[] { leftSuffix, rightSuffix, gapSuffix }.SelectMany(suffix =>
                FindColumnsBySuffix(headerRow, suffix)
            ).ToList();

            var columnNames = sourceColumns.Select(l => l.Key);
            var underlyingColumns = FindUnderlyingColumns(columnNames, leftSuffix, rightSuffix, gapSuffix);

            // hide gap column
            foreach (var column in sourceColumns)
            {
                if (column.Key.EndsWith(gapSuffix, StringComparison.OrdinalIgnoreCase))
                {
                    worksheet.Column(column.Value.ColumnNumber).Hide();
                }
            }

            // create formula
            var missingFormulaFormats = new List<string>();
            var notEqualFormulaFormats = new List<string>();
            var similarFormulaFormats = new List<string>();
            var equalFormulaFormats = new List<string>();

            foreach (var column in underlyingColumns)
            {
                var leftName = string.Format("{0}{1}", column, leftSuffix);
                var rightName = string.Format("{0}{1}", column, rightSuffix);
                var gapName = string.Format("{0}{1}", column, gapSuffix);

                var leftColumn =
                    sourceColumns.FirstOrDefault(l => leftName.Equals(l.Key, StringComparison.OrdinalIgnoreCase));
                var rightColumn =
                    sourceColumns.FirstOrDefault(l => rightName.Equals(l.Key, StringComparison.OrdinalIgnoreCase));
                var gapColumn =
                    sourceColumns.FirstOrDefault(l => gapName.Equals(l.Key, StringComparison.OrdinalIgnoreCase));

                CreateFormula(leftColumn, rightColumn, gapColumn, missingFormulaFormats, similarFormulaFormats, equalFormulaFormats, notEqualFormulaFormats);
            }

            var contentRows = worksheet.RowsUsed().Skip(1).ToList();
            var equalFormulaFormat = string.Format("AND({0})", string.Join(",", equalFormulaFormats));
            var missingFormulaFormat = string.Format("AND({0})", string.Join(",", missingFormulaFormats));
            var notEqualFormulaFormat = string.Format("OR({0})", string.Join(",", notEqualFormulaFormats));
            var similarFormulaFormat = string.Format("AND(NOT({0}),OR({1}))", notEqualFormulaFormat,
                string.Join(",", similarFormulaFormats));


            ApplyFormula(contentRows, equalFormulaFormat, missingFormulaFormat, similarFormulaFormat, notEqualFormulaFormat);

            // adjust width to contents
            worksheet.ColumnsUsed().ForEach(column => column.AdjustToContents());

        }

        private static void ApplyFormula(IList<IXLRow> contentRows, string equalFormulaFormat, string missingFormulaFormat,
            string similarFormulaFormat, string notEqualFormulaFormat)
        {
            var worksheet = contentRows.FirstOrDefault().Worksheet;
            var left = worksheet.FirstColumnUsed().ColumnNumber();
            var right = worksheet.LastColumnUsed().ColumnNumber();

            foreach (var row in contentRows)
            {
                var range = worksheet.Range(row.RowNumber(), left, row.RowNumber(), right);

                range.AddConditionalFormat().WhenIsTrue(
                    string.Format(equalFormulaFormat, row.RowNumber()).TrimWhiteSpaces())
                    .Fill.SetBackgroundColor(XLColor.Green);

                range.AddConditionalFormat().WhenIsTrue(
                    string.Format(missingFormulaFormat, row.RowNumber()).TrimWhiteSpaces())
                    .Fill.SetBackgroundColor(XLColor.Red);

                range.AddConditionalFormat().WhenIsTrue(
                    string.Format(similarFormulaFormat, row.RowNumber()).TrimWhiteSpaces())
                    .Fill.SetBackgroundColor(XLColor.GreenRyb);

                range.AddConditionalFormat().WhenIsTrue(string.Format(
                    notEqualFormulaFormat, row.RowNumber()).TrimWhiteSpaces())
                    .Fill.SetBackgroundColor(XLColor.Yellow);
            }
        }

        private static void CreateFormula(KeyValuePair<string, IXLAddress> leftColumn, KeyValuePair<string, IXLAddress> rightColumn, KeyValuePair<string, IXLAddress> gapColumn,
            List<string> missingFormulaFormats, List<string> similarFormulaFormats, List<string> equalFormulaFormats, List<string> notEqualFormulaFormats)
        {
            var missingFormulaFormat = string.Format(
                //OR(ISBLANK($B4),ISBLANK($A4))
                @"OR(
                    ISBLANK(${0}{{0}}),
                    ISBLANK(${1}{{0}})
                )",
                leftColumn.Value.ColumnLetter,
                rightColumn.Value.ColumnLetter);
            missingFormulaFormats.Add(missingFormulaFormat);

            var similarFormulaFormat = string.Format(
                //AND(NOT($F2),$C2<>0,$A2<>$B2,ABS($A2-$B2)<$C2)
                @"AND(
                    NOT({0}),
                    ${3}{{0}}<>0,
                    ${1}{{0}}<>${2}{{0}},
                    ABS(${1}{{0}}-${2}{{0}})<${3}{{0}}
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

            similarFormulaFormats.Add(similarFormulaFormat);
            equalFormulaFormats.Add(equalFormulaFormat);
            notEqualFormulaFormats.Add(notEqualFormulaFormat);
        }

        private static IEnumerable<string> FindUnderlyingColumns(IEnumerable<string> columns, string leftSuffix, string rightSuffix, string gapSuffix)
        {
            var suffixes = new[] { leftSuffix, rightSuffix, gapSuffix };
            return columns.Select(l => TrimEnd(l, suffixes))
                .Distinct()
                .ToList();
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