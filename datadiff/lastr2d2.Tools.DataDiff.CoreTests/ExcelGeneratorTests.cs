using System;
using System.Globalization;
using System.IO;
using LastR2D2.Tools.DataDiff.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LastR2D2.Tools.DataDiff.CoreTests
{
    [TestClass]
    public class ExcelGeneratorTests
    {
        [TestMethod]
        public void ExportTestThenSuccess()
        {
            var dataTable = TestHelper.MockDataTable("Test-case");
            var path = string.Format(CultureInfo.CurrentCulture, @"D:\Test_{0}.xlsx", DateTime.Now.Ticks);

            var workbook = ExcelGenerator.Export(dataTable);
            workbook.SaveAs(path);
            Assert.IsTrue(File.Exists(path));
            File.Delete(path);
        }

        [TestMethod]
        public void HighlightTextThenSuccess()
        {
            var ticks = DateTime.Now.Ticks;
            using (var dataTable = TestHelper.MockMergedDataTable())
            {
                var workbook = ExcelGenerator.Export(dataTable);
                var worksheet = workbook.Worksheet("Result");
                ExcelGenerator.Highlight(worksheet, "_UAT", "_PROD", "_Gap");
                var highlightPath = string.Format(CultureInfo.CurrentCulture, @"D:\hightlight_{0}.xlsx", ticks);
                workbook.SaveAs(highlightPath);
            }
        }
    }
}