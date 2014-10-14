using System;
using System.Data;
using System.Globalization;

namespace LastR2D2.Tools.DataDiff.CoreTests
{
    internal static class TestHelper
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope")]
        public static DataTable MockDataTable(string tableName = "test", bool withData = true)
        {
            var table = new DataTable { TableName = tableName, Locale = CultureInfo.CurrentCulture };
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

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope")]
        public static DataTable MockMergedDataTable(string tableName = "Result")
        {
            var table = new DataTable { TableName = tableName, Locale = CultureInfo.CurrentCulture };
            table.Columns.Add("Id", typeof(int));

            table.Columns.Add("DV01_UAT", typeof(double));
            table.Columns.Add("DV01_PROD", typeof(double));
            table.Columns.Add("DV01_Gap", typeof(double));

            table.Columns.Add("CV01_UAT", typeof(double));
            table.Columns.Add("CV01_PROD", typeof(double));
            table.Columns.Add("CV01_Gap", typeof(double));

            table.Columns.Add("LastUpdated_UAT", typeof(DateTime));
            table.Columns.Add("LastUpdated_PROD", typeof(DateTime));
            table.Columns.Add("LastUpdated_Gap", typeof(double));

            const int records = 7;
            var index = 0;
            var dice = new Random();
            while (index++ < records)
            {
                var dv01 = dice.NextDouble();
                var cv01 = dice.NextDouble();
                var lastUpdated = DateTime.Now;

                table.Rows.Add(index, dv01, dv01, dice.NextDouble(), cv01, cv01, dice.NextDouble(), lastUpdated,
                    lastUpdated, dice.NextDouble());
            }

            table.Rows[1]["DV01_UAT"] = (double)table.Rows[1]["DV01_PROD"] * (1 + (double)table.Rows[1]["DV01_Gap"]);
            table.Rows[2]["DV01_UAT"] = (double)table.Rows[2]["DV01_PROD"] * (1 - (double)table.Rows[2]["DV01_Gap"]);
            table.Rows[3]["DV01_PROD"] = DBNull.Value;
            table.Rows[4]["DV01_PROD"] = DBNull.Value;
            table.Rows[4]["CV01_PROD"] = DBNull.Value;
            table.Rows[4]["LastUpdated_PROD"] = DBNull.Value;

            return table;
        }
    }
}