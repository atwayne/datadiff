using System.Data;
using ClosedXML.Excel;

namespace lastr2d2.Tools.DataDiff.Core
{
    internal interface IReportGenerator
    {
        XLWorkbook Export(DataTable dataTable, string alias = null, string path = null);
    }
}