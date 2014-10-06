using System.Data;
using System.Linq;
using lastr2d2.Tools.DataDiff.Core;
using lastr2d2.Tools.DataDiff.Core.Model;

namespace lastr2d2.Tools.DataDiff.Deploy
{
    public class Deploy
    {
        private static void Main()
        {
            var task = Task.LoadFromXml("config.xml");

            var leftDataTable = PrepareDataTable(task, 0);
            var rightDataTable = PrepareDataTable(task, 1);
            var mergeResult = DataTableMerger.Merge(leftDataTable, rightDataTable,
                compareColumns: task.Columns.CompareColumns.ToList(),
                gapSettingForNumbericColumn: task.GapMapping);
            
            mergeResult.TableName = "Result";

            ExportToExcel(task, leftDataTable, rightDataTable, mergeResult);
        }

        private static void ExportToExcel(Task task, DataTable leftDataTable, DataTable rightDataTable, DataTable mergeResult)
        {
            var execelGenerator = new ExcelGenerator();
            var workbook = execelGenerator.Export(mergeResult, path: task.Report.Path);

            var worksheet = workbook.Worksheet(mergeResult.TableName);
            execelGenerator.Highlight(worksheet,
                "_" + leftDataTable.TableName,
                "_" + rightDataTable.TableName);
            workbook.SaveAs(task.Report.Path);
        }

        private static DataTable PrepareDataTable(Task task, int sourceIndex)
        {
            var source = task.Sources[sourceIndex];
            var sqlServer = new SqlServerHelper(source.ConnectionString);
            var dataTable = sqlServer.GetDataTable(source.QueryString);
            dataTable.TableName = source.Name;

            dataTable.PrimaryKey = task.Columns.PrimaryColumns.Select(column => dataTable.Columns[column]).ToArray();
            return dataTable;
        }
    }
}
