using System.Collections.Generic;
namespace LastR2D2.Tools.DataDiff.Core.Interfaces
{
    public interface IColumnNameBuilder
    {
        string BuildColumName(string nameOfDataSource, string nameOfColumn);
        string BuildGapColumnName(string nameOfColumn);
        string BuildCompareResultColumnName(string nameOfColumn);

        bool IsGapColumn(string nameOfColumn);
        bool IsCompareResultColumn(string nameOfColumn);
        bool IsCompareColumn(string nameOfDataSource, string nameOfColumn);

        bool IsGeneratedColumn(string nameOfColumn, ICollection<string> nameOfDataSources);
        ICollection<string> GetUnderlyingColumnNames(IEnumerable<string> nameOfColumns, ICollection<string> nameOfDataSources);
    }
}
