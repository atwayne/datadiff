﻿using System.Data;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace lastr2d2.Tools.DataDiff.Core.Tests
{
    
    
    /// <summary>
    ///This is a test class for SQLServerHelperTest and is intended
    ///to contain all SQLServerHelperTest Unit Tests
    ///</summary>
    [TestClass()]
    public class SQLServerHelperTest
    {


        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #region Additional test attributes
        // 
        //You can use the following additional attributes as you write your tests:
        //
        //Use ClassInitialize to run code before running the first test in the class
        //[ClassInitialize()]
        //public static void MyClassInitialize(TestContext testContext)
        //{
        //}
        //
        //Use ClassCleanup to run code after all tests in a class have run
        //[ClassCleanup()]
        //public static void MyClassCleanup()
        //{
        //}
        //
        //Use TestInitialize to run code before running each test
        //[TestInitialize()]
        //public void MyTestInitialize()
        //{
        //}
        //
        //Use TestCleanup to run code after each test has run
        //[TestCleanup()]
        //public void MyTestCleanup()
        //{
        //}
        //
        #endregion


        /// <summary>
        ///A test for GetDataTable
        ///</summary>
        [TestMethod()]
        public void GetDataTableTest()
        {
            string connectionString = ""; // TODO: Initialize to an appropriate value
            SQLServerHelper target = new SQLServerHelper(connectionString); 
            string query = @""; 
            DataTable actual;
            actual = target.GetDataTable(query);

            Assert.IsNotNull(actual);
            Assert.AreEqual<int>(actual.Columns.Count, 12);
        }
    }
}
