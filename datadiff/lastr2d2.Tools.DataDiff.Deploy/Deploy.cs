using System;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using lastr2d2.Tools.DataDiff.Core;
using Task = lastr2d2.Tools.DataDiff.Core.Model.Task;
namespace lastr2d2.Tools.DataDiff.Deploy
{
    public class Deploy
    {
        private static int Main(string[] args)
        {
            var path = (args == null || args.Length < 1) ? "/tasks/" : args[0];
            if (Directory.Exists(path))
            {
                var xmlFiles = Directory.GetFiles(path, "*.xml");
                Parallel.ForEach(xmlFiles, filePath =>
                {
                    try
                    {
                        ProcessTask(filePath);
                    }
                    catch (Exception exception)
                    {
                        Console.WriteLine(exception);
                    }
                });
            }
            else if (File.Exists(path))
            {
                ProcessTask(path);
            }
            else {
                return -1;
            }

            return 0;
        }

        private static void ProcessTask(string path)
        {
            var task = Task.LoadFromXml(path);

            var leftDataTable = PrepareDataTable(task, 0);
            var rightDataTable = PrepareDataTable(task, 1);
            var mergeResult = DataTableMerger.Merge(leftDataTable, rightDataTable,
                compareColumnNames: task.Columns.CompareColumns.ToList(),
                gapSettingForNumbericColumn: task.GapMapping);

            mergeResult.TableName = string.IsNullOrWhiteSpace(task.Name) ? "Result" : task.Name;

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
