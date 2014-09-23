using System.Data;
using System.Diagnostics.Contracts;
using System.IO;
using ClosedXML.Excel;

namespace lastr2d2.Tools.DataDiff.Core
{
    public class ExcelGenerator : IReportGenerator
    {
        public void Export(DataTable dataTable, string path, string sheetName = "")
        {
            Contract.Requires(dataTable != null);
            Contract.Requires(path != null);
            Contract.Requires(File.Exists(path), string.Format("path {0} doesn't exist", path));
            Contract.Requires(!string.IsNullOrWhiteSpace(dataTable.TableName) || !string.IsNullOrWhiteSpace(sheetName));

            var workbook = new XLWorkbook();
            if (File.Exists(path))
            {
                workbook = new XLWorkbook(path);
            }

            if (string.IsNullOrEmpty(sheetName))
            {
                workbook.Worksheets.Add(dataTable);
            }
            else
            {
                workbook.Worksheets.Add(dataTable, sheetName);
            }

            workbook.SaveAs(path);
        }
    }
}
