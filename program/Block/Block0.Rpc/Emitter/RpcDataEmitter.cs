//using Block.RPC.Attr;
//using System;
//using System.Collections.Generic;
//using System.Diagnostics;
//using System.Linq;
//using System.Reflection;
//using System.Text;
//using System.Threading.Tasks;

//namespace Block.RPC.Emitter
//{
//    public static class RpcDataEmitter
//    {
//        private static Dictionary<Type, RpcDataAttribute> _RpcDataDict;
//        public static Dictionary<Type, RpcDataAttribute> RpcDataDict
//        {
//            get
//            {
//                if (_RpcDataDict == null)
//                    RpcDataDict = GetRpcDataDict();
//                return _RpcDataDict;
//            }
//            private set { _RpcDataDict = value; }
//        }


//        public static Dictionary<Type, RpcDataAttribute> GetRpcDataDict()
//        {
//            var assemblyList = RpcEmitterHelper.GetIServiceAssemblyList();

//            Dictionary<Type, RpcDataAttribute> rpcDataDict = new Dictionary<Type, RpcDataAttribute>();
//            foreach(var assembly in assemblyList)
//            {

//                var classType = assembly.GetTypes().Where(type => type.IsClass);
//                foreach (var type in classType)
//                {
//                    var rpcDataAttr = type.GetCustomAttribute<RpcDataAttribute>();
//                    if (rpcDataAttr != null)
//                        rpcDataDict.Add(type, rpcDataAttr);
//                }
//            }

//            return rpcDataDict;
//        }

//        public static bool IsAgressiveCompress(this Type type)
//        {
//            //TODO 这里好像MessagePack有bug
//            //if (RpcDataDict == null)
//                return false;

//            var containType = RpcDataDict.TryGetValue(type, out var val);
//            if (!containType)
//                return false;

//            return val.IsOptionOn(RpcDataOption.UseCompress);
//        }

//    }

//}
