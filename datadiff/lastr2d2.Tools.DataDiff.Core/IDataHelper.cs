using System.Collections.Generic;
using System.Data;

namespace LastR2D2.Tools.DataDiff.Core
{
    public interface IDataHelper
    {
        DataTable GetDataTable(string query, IDictionary<string, string> parameters);
    }
}