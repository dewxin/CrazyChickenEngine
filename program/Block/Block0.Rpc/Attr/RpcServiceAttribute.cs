using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Block.RPC.Attr
{
    [AttributeUsage(AttributeTargets.Interface)]
    public class RpcServiceAttribute : Attribute
    {
        public ushort MinMethodID { get; set; }
        public ushort MaxMethodID { get; set; }

        public RpcServiceAttribute()
        {
            MinMethodID = 1;
            MaxMethodID = ushort.MaxValue;
        }

        //msgId 左闭右开
        public RpcServiceAttribute(ushort minMethodID, ushort maxMethodID)
        {
            MinMethodID = minMethodID;
            MaxMethodID = maxMethodID;

            if(MinMethodID > MaxMethodID)
            {
                throw new ArgumentException($"MinMsgId is greater than MaxMsgId");
            }
        }

        public bool Conflict(RpcServiceAttribute another)
        {
            // {me} {another}
            if (MaxMethodID <= another.MinMethodID)
                return false;

            // {another} {me}
            if (MinMethodID >= another.MaxMethodID)
                return false;

            return true;
        }

        public bool Include(ushort msgId)
        {
            return MinMethodID <= msgId && msgId < MaxMethodID;
        }
    }

}
