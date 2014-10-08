using System.IO;
using System.Linq;
using lastr2d2.Tools.DataDiff.Core.Model;

namespace lastr2d2.Tools.DataDiff.Deploy
{
    internal static class TaskExtensions
    {
        public static void LoadConfig(this Task task)
        {
            LoadQueryParameters(task);
            LoadReportPath(task);
        }

        private static void LoadReportPath(Task task)
        {
            if (!string.IsNullOrEmpty(Config.DefaultOutputFile) && File.Exists(Config.DefaultOutputFile)
                && string.IsNullOrEmpty(task.Report.Path))
            {
                task.Report.Path = Config.DefaultOutputFile;
            }
        }

        private static void LoadQueryParameters(Task task)
        {
            if (Config.QueryParameters != null && Config.QueryParameters.Any())
            {
                foreach (var dataSource in task.Sources)
                {
                    if (dataSource.QueryParameters == null || !dataSource.QueryParameters.Any())
                    {
                        dataSource.QueryParameters = Config.QueryParameters;
                    }
                }
            }
        }
    }
}
