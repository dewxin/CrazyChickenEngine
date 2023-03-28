using Block0.Threading.Pipe;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Block1.LocatableRPC
{

    internal class RpcMsg : JobMsg
    {
        public ushort MethodId { get; set; }
        public ushort MethodCallTaskId { get; set; }

        public byte SourceServiceId { get => SourceJobId; set => SourceJobId = value; }
        public byte DestServiceId { get => DestJobId; set => DestJobId = value; }
        public bool IsMethodCallDoneReply { get; set; } = false;
    }

    internal class RemoteRpcMsg : RpcMsg
    {
        public enum ForwardEnum
        {
            Output = 0, // 往外发
            Input,  //接收进来的
        }
        public ForwardEnum ForwardType { get; set; }
        public IPEndPoint RemoteIPEndPoint { get; set; }

        //用于NetworkJob转发
        public byte RealDestServiceId { get; set; } 
    }
}
