using System;
using System.Data;

namespace lastr2d2.Tools.DataDiff.Core
{
    public interface IDataTableComparator
    {
        DataTable GetDifference(DataTable firstTable, DataTable secondTable);
    }
}
