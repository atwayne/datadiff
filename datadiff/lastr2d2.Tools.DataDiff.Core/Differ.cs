using ClosedXML.Excel;
using LastR2D2.Tools.DataDiff.Core.Interfaces;
using Task = LastR2D2.Tools.DataDiff.Core.Model.Task;

namespace LastR2D2.Tools.DataDiff.Core
{
    public class Differ : IDiffer
    {
        private IDataReader ReaderOfLeftDataSource { get; set; }
        private IDataReader ReaderOfRightDataSource { get; set; }
        private IDataMerger DataMerger { get; set; }
        private IDataExporter<XLWorkbook> DataExporter { get; set; }
        private IExcelHighlighter ExcelHighlighter { get; set; }
        private IColumnNameBuilder ColumnNameBuilder { get; set; }

        private DataReadOptions ReadOptionsOfLeftSource { get; set; }
        private DataReadOptions ReadOptionsOfRightSource { get; set; }

        private MergeOptions MergeOptions { get; set; }

        private ExportOptions ExportOptions { get; set; }

        private object ExportLockObject { get; set; }

        public Differ(Task task, DiffOptions options, object exportLockObject)
        {
            task.LoadConfig(options.DefaultOutputFilePath, options.QueryParameters);

            ExportLockObject = exportLockObject;

            var leftDataSourceSetting = task.Sources[0];
            var rightDataSourceSetting = task.Sources[1];

            ReaderOfLeftDataSource = new SqlServerDataReader();
            ReaderOfRightDataSource = new SqlServerDataReader();

            ReadOptionsOfLeftSource = new SqlServerReaderOptions(leftDataSourceSetting.Name, task.Columns.PrimaryColumns,
                leftDataSourceSetting.ConnectionString, leftDataSourceSetting.QueryString, leftDataSourceSetting.QueryParameters, options.DefaultTimeout);
            ReadOptionsOfRightSource = new SqlServerReaderOptions(rightDataSourceSetting.Name, task.Columns.PrimaryColumns,
                rightDataSourceSetting.ConnectionString, rightDataSourceSetting.QueryString, rightDataSourceSetting.QueryParameters, options.DefaultTimeout);

            MergeOptions = new MergeOptions(leftDataSourceSetting.Name, rightDataSourceSetting.Name,
                task.Columns.CompareColumns, task.GapMapping);

            ColumnNameBuilder = new ExcelColumnNameBuilder(options.SuffixOfGapColumn, options.SuffixOfCompareResultColumn);

            ExcelHighlighter = new ExcelHighlighter(ColumnNameBuilder);

            DataExporter = new ExcelExporter();
            var highlightOptions = new HighlightOptions(leftDataSourceSetting.Name, rightDataSourceSetting.Name);
            ExportOptions = new ExcelExportOptions(task.Name, task.Report.Path, highlightOptions);
        }

        public void Diff()
        {
            var dataTableFromLeftSource = ReaderOfLeftDataSource.Read(ReadOptionsOfLeftSource);
            var dataTableFromRightSource = ReaderOfRightDataSource.Read(ReadOptionsOfRightSource);

            DataMerger = new DataMerger(dataTableFromLeftSource, dataTableFromRightSource
                , MergeOptions
                , ColumnNameBuilder);
            var mergeResult = DataMerger.Merge();

            lock (ExportLockObject)
            {
                DataExporter.Export(mergeResult, ExportOptions, ExcelHighlighter);
            }
        }
    }
}
