using System;
using System.Dynamic;
using System.IO;
using System.Text;

namespace logger
{
    public static class Logger
    {
        public static readonly bool IsLoggerStarted = false;
        public static volatile StringBuilder LogContent = new StringBuilder();
        public static volatile StringBuilder FullLog = new StringBuilder();
        public static string LogName = "Events.log";

        /// <summary>
        /// Logs input values into log file, which is saved upon end of the program. 
        /// </summary>
        /// <param name="args"> Anything convertible to string </param>
        public static void Log(params object[] args)
        {
            LogContent.Append("[" + DateTime.Now.ToLongTimeString() + "]");
            FullLog.Append("[" + DateTime.Now.ToLongTimeString() + "]");
            foreach (var arg in args)
            {
                Console.Write(arg);
                LogContent.Append(arg);
                FullLog.Append(arg);
            }
            Console.WriteLine();
            LogContent.AppendLine();
            FullLog.AppendLine();
        }

        /// <summary>
        /// Log showing output to console only when ]se mode is enabled.
        /// </summary>
        /// <param name="args"></param>
        public static void TalkyLog(params object[] args)
        {
            LogContent.Append("[" + DateTime.Now.ToLongTimeString() + "]");
            FullLog.Append("[" + DateTime.Now.ToLongTimeString() + "]");
            foreach (var arg in args)
            {
                if (loader.Init._cfg.Verbose)
                    Console.Write(arg);
                LogContent.Append(arg);
                FullLog.Append(arg);
            }
            if (loader.Init._cfg.Verbose)
                Console.WriteLine();
            LogContent.AppendLine();
            FullLog.AppendLine();
        }

        /// <summary>
        /// Logs input values into log file, but does not create a new line.
        /// </summary>
        /// <param name="args"> Anything convertible to string </param>
        public static void LogNoNl(params object[] args)
        {
            foreach (var arg in args)
            {
                Console.Write(arg);
                LogContent.Append(arg);
                FullLog.Append(arg);
            }
        }
        /// <summary>
        /// Logs without telling anyone anything.
        /// </summary>
        /// <param name="args"></param>
        public static void LogNoTrace(params object[] args)
        {
            LogContent.Append("[" + DateTime.Now.ToLongTimeString() + "]");
            FullLog.Append("[" + DateTime.Now.ToLongTimeString() + "]");
            foreach (var arg in args)
                LogContent.Append(arg);
            FullLog.AppendLine();
            LogContent.AppendLine();
        }
        /// <summary>
        /// Inits the logger
        /// </summary>
        static Logger()
        {
            IsLoggerStarted = true;
            Log("Logger initialized.");
            loader.Init.Exit += (x, y) => Save();
        }

        public static void SaveLog()
        {
            var path = Path.Combine(Environment.CurrentDirectory, LogName);
            var existingContents = "";
            if (File.Exists(path))
                existingContents = File.ReadAllText(path);
            var output = new StreamWriter(path);
            output.Write(existingContents);
            output.WriteLine();
            output.Write(LogContent);
            output.Flush();
            output.Close();
            LogContent.Clear();
        }
        /// <summary>
        /// Saves log to a file with custom filename.
        /// </summary>
        /// <param name="filename"></param>
        public static void SaveLog(string filename)
        {
            var separateAdditions = false;
            if (!string.IsNullOrEmpty(filename) && !string.IsNullOrWhiteSpace(filename))
            {
                var path = Path.Combine(Environment.CurrentDirectory, filename);
                var existingContents = "";
                if (File.Exists(path))
                {
                    existingContents = File.ReadAllText(path);
                    separateAdditions = true;
                }
                var output = new StreamWriter(path);
                output.Write(existingContents);
                if (separateAdditions)
                {
                    output.WriteLine("\n");
                    output.WriteLine("\n");
                }
                output.Write("\n" + DateTime.Now.ToLongTimeString() + "\n");
                output.Write(LogContent);
                output.Close();
                LogContent.Clear();
            }
        }
        public static void ViewLog(StreamWriter p) { p.WriteLine(FullLog.ToString()); }

        public static void LogWorker()
        {
            while (true)
            {
                if (!loader.Init.PendingLogSave) continue;
                SaveLog();
                loader.Init.PendingLogSave = false;
            }
        }

        public static void Save()
        {
            if (loader.Init._cfg.Asynchlog)
                loader.Init.PendingLogSave = true;
            else
                SaveLog();
        }
    }
}
