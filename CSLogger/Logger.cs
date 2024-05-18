using System.Diagnostics;
using System.Collections.Generic;

namespace CSLogger
{
    public enum LogMessageType
    {
        ALL,
        INFO,
        DEBUG,
        WARN,
        ERROR
    }

    public class Logger
    {
        // ###################################################################
        // Fields
        // ###################################################################
        private string logFileName = string.Empty;
        private string programName = string.Empty;
        private LogMessageType messageType = LogMessageType.ALL;
        private string workingDirectory = string.Empty;

        // ###################################################################
        // Constructors
        // ###################################################################

        // Create a logger to write log files to the current working directory
        // using a name based off of the program's name
        public Logger()
        {
            workingDirectory = Directory.GetCurrentDirectory();
            programName = Process.GetCurrentProcess().ProcessName;
            logFileName = getName();
        }

        // Create a logger to write log files to the current working directory
        // using the given logger name
        public Logger(string loggerName)
        {
            workingDirectory = Directory.GetCurrentDirectory();
            programName = validateName(loggerName);
            logFileName = getName();
        }

        // Create a logger to write log files along the given path using
        // the given logger name. Path can be relative or absolute
        public Logger(string directoryPath, string loggerName)
        {
            workingDirectory = validatePath(directoryPath);
            programName = validateName(loggerName);
            logFileName = getName();
        }

        // Create a default logger with a message type filter
        public static Logger CreateFilteredLogger(LogMessageType type)
        {
            Logger logger = new Logger();
            logger.setFilter(type);
            return logger;
        }

        // Create a logger using a loggerName and message type filter
        public static Logger CreateFilteredLogger(string loggerName, LogMessageType type)
        {
            Logger logger = new Logger(loggerName);
            logger.setFilter(type);
            return logger;
        }

        // Create a logger using a file path, logger name, and message type filter
        public static Logger CreateFilteredLogger(string directoryPath, string loggerName, LogMessageType type)
        {
            Logger logger = new Logger(directoryPath, loggerName);
            logger.setFilter(type);
            return logger;
        }

        // Create a default logger using command line arguments
        public static Logger CreateFilteredLogger(string[] commandArgs)
        {
            // Parse the command line into pairs
            var commands = new List<KeyValuePair<string, string>>();
            parseCommands(ref commands, ref commandArgs);

            // Set default values in case parsing fails
            LogMessageType type = LogMessageType.ALL;
            bool clear = false;
            string path = string.Empty;
            string filename = string.Empty;

            // If no commands (or invalid command line), return default logger
            if (commands.Count == 0)
            {
                return new Logger();
            }
            else
            {
                foreach (var command in commands)
                {
                    if (command.Key == "--type" || command.Key == "-t")
                    {
                        Enum.TryParse(command.Value, out type);
                    }
                    else if (command.Key == "--clear" || command.Key == "-c")
                    {
                        clear = true;
                    }
                    else if (command.Key == "--path" || command.Key == "-p")
                    {
                        path = validatePath(command.Value);
                    }
                    else if (command.Key == "--logname" || command.Key == "-ln")
                    {
                        filename = validateName(command.Value);
                    }
                }
            }

            // Create logger based off of options
            Logger logger = CreateFilteredLogger(path, filename, type);

            // If clear option set via command line, clear the log
            if (clear)
            {
                logger.clear();
            }

            return logger;
        }

        // ###################################################################
        // Private Methods
        // ###################################################################

        // Convert all / into \ characters
        private static string linuxPathToWindowsPath(string path)
        {
            string tempPath = "";
            for (int i = 0; i < path.Length; i++)
            {
                if (path[i] == '/')
                {
                    tempPath += '\\';
                }
                else
                {
                    tempPath += path[i];
                }
            }

            return tempPath;
        }

        // Parse command line arguments into a list of paired arguments
        private static void parseCommands(ref List<KeyValuePair<string, string>> pairList, ref string[] commandArgs)
        {
            for (int i = 0; i < commandArgs.Length; i++)
            {
                if (commandArgs[i] == "--type" || commandArgs[i] == "-t" ||
                    commandArgs[i] == "--path" || commandArgs[i] == "-p" ||
                    commandArgs[i] == "--logname" || commandArgs[i] == "-ln")
                {
                    if (i + 1 < commandArgs.Length)
                    {
                        pairList.Add(new KeyValuePair<string, string>(commandArgs[i], commandArgs[i + 1]));
                    }

                    // Skip next arg and advance to beginning of loop
                    i++;
                    continue;
                }
                else if (commandArgs[i] == "--clear" || commandArgs[i] == "-c")
                {
                    pairList.Add(new KeyValuePair<string, string>(commandArgs[i], ""));

                    // Advance to beginning of loop
                    continue;
                }
            }
        }

