using System.Data;
using lastr2d2.Tools.DataDiff.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace lastr2d2.Tools.DataDiff.CoreTests
{
    
    
    /// <summary>
    ///This is a test class for SQLServerHelperTest and is intended
    ///to contain all SQLServerHelperTest Unit Tests
    ///</summary>
    [TestClass]
    public class SqlServerHelperTest
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
            var target = new SQLServerHelper(connectionString); 
            var query = @""; 
            DataTable actual = target.GetDataTable(query);

            Assert.IsNotNull(actual);
            Assert.AreEqual(actual.Columns.Count, 12);
        }
    }
}
