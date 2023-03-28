using Block.RPC;
using Block.RPC.Attr;
using Block.RPC.Emitter;
using Block.RPC.Task;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Block.Rpc
{
    public abstract class MessageServiceHandler
    {
        internal RpcServiceAttribute RpcServiceAttribute;

        protected Dictionary<ushort, ProcedureInfo> id2MethodMetaDict = new Dictionary<ushort, ProcedureInfo>();


        public bool CanHandleMethod(ushort methodId)
        {
            return id2MethodMetaDict.ContainsKey(methodId);
        }

        public MethodCallTask HandleMethodCall(ushort methodId, object param)
        {
            //TODO 顺便统计数据
            var method = id2MethodMetaDict[methodId];
            return method.Invoke(this, param) as MethodCallTask;
        }

        public void Init()
        {
            var serviceEntity = RpcServerCodeEmitter.GetRpcServiceEntity(this.GetType());
            (id2MethodMetaDict, RpcServiceAttribute) = (serviceEntity.id2ProcedureInfoDict, serviceEntity.Attribute);
        }

    }
}
