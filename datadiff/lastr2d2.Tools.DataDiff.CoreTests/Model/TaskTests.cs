using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace lastr2d2.Tools.DataDiff.Core.Model.Tests
{
    [TestClass()]
    public class TaskTests
    {
        [TestMethod()]
        public void LoadFromXmlTest()
        {
            var xmlPath = @"testcase.xml";
            var actual = Task.LoadFromXml(xmlPath);

            Assert.IsNotNull(actual);
            Assert.IsNotNull(actual.Columns);
            Assert.IsNotNull(actual.Gaps);
            Assert.IsNotNull(actual.Report);
            Assert.IsNotNull(actual.Sources);

            Assert.IsNotNull(actual.Columns.CompareColumns);
            Assert.IsNotNull(actual.Columns.PrimaryColumns);
            Assert.AreEqual(actual.Columns.CompareColumns.Length, 2);
            Assert.AreEqual(actual.Columns.PrimaryColumns.Length, 2);
            CollectionAssert.AreEqual(actual.Columns.CompareColumns, new string[] { "CompareColumn1", "CompareColumn2" });
            CollectionAssert.AreEqual(actual.Columns.PrimaryColumns, new string[] { "PrimaryColumn1", "PrimaryColumn2" });

            Assert.AreEqual(actual.Gaps.Length, 2);
            Assert.IsNotNull(actual.Gaps[0]);
            Assert.IsNotNull(actual.Gaps[1]);
            Assert.IsNotNull(actual.Gaps[0].Columns);
            Assert.IsNotNull(actual.Gaps[1].Columns);
            CollectionAssert.AreEqual(actual.Gaps[0].Columns, new string[] { "CompareColumn1", "CompareColumn2" });
            CollectionAssert.AreEqual(actual.Gaps[1].Columns, new string[] { "CompareColumn3", "CompareColumn4" });
            Assert.AreEqual(actual.Gaps[0].Value, 0.95);
            Assert.AreEqual(actual.Gaps[1].Value, 0.98);

            Assert.AreEqual(actual.Report.Path, @"E:\report.xlsx");

            Assert.AreEqual(actual.Sources.Length, 2);
            Assert.IsNotNull(actual.Sources[0]);
            Assert.IsNotNull(actual.Sources[1]);

            Assert.AreEqual(actual.Sources[0].Name, "UAT");
            Assert.AreEqual(actual.Sources[0].ConnectionString, "UAT ConnectionString");
            Assert.AreEqual(actual.Sources[0].QueryString, "UAT QueryString");

            Assert.AreEqual(actual.Sources[1].Name, "QA");
            Assert.AreEqual(actual.Sources[1].ConnectionString, "QA ConnectionString");
            Assert.AreEqual(actual.Sources[1].QueryString, "QA QueryString");
        }
    }
}