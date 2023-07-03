using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Block0.Threading.Pipe
{
    public class JobMsg
    {
        public byte SourceJobId { get; set; }
        public byte DestJobId { get; set;}
        public object MethodParam { get; set; }


        //栈信息
        public StackInfo StackInfo { get; set; }
        //public long Timestamp { get; set; }
    }
}
