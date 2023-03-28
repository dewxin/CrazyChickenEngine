using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Block.RPC.Attr
{
    [AttributeUsage(AttributeTargets.Interface)]
    public class RpcServiceGroupAttribute : Attribute
    {
        public ushort MinMsgId { get; set; }
        public ushort MaxMsgId { get; set; }

        //msgId 左闭右开
        public RpcServiceGroupAttribute(ushort minMsgId, ushort maxMsgId)
        {
            MinMsgId = minMsgId;
            MaxMsgId = maxMsgId;

            if (MinMsgId >= MaxMsgId)
            {
                throw new ArgumentException($"MinMsgId is greater than MaxMsgId");
            }
        }

    }
}
