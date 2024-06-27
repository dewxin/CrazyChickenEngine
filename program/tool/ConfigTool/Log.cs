using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConfigTool
{
    public static class Debug
    {
        public static Action<string> Log = delegate{};
        public static Action<string> LogInfo = delegate{};
        public static Action<string> LogWarning = delegate{};
        public static Action<string> LogError = delegate{};
    }
}
