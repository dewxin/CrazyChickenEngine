using Block.RPC.Attr;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Block.RPC.Emitter
{
    internal class RpcMethodInfo
    {
        public ushort methodId;
        public Type ParamType;
        public Type RetType;
    }

    internal class RpcEmitterHelper
    {

        public static List<MethodInfo> GetSortedMethods(Type interfaceType)
        {
            var methodList = interfaceType.GetMethods().ToList();
            methodList.Sort((m1, m2) => { return m1.Name.CompareTo(m2.Name); });
            return methodList;
        }


        #region Break RpcServiceGroup into RpcService

        private static Dictionary<Type, RpcServiceAttribute> intfType2AttrDict { get; set; } = new Dictionary<Type, RpcServiceAttribute>();

        public static void AddRpcAttr2Dict(Assembly assembly)
        {
            foreach (var type in assembly.GetTypes())
            {
                var attr = type.GetCustomAttribute<RpcServiceGroupAttribute>();
                if (attr == null)
                    continue;

                var pairList = BreakGroupServiceToRpcService(type);
                pairList.ForEach(t => intfType2AttrDict.Add(t.Item1, t.Item2));
            }
        }


        public static RpcServiceAttribute GetRpcServiceAttribute(Type serviceInterfaceType)
        {
            if(!intfType2AttrDict.ContainsKey(serviceInterfaceType))
            {
                AddRpcAttr2Dict(serviceInterfaceType.Assembly);
            }
            
            return intfType2AttrDict[serviceInterfaceType];
        }


        public static List<(Type,RpcServiceAttribute)> BreakGroupServiceToRpcService(Type serviceGroupInterface)
        {
            var assembly = Assembly.GetAssembly(serviceGroupInterface);

            var attr = serviceGroupInterface.GetCustomAttribute<RpcServiceGroupAttribute>();
            Debug.Assert(attr != null);

            var subIntfList = new List<Type>();

            foreach(var type in assembly.GetTypes())
            {
                if (!type.IsInterface)
                    continue;

                if (type.GetInterfaces().Any(t => t.Equals(serviceGroupInterface)))
                    subIntfList.Add(type);
            }

            subIntfList.Sort((t1, t2) => { return t1.Name.CompareTo(t2.Name); });

            var result = new List<(Type, RpcServiceAttribute)>();

            ushort minId = attr.MinMsgId;
            foreach (var subIntf in subIntfList)
            {
                checked
                {
                    ushort maxId = (ushort)(minId + subIntf.GetMethods().Length);
                    result.Add((subIntf, new RpcServiceAttribute(minId, maxId)));
                    Debug.Assert(maxId < attr.MaxMsgId);
                    minId = maxId;
                }
            }

            return result;

        }

        #endregion


    }

}
