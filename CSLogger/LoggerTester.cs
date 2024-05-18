using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSLogger
{
    internal class LoggerTester
    {
        // ###################################################################
        // Class Static Testing Members
        // ###################################################################

        // Test file name
        private static string filename = "test";

        // ###################################################################
        // Public Test Methods
        // ###################################################################

        public static void TestAllLogStatements(ref Logger log)
        {
            log.logAll("All log");
            log.logInfo("Info log");
            log.logDebug("Debug log");
            log.logWarn("Warn log");
            log.logError("Error log");
        }

        public static void TestClearLog()
        {
            Logger log = new Logger(filename);
            log.clear();
        }

        public static void TestCommandLineLogger(string[] args)
        {
            Logger log = Logger.CreateFilteredLogger(args);
            TestAllLogStatements(ref log);
        }

        public static void TestNonFilteredLogger()
        {
            Logger log = new Logger(filename);
            TestAllLogStatements(ref log);
        }

        public static void TestLogType(LogMessageType type)
        {
            Logger log = Logger.CreateFilteredLogger(filename, type);
            TestAllLogStatements(ref log);
        }

        public static void TestLoggerFiltering()
        {
            TestNonFilteredLogger();
            TestLogType(LogMessageType.ALL);
            TestLogType(LogMessageType.INFO);
            TestLogType(LogMessageType.DEBUG);
            TestLogType(LogMessageType.WARN);
            TestLogType(LogMessageType.ERROR);
        }

        public static void TestLongLine()
        {
            Logger log = new Logger(filename);
            log.log("LONGLONGLONGLONGLONGLONGLONGLONGLONGLONGLONGLONGLONG" +
                "LONGLONGLONGLONGLONGLONGLONGLONGLONGLONGLONGLONGLONG" +
                "LONGLONGLONGLONGLONGLONGLONGLONGLONGLONGLONGLONGLONG" +
                "LONGLONGLONGLONGLONGLONGLONGLONGLONGLONGLONGLONGLONG" +
                "LONGLONGLONGLONGLONGLONGLONGLONGLONGLONGLONGLONGLONG" +
                "LONGLONGLONGLONGLONGLONGLONGLONGLONGLONGLONGLONGLONG" +
                "LONGLONGLONGLONGLONGLONGLONGLONGLONGLONGLONGLONGLONG");
        }

        // ###################################################################
        // Main
        // ###################################################################

        public static int Main(string[] args)
        {
            //TestClearLog();
            //TestLoggerFiltering();
            //TestLongLine();
            //TestCommandLineLogger(args);

            return 0;
        }
    }
}
