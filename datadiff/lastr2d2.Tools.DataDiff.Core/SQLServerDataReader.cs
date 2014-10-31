using System;
using System.Data;
using System.Linq;
using IDataReader = LastR2D2.Tools.DataDiff.Core.Interfaces.IDataReader;

namespace LastR2D2.Tools.DataDiff.Core
{
    public class SqlServerDataReader : IDataReader
    {
        public DataTable Read(DataReadOptions options) {
            var sqlServerDataReaderOptions = options as SqlServerReaderOptions;
            if (sqlServerDataReaderOptions == null)
                throw new ArgumentException("SQLServerReaderOptions only", "options");

            var sqlServerHelper = new SqlServerHelper(sqlServerDataReaderOptions.ConnectionString);
            var result = sqlServerHelper.GetDataTable(sqlServerDataReaderOptions.ConnectionString, sqlServerDataReaderOptions.QueryParameters, sqlServerDataReaderOptions.QueryTimeout);
            result.TableName = sqlServerDataReaderOptions.TableName;
            result.PrimaryKey = sqlServerDataReaderOptions.PrimaryColumnNames.Select(column => result.Columns[column]).ToArray();
            return result;
        }
    }
}
