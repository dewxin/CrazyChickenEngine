using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Block.Assorted.Logging
{
    /// <summary>
    /// Log interface
    /// </summary>
    public interface ILog
    {
        void Debug(string message, CallerInfo invokerInfo);

        void Info(string message, CallerInfo invokerInfo);

        void Warn(string message, CallerInfo invokerInfo);

        void Error(string message, CallerInfo invokerInfo);

        void Fatal(string message, CallerInfo invokerInfo);

    }
}
