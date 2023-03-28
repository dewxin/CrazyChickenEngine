using Block.Assorted.Logging.ILogImpl;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Block.Assorted.Logging
{

    public class CallerInfo
    {
        public string MemberName { get; set; }
        public string SourceFilePath { get; set; }
        public int LineNumber { get; set; }
    }

    public class LogExtension
    {
        public static LogExtension Instance { get; set; } = new LogExtension();

    }

    public static class Log
    {
        public static ILog LogImpl { get; private set; } = new Log4NetImpl();


        public static void Init(ILog impl)
        {
            LogImpl = impl;
        }

        //[Conditional("LogDebug")]
        public static void Debug(string message, 
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            var data = new CallerInfo
            { MemberName = memberName, SourceFilePath = sourceFilePath, LineNumber = sourceLineNumber };

            LogImpl.Debug(message, data);
        }

        //[Conditional("LogInfo")]
        public static void Info(string message,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            var data = new CallerInfo
            { MemberName = memberName, SourceFilePath = sourceFilePath, LineNumber = sourceLineNumber };


            LogImpl.Info(message, data);
        }

        [Conditional("LogWarn")]
        public static void Warn(string message, 
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            var data = new CallerInfo
            { MemberName = memberName, SourceFilePath = sourceFilePath, LineNumber = sourceLineNumber };


            LogImpl.Warn(message, data);
        }

        [Conditional("LogError")]
        public static void Error(string message,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            var data = new CallerInfo
            { MemberName = memberName, SourceFilePath = sourceFilePath, LineNumber = sourceLineNumber };


            LogImpl.Error(message, data);
        }

        [Conditional("LogFatal")]
        public static void Fatal(string message,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            var data = new CallerInfo
            { MemberName = memberName, SourceFilePath = sourceFilePath, LineNumber = sourceLineNumber };

            LogImpl.Fatal(message, data);
        }


    }
}
