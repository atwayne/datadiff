using System;
namespace lastr2d2.Tools.DataDiff.Core
{
    interface IReportGenerator
    {
        void Export(System.Data.DataTable dataTable, string path, string alias = "");
    }
}
