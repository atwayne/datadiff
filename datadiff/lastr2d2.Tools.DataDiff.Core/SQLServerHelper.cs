using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics.Contracts;
using System.Globalization;

namespace LastR2D2.Tools.DataDiff.Core
{
    public class SqlServerHelper : IDataHelper
    {
        private string connectionString;

        public SqlServerHelper(string connectionString)
        {
            Contract.Requires(!string.IsNullOrEmpty(connectionString));
            this.connectionString = connectionString;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2100:Review SQL queries for security vulnerabilities"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope")]
        public DataTable GetDataTable(string query, IDictionary<string, string> parameters = null)
        {
            Contract.Requires(!string.IsNullOrEmpty(query));
            var dataTable = new DataTable() { Locale = CultureInfo.CurrentCulture };
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using (var command = new SqlCommand(query, connection))
                {
                    if (parameters != null && parameters.Count > 0)
                    {
                        foreach (var pair in parameters)
                        {
                            command.Parameters.AddWithValue(pair.Key, pair.Value);
                        }
                    }

                    using (var dataAdapter = new SqlDataAdapter(command))
                    {
                        dataAdapter.Fill(dataTable);
                    }
                }
            }
            Contract.Ensures(dataTable != null);
            return dataTable;
        }
    }
}