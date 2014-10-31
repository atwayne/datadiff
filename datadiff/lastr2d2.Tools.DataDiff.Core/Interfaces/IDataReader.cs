using System.Data;

namespace LastR2D2.Tools.DataDiff.Core.Interfaces
{
    interface IDataReader
    {
        DataTable ReadData(DataReadOptions options);
    }
}