        // Set message type filter for logger
        private void setFilter(LogMessageType type)
        {
            messageType = type;
        }

        // Validate that the given logger name does not have a file extension
        private static string validateName(string filename)
        {
            if (filename.Contains('.'))
            {
                return Directory.GetCurrentDirectory();
            }

            return filename;
        }

        // Validate that the given path is either absolute, exists, or create it
        private static string validatePath(string path)
        {
            if (!Directory.Exists(path))
            {
                try
                {
                    path = linuxPathToWindowsPath(path);
                    path = validatePathEnding(path);

                    if (!string.IsNullOrEmpty(path))
                    {
                        Directory.CreateDirectory(path);
                    }
                }
                catch (Exception exception)
                {
                    Console.WriteLine("Exception: {0}", exception.Message);
                    return Directory.GetCurrentDirectory();
                }
            }

            return path;
        }

        // Validate that the given path does not end with \
        private static string validatePathEnding(string path)
        {
            if (path.Length > 1 && path[path.Length - 1] == '\\')
            {
                path = path.Substring(0, path.Length - 1);
            }
            else if (path.Length == 1 && path[path.Length - 1] == '\\')
            {
                path = string.Empty;
            }

            return path;
        }

        // ###################################################################
        // Public Methods
        // ###################################################################

        // Flush log file
        public void clear()
        {
            using (StreamWriter writer = new StreamWriter(logFileName));
        }

        // Combine workingDirectory and programName to build log file name
        public string getName()
        {
            return workingDirectory + "\\" + programName + "_log.txt";
        }

        // Write line according to message type filter
        public void log(string line)
        {
            if (messageType == LogMessageType.ALL)
            {
                logAll(line);
            }
            else if (messageType == LogMessageType.INFO)
            {
                logInfo(line);
            }
            else if (messageType == LogMessageType.DEBUG)
            {
                logDebug(line);
            }
            else if (messageType == LogMessageType.WARN)
            {
                logWarn(line);
            }
            else if (messageType == LogMessageType.ERROR)
            {
                logError(line);
            }

        }

        // Write ALL line: special because it doesn't use ALL prefix
        public void logAll(string line)
        {
            line = "\t\t" + DateTime.Now.ToString() + " | " + line;

            using (StreamWriter writer = new StreamWriter(logFileName, true))
            {
                writer.WriteLine(line);
            }
        }

        // Write Info line
        public void logInfo(string line)
        {
            if (messageType == LogMessageType.INFO || messageType == LogMessageType.ALL)
            {
                line = "INFO  | " + DateTime.Now.ToString() + " | " + line;

                using (StreamWriter writer = new StreamWriter(logFileName, true))
                {
                    writer.WriteLine(line);
                }
            }
        }

        // Write Debug line
        public void logDebug(string line)
        {
            if (messageType == LogMessageType.DEBUG || messageType == LogMessageType.ALL)
            {
                line = "DEBUG | " + DateTime.Now.ToString() + " | " + line;

                using (StreamWriter writer = new StreamWriter(logFileName, true))
                {
                    writer.WriteLine(line);
                }
            }
        }

        // Write Warn line
        public void logWarn(string line)
        {
            if (messageType == LogMessageType.WARN || messageType == LogMessageType.ALL)
            {
                line = "WARN  | " + DateTime.Now.ToString() + " | " + line;

                using (StreamWriter writer = new StreamWriter(logFileName, true))
                {
                    writer.WriteLine(line);
                }
            }
        }

        // Write Error line
        public void logError(string line)
        {
            if (messageType == LogMessageType.ERROR || messageType == LogMessageType.ALL)
            {
                line = "ERROR | " + DateTime.Now.ToString() + " | " + line;

                using (StreamWriter writer = new StreamWriter(logFileName, true))
                {
                    writer.WriteLine(line);
                }
            }
        }
    }
}