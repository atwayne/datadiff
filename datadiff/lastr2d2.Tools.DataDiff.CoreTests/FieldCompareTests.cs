using System;
using LastR2D2.Tools.DataDiff.Core;
using LastR2D2.Tools.DataDiff.Core.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LastR2D2.Tools.DataDiff.CoreTests
{
    [TestClass]
    public class FieldCompareTests
    {
        [TestMethod]
        public void CompareIdenticalDateTimeThenSuccess()
        {
            var leftDateTime = DateTime.Now;
            var rightDateTime = leftDateTime;

            var dataTable = TestHelper.MockDataTable(withData: false);
            var leftRow = dataTable.NewRow();
            var rightRow = dataTable.NewRow();
            leftRow["datetime"] = leftDateTime;
            rightRow["datetime"] = rightDateTime;

            var field = new Field
            {
                Name = "datetime",
                IsKey = false,
                FieldType = typeof(DateTime)
            };

            var expected = FieldCompareResult.Identical;
            var actual = FieldCompare.Compare(leftRow, rightRow, field);

            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void CompareTest_DateTime_QuiteDifferent()
        {
            var leftDateTime = DateTime.Now;
            var rightDateTime = DateTime.Now.AddDays(-1);

            var dataTable = TestHelper.MockDataTable(withData: false);
            var leftRow = dataTable.NewRow();
            var rightRow = dataTable.NewRow();
            leftRow["datetime"] = leftDateTime;
            rightRow["datetime"] = rightDateTime;

            var field = new Field
            {
                Name = "datetime",
                IsKey = false,
                FieldType = typeof(DateTime)
            };

            var expected = FieldCompareResult.QuiteDifferent;
            var actual = FieldCompare.Compare(leftRow, rightRow, field);

            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void CompareTest_Double_Identical()
        {
            var leftDouble = 1.5e7;
            var rightDouble = leftDouble;

            var dataTable = TestHelper.MockDataTable(withData: false);
            var leftRow = dataTable.NewRow();
            var rightRow = dataTable.NewRow();
            leftRow["double"] = leftDouble;
            rightRow["double"] = rightDouble;

            var field = new Field
            {
                Name = "double",
                IsKey = false,
                FieldType = typeof(double)
            };

            var expected = FieldCompareResult.Identical;
            var actual = FieldCompare.Compare(leftRow, rightRow, field);

            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void CompareTest_Double_Similar()
        {
            var leftDouble = 1.5e7;
            var rightDouble = leftDouble * 0.99;

            var dataTable = TestHelper.MockDataTable(withData: false);
            var leftRow = dataTable.NewRow();
            var rightRow = dataTable.NewRow();
            leftRow["double"] = leftDouble;
            rightRow["double"] = rightDouble;

            var field = new Field
            {
                Name = "double",
                IsKey = false,
                FieldType = typeof(double),
                Gap = 0.02
            };

            var expected = FieldCompareResult.Similar;
            var actual = FieldCompare.Compare(leftRow, rightRow, field);

            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void CompareTest_Double_QuiteDifferent()
        {
            var leftDouble = 1.5e7;
            var rightDouble = leftDouble * 0.95;

            var dataTable = TestHelper.MockDataTable(withData: false);
            var leftRow = dataTable.NewRow();
            var rightRow = dataTable.NewRow();
            leftRow["double"] = leftDouble;
            rightRow["double"] = rightDouble;

            var field = new Field
            {
                Name = "double",
                IsKey = false,
                FieldType = typeof(double),
                Gap = 0.02
            };

            var expected = FieldCompareResult.QuiteDifferent;
            var actual = FieldCompare.Compare(leftRow, rightRow, field);

            Assert.AreEqual(expected, actual);
        }
    }
}