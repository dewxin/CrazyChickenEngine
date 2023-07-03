using Block.Assorted.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Client.Simulator.Logging
{
    internal class ConsoleImpl : ILog
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private string ToStr(string message, CallerInfo callerInfo)
        {
            var time = DateTime.Now.ToString("HH:mm:ss");
            var fileName = callerInfo.SourceFilePath.Split('\\', '/').Last();
            return $"{time} {fileName}.{callerInfo.MethodName}: {message}";
        }

        public void Debug(string message, CallerInfo invokerInfo)
        {
            Console.WriteLine(ToStr(message,invokerInfo));
        }

        public void Error(string message, CallerInfo invokerInfo)
        {
            Console.WriteLine(ToStr(message,invokerInfo));
        }

        public void Fatal(string message, CallerInfo invokerInfo)
        {
            Console.WriteLine(ToStr(message,invokerInfo));
        }

        public void Info(string message, CallerInfo invokerInfo)
        {
            Console.WriteLine(ToStr(message,invokerInfo));
        }

        public void Warn(string message, CallerInfo invokerInfo)
        {
            Console.WriteLine(ToStr(message,invokerInfo));
        }
    }
}
