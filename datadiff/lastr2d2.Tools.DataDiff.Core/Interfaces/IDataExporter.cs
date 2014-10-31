﻿using System.Data;
namespace LastR2D2.Tools.DataDiff.Core.Interfaces
{
    interface IDataExporter<out T>
    {
        T Export(DataTable dataTable, ExportOptions options, IExcelHighlighter highLighter);
    }
}
