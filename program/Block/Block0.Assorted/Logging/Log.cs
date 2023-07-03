using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Block.Assorted.Logging
{

    public enum LogLevel
    {
        None = 0,
        Debug = 1,
        Info,
        Warn,
        Error,
        Fatal,
    }

    public class CallerInfo
    {
        public string MethodName { get; set; }
        public string SourceFilePath { get; set; }
        public int LineNumber { get; set; }

    }

    public class LogExtension
    {
        public static LogExtension Instance { get; set; } = new LogExtension();

        public void SetLevel(LogLevel level)
        {
            Log.Level = level;
        }

    }

    public static class Log
    {
        public static ILog LogImpl { get; private set; } 

        internal static LogLevel Level { get; set; }


        public static void Init(ILog impl)
        {
            LogImpl = impl;
        }

        public static void Debug(string message, 
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            if(Level >LogLevel.Debug)
                return;

            var data = new CallerInfo
            { MethodName = memberName, SourceFilePath = sourceFilePath, LineNumber = sourceLineNumber };

            LogImpl.Debug(message, data);
        }

        public static void Info(string message,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            if(Level > LogLevel.Info) return;

            var data = new CallerInfo
            { MethodName = memberName, SourceFilePath = sourceFilePath, LineNumber = sourceLineNumber };

            LogImpl.Info(message, data);
        }

        public static void Warn(string message, 
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            if(Level > LogLevel.Warn) return;

            var data = new CallerInfo
            { MethodName = memberName, SourceFilePath = sourceFilePath, LineNumber = sourceLineNumber };

            LogImpl.Warn(message, data);
        }

        public static void Error(string message,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            if(Level > LogLevel.Error) return;

            var data = new CallerInfo
            { MethodName = memberName, SourceFilePath = sourceFilePath, LineNumber = sourceLineNumber };


            LogImpl.Error(message, data);
        }

        public static void Fatal(string message,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            if (Level > LogLevel.Fatal) return;
            var data = new CallerInfo
            { MethodName = memberName, SourceFilePath = sourceFilePath, LineNumber = sourceLineNumber };

            LogImpl.Fatal(message, data);
        }


    }
}
