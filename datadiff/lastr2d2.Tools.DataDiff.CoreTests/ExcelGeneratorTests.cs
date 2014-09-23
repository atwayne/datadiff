using System;
using System.Data;
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
            var dataTable = MockDataTable("Testcase");
            var path = string.Format(@"E:\Test_{0}.xlsx",DateTime.Now.Ticks);
            var excelExport = new ExcelGenerator();
            excelExport.Export(dataTable,path);
            Assert.IsTrue(File.Exists(path));
            File.Delete(path);
        }

        internal static DataTable MockDataTable(string tableName)
        {
            var table = new DataTable();
            table.TableName = tableName;
            table.Columns.Add("Dosage", typeof(int));
            table.Columns.Add("Drug", typeof(string));
            table.Columns.Add("Patient", typeof(string));
            table.Columns.Add("Date", typeof(DateTime));

            table.Rows.Add(25, "Indocin", "David", DateTime.Now);
            table.Rows.Add(50, "Enebrel", "Sam", DateTime.Now);
            table.Rows.Add(10, "Hydralazine", "Christoff", DateTime.Now);
            table.Rows.Add(21, "Combivent", "Janet", DateTime.Now);
            table.Rows.Add(100, "Dilantin", "Melanie", DateTime.Now);
            return table;
        }
    }
}
