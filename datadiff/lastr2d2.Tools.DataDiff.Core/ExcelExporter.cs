using System;
using System.Data;
using System.IO;
using System.Linq;
using ClosedXML.Excel;
using LastR2D2.Tools.DataDiff.Core.Interfaces;

namespace LastR2D2.Tools.DataDiff.Core
{
    public class ExcelExporter : IDataExporter<XLWorkbook>
    {
        public XLWorkbook Export(DataTable dataTable, ExportOptions options, IExcelHighlighter highLighter)
        {
            if (dataTable == null)
                throw new ArgumentNullException("dataTable");
            var excelExportOptions = options as ExcelExportOptions;
            if (excelExportOptions == null)
                throw new ArgumentException("ExcelExportOptions only", "options");

            var sheetName = excelExportOptions.SheetName ?? dataTable.TableName;
            var workbook = string.IsNullOrEmpty(excelExportOptions.Path) || !File.Exists(excelExportOptions.Path) ?
                    new XLWorkbook() : new XLWorkbook(excelExportOptions.Path);

            if (string.IsNullOrEmpty(sheetName))
            {
                workbook.Worksheets.Add(dataTable);
            }
            else
            {
                workbook.Worksheets.Add(dataTable, sheetName);
            }

            if (highLighter != null)
            {
                var worksheet = workbook.Worksheet(sheetName);
                highLighter.Highlight(worksheet, excelExportOptions.HighlightOptions);
            }

            OrderWorksheets(workbook);
            workbook.SaveAs(excelExportOptions.Path);

            return workbook;
        }



        private static void OrderWorksheets(XLWorkbook workbook)
        {
            var worksheets = workbook.Worksheets.OrderBy(sheet => sheet.Name, StringComparer.OrdinalIgnoreCase).ToArray();
            for (int index = 0; index < worksheets.Length; index++)
            {
                workbook.Worksheet(worksheets[index].Name).Position = index + 1;
            }
        }
    }
}
