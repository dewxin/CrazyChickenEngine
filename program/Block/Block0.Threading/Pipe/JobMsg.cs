using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Block0.Threading.Pipe
{
    public class JobMsg
    {
        internal protected byte SourceJobId { get; set; }
        internal protected byte DestJobId { get; set;}
        public object MethodParam { get; set; }


        //栈信息
        public StackInfo StackInfo { get; set; }
        //public long Timestamp { get; set; }
    }
}
