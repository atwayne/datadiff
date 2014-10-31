using ClosedXML.Excel;
namespace LastR2D2.Tools.DataDiff.Core.Interfaces
{
    public interface IExcelHighlighter
    {
        void Highlight(IXLWorksheet excel, HighlightOptions options);
    }
}
