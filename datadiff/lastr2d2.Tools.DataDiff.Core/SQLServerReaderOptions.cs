using System.Collections.Generic;

namespace LastR2D2.Tools.DataDiff.Core
{
    public class SqlServerReaderOptions : DataReadOptions
    {
        public string ConnectionString { get; private set; }
        public string QueryString { get; private set; }
        public IDictionary<string, string> QueryParameters { get; private set; }
        public int QueryTimeout { get; private set; }

        public SqlServerReaderOptions(
            string tableName, ICollection<string> primaryColumnNames,
            string connectionString, string queryString, IDictionary<string, string> queryParameters = null, int queryTimeout = 30)
        {
            TableName = tableName;
            PrimaryColumnNames = primaryColumnNames;
            ConnectionString = connectionString;
            QueryParameters = queryParameters;
            QueryTimeout = queryTimeout;
            QueryString = queryString;
        }
    }
}
