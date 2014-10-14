using System.Collections.Generic;
using System.Data;

namespace lastr2d2.Tools.DataDiff.Core
{
    public interface IDataHelper
    {
        DataTable GetDataTable(string query, IDictionary<string, string> parameters);
    }
}