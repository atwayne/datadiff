using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;


namespace lastr2d2.Tools.DataDiff.Core.Tests
{
    [TestClass()]
    public class ExcelGeneratorTests
    {
        [TestMethod()]
        public void ExportTest_Success()
        {
            var dataTable = TestHelper.MockDataTable("Testcase");
            var path = string.Format(@"E:\Test_{0}.xlsx",DateTime.Now.Ticks);
            var excelExport = new ExcelGenerator();
            excelExport.Export(dataTable,path);
            Assert.IsTrue(File.Exists(path));
            File.Delete(path);
        }
    }
}
