using System.Data;
using LastR2D2.Tools.DataDiff.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LastR2D2.Tools.DataDiff.CoreTests
{
    /// <summary>
    ///This is a test class for SQLServerHelperTest and is intended
    ///to contain all SQLServerHelperTest Unit Tests
    ///</summary>
    [TestClass]
    public class SqlServerHelperTests
    {
        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext { get; set; }

        /// <summary>
        ///A test for GetDataTable
        ///</summary>
        [TestMethod]
        public void GetDataTableTest()
        {
            var connectionString = ""; // TODO: Initialize to an appropriate value
            var target = new SqlServerHelper(connectionString);
            var query = @"";
            DataTable actual = target.GetDataTable(query);

            Assert.IsNotNull(actual);
            Assert.AreEqual(actual.Columns.Count, 12);
        }
    }
}