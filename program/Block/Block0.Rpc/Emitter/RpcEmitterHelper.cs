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

        public static Assembly GetIServiceAssembly()
        {
            var assemblyList = AppDomain.CurrentDomain.GetAssemblies();

            foreach (var assembly in assemblyList)
            {
                var titleAttr = assembly.GetCustomAttributes<AssemblyMetadataAttribute>();
                foreach(var attr in titleAttr)
                {
                    if (attr.Key.Equals("NagiAsmType") && attr.Value.Equals("IService"))
                        return assembly;
                }
            }

            return null;
        }

        #region Break RpcServiceGroup into RpcService

        private static Dictionary<Type, RpcServiceAttribute> _IntfType2AttrDict;
        public static Dictionary<Type, RpcServiceAttribute> IntfType2AttrDict
        {
            get
            {
                if (_IntfType2AttrDict == null)
                    _IntfType2AttrDict = GetRpcServiceAttrDict();
                return _IntfType2AttrDict;
            }
            private set { _IntfType2AttrDict = value; }
        }

        public static Dictionary<Type, RpcServiceAttribute> GetRpcServiceAttrDict()
        {
            Dictionary<Type, RpcServiceAttribute> dict = new Dictionary<Type, RpcServiceAttribute>();
            var assembly = GetIServiceAssembly();
            foreach(var type in assembly.GetTypes())
            {
                var attr = type.GetCustomAttribute<RpcServiceGroupAttribute>();
                if (attr == null)
                    continue;

                var pairList = BreakGroupServiceToRpcService(type);
                pairList.ForEach(t => dict.Add(t.Item1, t.Item2));
            }

            return dict;
        }

        public static RpcServiceAttribute GetRpcServiceAttribute(Type serviceInterfaceType)
        {
            if(!IntfType2AttrDict.ContainsKey(serviceInterfaceType))
                return null;
            
            return IntfType2AttrDict[serviceInterfaceType];
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
