using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Block.RPC.Attr
{
    [AttributeUsage(AttributeTargets.Class)]
    public class RpcDataAttribute: Attribute
    {
        public RpcDataOption RpcDataOption { get; private set; }

        public RpcDataAttribute(RpcDataOption rpcDataOption)
        {
            this.RpcDataOption = rpcDataOption;
        }

        public bool IsOptionOn(RpcDataOption rpcDataOption)
        {
            return (RpcDataOption & rpcDataOption) == rpcDataOption;
        }
    }


    [Flags]
    public enum RpcDataOption:ushort
    {
        None = 0,
        UseCompress= 0b_0000_0001
    }

}
