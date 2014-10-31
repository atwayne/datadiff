using LastR2D2.Tools.DataDiff.Core;
using System;
using System.IO;
using System.Threading.Tasks;
using Task = LastR2D2.Tools.DataDiff.Core.Model.Task;

namespace LastR2D2.Tools.DataDiff.Deploy
{
    public static class Deploy
    {
        private static int Main(string[] args)
        {
            var options = new DeployOptions();
            if (!CommandLine.Parser.Default.ParseArguments(args, options))
            {
                return -1;
            }

            Config.Load(options);

            var diffOptions = new DiffOptions
            {
                DefaultOutputFilePath = Config.DefaultOutputFile,
                DefaultTimeout = Config.DefaultDatabaseQueryTimeout,
                QueryParameters = Config.QueryParameters,
                SuffixOfGapColumn = "_Gap",
                SuffixOfCompareResultColumn = "_Compare"
            };

            var pathOfInput = (args == null || args.Length < 1) ? Config.DefaultInputPath : args[0];
            if (Directory.Exists(pathOfInput))
            {
                var xmlFiles = Directory.GetFiles(pathOfInput, Config.DefaultInputFileNamePattern);

                Parallel.ForEach(xmlFiles, filePath =>
                {
                    try
                    {
                        ProcessTask(filePath, diffOptions, Config.DefaultOutputFileLock);
                    }
                    catch (Exception exception)
                    {
                        Console.WriteLine(exception);
                    }
                });
            }
            else if (File.Exists(pathOfInput))
            {
                ProcessTask(pathOfInput, diffOptions, Config.DefaultOutputFileLock);
            }
            else
            {
                return -1;
            }

            return 0;
        }

        private static void ProcessTask(string path, DiffOptions diffOptions, object exportLockObject)
        {
            var tasks = Task.LoadFromXml(path);
            Parallel.ForEach(tasks, new ParallelOptions { MaxDegreeOfParallelism = 5 },
                task => ProcessTask(task, diffOptions, exportLockObject));
        }

        private static void ProcessTask(Task task, DiffOptions diffOptions, object exportLockObject)
        {
            var differ = new Differ(task, diffOptions, exportLockObject);
            differ.Diff();
        }
    }
}