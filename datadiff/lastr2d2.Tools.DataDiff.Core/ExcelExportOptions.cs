namespace LastR2D2.Tools.DataDiff.Core
{
    public class ExcelExportOptions : ExportOptions
    {
        public string SheetName { get; private set; }

        public ExcelExportOptions(string sheetName, string path)
        {
            SheetName = sheetName;
            Path = path;
        }
    }
}
