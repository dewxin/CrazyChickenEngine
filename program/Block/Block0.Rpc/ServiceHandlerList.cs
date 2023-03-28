using Block.RPC.Attr;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Block.Rpc
{
    public class ServiceHandlerList
    {
        protected List<MessageServiceHandler> serviceHandlerList = new List<MessageServiceHandler>();

        public ServiceHandlerList()
        { }

        public void AddHandlerRange(IEnumerable<MessageServiceHandler> serviceHandlerCollection)
        {
            foreach (var serviceHandler in serviceHandlerCollection)
            {
                serviceHandler.Init();
                serviceHandlerList.Add(serviceHandler);
            }

            CheckMethodIdConflict(serviceHandlerCollection);

        }


        private static void CheckMethodIdConflict(IEnumerable<MessageServiceHandler> serviceHandlerCollection)
        {
            var rpcAttr2HandlerDict = new Dictionary<RpcServiceAttribute, MessageServiceHandler>();

            foreach (var serviceHandler in serviceHandlerCollection)
            {
                var rpcAttr = serviceHandler.RpcServiceAttribute;

                foreach (var ele in rpcAttr2HandlerDict)
                {
                    if (ele.Key.Conflict(rpcAttr))
                    {
                        var exceptionStr = $"{ele.Value.GetType().Name}[{ele.Key.MinMethodID}-{ele.Key.MaxMethodID}] conflict with {serviceHandler.GetType().Name}[{rpcAttr.MinMethodID}-{rpcAttr.MaxMethodID}]";
                        throw new ArgumentException(exceptionStr);
                    }
                }
                rpcAttr2HandlerDict.Add(rpcAttr, serviceHandler);
                //Log.Debug($"{nameof(ServiceHandlerList)} id:{rpcAttr.MinMsgId}-{rpcAttr.MaxMsgId} type:{serviceHandler.GetType().Name}");
            }

        }


        /// 查找handler
        public MessageServiceHandler FindHandler(ushort methodId)
        {
            foreach (var handler in serviceHandlerList)
            {
                if (handler.CanHandleMethod(methodId))
                    return handler;
            }

            return null;
        }


    }
}
