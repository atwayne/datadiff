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
            Config.Load();
            var pathOfInput = (args == null || args.Length < 1) ? Config.DefaultInputDirectory : args[0];

            if (Directory.Exists(pathOfInput))
            {
                var xmlFiles = Directory.GetFiles(pathOfInput, Config.DefaultInputFileNamePattern);

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
            else if (File.Exists(pathOfInput))
            {
                ProcessTask(pathOfInput);
            }
            else
            {
                return -1;
            }

            return 0;
        }

        private static void ProcessTask(string path)
        {
            var tasks = Task.LoadFromXml(path);
            Parallel.ForEach(tasks, new ParallelOptions() { MaxDegreeOfParallelism = 5 }, task =>
            {
                task.LoadConfig();
                ProcessTask(task);
            });
        }

        private static void ProcessTask(Task task)
        {
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
            lock (Config.DefaultOutputFileLock)
            {
                var workbook = execelGenerator.Export(mergeResult, path: task.Report.Path);
                var worksheet = workbook.Worksheet(mergeResult.TableName);
                execelGenerator.Highlight(worksheet,
                    "_" + leftDataTable.TableName,
                    "_" + rightDataTable.TableName);

                OrderWorksheets(workbook);
                workbook.SaveAs(task.Report.Path);
            }
        }

        private static void OrderWorksheets(ClosedXML.Excel.XLWorkbook workbook)
        {
            var worksheets = workbook.Worksheets.OrderBy(sheet => sheet.Name, StringComparer.OrdinalIgnoreCase).ToArray();
            for (int index = 0; index < worksheets.Length; index++)
            {
                workbook.Worksheet(worksheets[index].Name).Position = index + 1;
            }
        }

        private static DataTable PrepareDataTable(Task task, int sourceIndex)
        {
            var source = task.Sources[sourceIndex];
            var sqlServer = new SqlServerHelper(source.ConnectionString);
            var dataTable = sqlServer.GetDataTable(source.QueryString, source.QueryParameters);
            dataTable.TableName = source.Name;
            dataTable.PrimaryKey = task.Columns.PrimaryColumns.Select(column => dataTable.Columns[column]).ToArray();
            return dataTable;
        }
    }
}
