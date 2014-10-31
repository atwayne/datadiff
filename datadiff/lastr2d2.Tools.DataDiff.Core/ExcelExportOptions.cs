namespace LastR2D2.Tools.DataDiff.Core
{
    public class ExcelExportOptions : ExportOptions
    {
        public string SheetName { get; private set; }

        public HighlightOptions HighlightOptions { get; private set; }

        public ExcelExportOptions(string sheetName, string path, HighlightOptions highlightOptions)
        {
            SheetName = sheetName;
            Path = path;
            HighlightOptions = highlightOptions;
        }
    }
}
