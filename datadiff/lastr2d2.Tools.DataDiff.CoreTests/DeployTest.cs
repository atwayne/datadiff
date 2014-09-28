using System;
using System.Data;
using lastr2d2.Tools.DataDiff.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace lastr2d2.Tools.DataDiff.CoreTests
{
    [TestClass]
    public class DeployTest
    {
        [TestMethod]
        public void TestMethod1()
        {
            var uatConnectionString = "";
            var qaConnectionString = "";

            var uatQuery =
            @"";
            var qaQuery = uatQuery;

            var uatSqlServerHelper = new SQLServerHelper(uatConnectionString);
            var qaSqlServerHelper = new SQLServerHelper(qaConnectionString);

            var uatResult = uatSqlServerHelper.GetDataTable(uatQuery);
            var qaResult = qaSqlServerHelper.GetDataTable(qaQuery);

            uatResult.PrimaryKey = new DataColumn[]{
                uatResult.Columns[""]
            };

            qaResult.PrimaryKey = new DataColumn[]{
                qaResult.Columns[""]
            };

            var diff = DataTableMerger.Merge(uatResult, qaResult, null, "UAT", "QA");

            var path = string.Format(@"E:\Test_{0}.xlsx", DateTime.Now.Ticks);
            var excelExport = new ExcelGenerator();
            excelExport.Export(uatResult, path, "UAT");
            excelExport.Export(qaResult, path, "QA");
            excelExport.Export(diff, path, "Diff");


        }
    }
}
