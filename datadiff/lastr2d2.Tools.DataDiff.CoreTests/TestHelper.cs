using System;
using System.Data;

namespace lastr2d2.Tools.DataDiff.CoreTests
{
    internal static class TestHelper
    {
        public static DataTable MockDataTable(string tableName = "test", bool withData = true)
        {
            var table = new DataTable { TableName = tableName };
            table.Columns.Add("int", typeof(int));
            table.Columns.Add("string", typeof(string));
            table.Columns.Add("datetime", typeof(DateTime));
            table.Columns.Add("double", typeof(double));
            table.Columns.Add("guid", typeof(Guid));
            table.PrimaryKey = new[] { table.Columns["guid"] };

            if (!withData) return table;


            table.Rows.Add(42, "Shanghai", DateTime.Now, 42 / 10, Guid.NewGuid());
            table.Rows.Add(36, "New York", DateTime.Now, 36 / 10, Guid.NewGuid());
            table.Rows.Add(69, null, DateTime.Now, 69 / 10, Guid.NewGuid());
            table.Rows.Add(12, "Tianjin", DateTime.Now, 12 / 10, Guid.NewGuid());
            table.Rows.Add(5, "Penn", DateTime.Now, 5 / 10, Guid.NewGuid());
            table.Rows.Add(-8, "London", DateTime.Now, -8 / 10, Guid.NewGuid());
            return table;
        }

    }
}
