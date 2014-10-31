using System.Collections.Generic;

namespace LastR2D2.Tools.DataDiff.Core
{
    public class SQLServerReaderOptions : DataReadOptions
    {
        public string ConnectionString { get; private set; }
        public string QueryString { get; private set; }
        public IDictionary<string, string> QueryParameters { get; private set; }
        public int QueryTimeout { get; private set; }

        public SQLServerReaderOptions(
            string tableName, ICollection<string> primaryColumnNames,
            string connectionString, string queryString, IDictionary<string, string> queryParameters = null, int queryTimeout = 30)
        {
            this.TableName = tableName;
            this.PrimaryColumnNames = primaryColumnNames;
            this.ConnectionString = connectionString;
            this.QueryParameters = queryParameters;
            this.QueryTimeout = queryTimeout;
            this.QueryString = queryString;
        }
    }
}
