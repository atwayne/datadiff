using System;
using System.IO;
using lastr2d2.Tools.DataDiff.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace lastr2d2.Tools.DataDiff.CoreTests
{
    [TestClass]
    public class ExcelGeneratorTests
    {
        [TestMethod]
        public void ExportTest_Success()
        {
            var dataTable = TestHelper.MockDataTable("Testcase");
            var path = string.Format(@"D:\Test_{0}.xlsx", DateTime.Now.Ticks);
            var excelExport = new ExcelGenerator();
            var workbook = excelExport.Export(dataTable);
            workbook.SaveAs(path);
            Assert.IsTrue(File.Exists(path));
            File.Delete(path);
        }

        [TestMethod]
        public void HightLightText_Success()
        {
            var ticks = DateTime.Now.Ticks;
            var dataTable = TestHelper.MockMergedDataTable();
            var excelExport = new ExcelGenerator();
            var workbook = excelExport.Export(dataTable);
            var worksheet = workbook.Worksheet("Result");
            excelExport.Highlight(worksheet, "_UAT", "_PROD", "_Gap");
            var highlightPath = string.Format(@"D:\hightlight_{0}.xlsx", ticks);
            workbook.SaveAs(highlightPath);
        }
    }
}
