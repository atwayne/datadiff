using System.Data;
using System.Data.SqlClient;
using System.Diagnostics.Contracts;

namespace lastr2d2.Tools.DataDiff.Core
{
    public class SqlServerHelper: IDataHelper
    {
        private string connectionString;

        public SqlServerHelper(string connectionString) {
            Contract.Requires(!string.IsNullOrEmpty(connectionString));

            this.connectionString = connectionString;
        }

        public DataTable GetDataTable(string query) {
            Contract.Requires(!string.IsNullOrEmpty(query));
            var dataTable = new DataTable();
            using (var connection = new SqlConnection(connectionString)) {
                connection.Open();
                using (var command = new SqlCommand(query, connection)) {
                    using (var dataAdapter = new SqlDataAdapter(command)) {
                        dataAdapter.Fill(dataTable);
                    }
                }
            }
            Contract.Ensures(dataTable != null);
            return dataTable;
        }
    }
}
