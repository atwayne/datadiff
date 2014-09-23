using lastr2d2.Tools.DataDiff.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Data;
using lastr2d2.Tools.DataDiff.Core.Tests;

namespace lastr2d2.Tools.DataDiff.CoreTests
{
    
    
    /// <summary>
    ///This is a test class for DataTableComparatorTest and is intended
    ///to contain all DataTableComparatorTest Unit Tests
    ///</summary>
    [TestClass()]
    public class DataTableComparatorTest
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
        ///A test for GetDifference
        ///</summary>
        [TestMethod()]
        public void GetDifferenceTest_NoDifference()
        {
            DataTableComparator target = new DataTableComparator();
            DataTable firstTable = ExcelGeneratorTests.MockDataTable("firstTable");
            DataTable secondTable = firstTable.Copy(); 
            DataTable expected = null; // TODO: Initialize to an appropriate value
            DataTable actual;
            actual = target.GetDifference(firstTable, secondTable);

            var path = string.Format(@"E:\Test_{0}.xlsx",DateTime.Now.Ticks);
            var excelExport = new ExcelGenerator();
            excelExport.Export(firstTable, path,"first");
            excelExport.Export(secondTable, path, "second");
            excelExport.Export(actual, path, "diff");

            Assert.AreEqual(expected, actual);
        }


    }
}
