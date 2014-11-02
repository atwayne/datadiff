using LastR2D2.Tools.DataDiff.Core;
using System;
using System.IO;
using System.Threading.Tasks;
using Task = LastR2D2.Tools.DataDiff.Core.Model.Task;

namespace LastR2D2.Tools.DataDiff.Deploy
{
    public static class MainApp
    {
        private static int Main(string[] args)
        {
            // get options from command line arguments
            var options = new CommandLineOptions();
            if (!CommandLine.Parser.Default.ParseArguments(args, options))
            {
                return -1;
            }

            // get configs from app.config
            Config.Load(options);

            // create diff Options
            var diffOptions = new DiffClientOptions
            {
                DefaultOutputFilePath = Config.DefaultOutputFile,
                DefaultTimeout = Config.DefaultDatabaseQueryTimeout,
                QueryParameters = Config.QueryParameters,
                SuffixOfGapColumn = Config.DefaultSuffixOfGapColumn,
                SuffixOfCompareResultColumn = Config.DefaultSuffixOfCompareColumn
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

        private static void ProcessTask(string path, DiffClientOptions diffOptions, object exportLockObject)
        {
            var tasks = Task.LoadFromXml(path);
            Parallel.ForEach(tasks, new ParallelOptions { MaxDegreeOfParallelism = 5 },
                task =>
                {
                    var differ = new DiffClient(task, diffOptions, exportLockObject);
                    differ.Diff();
                });
        }
    }
}