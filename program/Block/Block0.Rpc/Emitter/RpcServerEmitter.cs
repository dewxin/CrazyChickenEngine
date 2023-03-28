using Block.RPC;
using Block.RPC.Attr;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace Block.RPC.Emitter
{
    public class RpcServiceEntity
    {
        public Dictionary<ushort, ProcedureInfo> id2ProcedureInfoDict = new Dictionary<ushort, ProcedureInfo>();
        public RpcServiceAttribute Attribute { get; set; }

        public void Add(ProcedureInfo procedureInfo)
        {
            id2ProcedureInfoDict.Add(procedureInfo.Id, procedureInfo);
        }
    }

    public static partial class RpcServerCodeEmitter
    {
        private static readonly Dictionary<ushort, RpcMethodInfo> methodId2InfoDict = new Dictionary<ushort, RpcMethodInfo>();

        private static void RegisterMethodInfo(ushort methodId, Type paramType)
        {
            var info = new RpcMethodInfo();
            info.methodId = methodId;
            info.ParamType = paramType;

            methodId2InfoDict.Add(methodId, info);
        }

        public static Type GetMethodParamType(ushort methodId)
        {
            return methodId2InfoDict[methodId].ParamType;
        }
    }

    public static partial class RpcServerCodeEmitter
    {
        private static Dictionary<Type, RpcServiceEntity> interface2EntityDict = new Dictionary<Type, RpcServiceEntity>();



        public static RpcServiceEntity GetRpcServiceEntity(Type servieImplType)
        {
            var serviceInterfaceType = GetServiceInterfaceDirect(servieImplType);
            if (serviceInterfaceType == null)
            {
                serviceInterfaceType = GetServiceInterfaceGroup(servieImplType);
            }
            Debug.Assert(serviceInterfaceType != null);
            Debug.Assert(serviceInterfaceType.IsInterface);

            lock(interface2EntityDict)
            {
                //多线程访问会冲突
                if (!interface2EntityDict.ContainsKey(serviceInterfaceType))
                {
                    var attr = serviceInterfaceType.GetCustomAttribute<RpcServiceAttribute>();
                    if (attr == null)
                        attr = RpcEmitterHelper.GetRpcServiceAttribute(serviceInterfaceType);

                    Debug.Assert(attr != null);

                    ParseServiceDirect(serviceInterfaceType, servieImplType, attr);
                }

                return interface2EntityDict[serviceInterfaceType];
            }

        }


        public static void ParseServiceDirect(Type interfaceType, Type typeImplServiceIntf, RpcServiceAttribute serviceAttr)
        {
            var id2FuncDict = new Dictionary<ushort, ProcedureInfo>();
            var methodList = RpcEmitterHelper.GetSortedMethods(interfaceType);
            ushort i = serviceAttr.MinMethodID;
            foreach (var method in methodList)
            {
                var methodMeta = CreateMethodMeta(method.Name, typeImplServiceIntf);
                id2FuncDict.Add(i, methodMeta);

                RegisterMethodInfo(i, methodMeta.ParamType);

                i++;
                Debug.Assert(i <= serviceAttr.MaxMethodID);
            }

            var serviceEntity = new RpcServiceEntity { Attribute = serviceAttr, id2ProcedureInfoDict = id2FuncDict };

            interface2EntityDict.Add(interfaceType, serviceEntity);
        }



        #region service interface type
        public static Type GetServiceInterfaceDirect(Type serviceImplType)
        {
            Type[] interfaceTypeArray = serviceImplType.GetInterfaces();

            foreach (var interfaceType in interfaceTypeArray)
            {
                if (interfaceType.GetCustomAttribute<RpcServiceAttribute>() != null)
                    return interfaceType;
            }

            return null;
        }

        public static Type GetServiceInterfaceGroup(Type serviceHandlerType)
        {
            Type[] interfaceTypeArray = serviceHandlerType.GetInterfaces();

            foreach (var interfaceType in interfaceTypeArray)
            {
                Type[] parentInterfaceArray = interfaceType.GetInterfaces();
                foreach (var parentInterfaceType in parentInterfaceArray)
                {
                    if (parentInterfaceType.GetCustomAttribute<RpcServiceGroupAttribute>() != null)
                        return interfaceType;
                }
            }

            return null;
        }
        #endregion


        /// <summary>
        /// 暂时处理形式如以下格式的RPC调用
        /// Object PlayerXXX(Object arg0) √
        /// Object PlayerXXX()
        /// void PlayerXXX(Object arg0) √
        /// void PlayerXXX() √
        /// 这里是服务端handler，客户端见
        /// <see cref="RpcClientEmitter.CreateMethod"/>
        /// </summary>
        /// <param name="methodName"></param>
        /// <param name="target"></param>
        /// <returns></returns>
        private static ProcedureInfo CreateMethodMeta(string methodName, Type serviceHandlerType)
        {
            var method = serviceHandlerType.GetMethod(methodName);

            bool hasRet = !(method.ReturnType == typeof(void));
            bool hasParam = method.GetParameters().Length > 0;

            // Object PlayerXXX(Object arg0) 
            if (hasRet && hasParam)
            {
                Type[] dynamicArgs = { typeof(object), typeof(object) };
                DynamicMethod dynamicMethod = new DynamicMethod("Nagi_Rpc_" + method.Name, typeof(object), dynamicArgs, typeof(RpcServerCodeEmitter).Module);

                ILGenerator il = dynamicMethod.GetILGenerator(64);
                il.Emit(OpCodes.Ldarg_0);
                il.Emit(OpCodes.Ldarg_1);
                il.EmitCall(OpCodes.Call, method, null);
                il.Emit(OpCodes.Ret);


                Func<object, object, object> methodFunc =
                (Func<object, object, object>)dynamicMethod.CreateDelegate(typeof(Func<object, object, object>));

                var paramType = method.GetParameters().Single().ParameterType;
                var retType = method.ReturnType;

                //不支持值类型
                if (paramType.IsValueType || retType.IsValueType)
                    throw new ArgumentException("不能是值类型");


                return new ProcedureInfo()
                {
                    FuncParamObjRetObj = methodFunc,
                    ParamType = paramType,
                    MethodName = methodName,
                };
            }
            //object PlayerXXX()
            else if (hasRet && !hasParam)
            {
                Type[] dynamicArgs = { typeof(object) };
                DynamicMethod dynamicMethod = new DynamicMethod("Nagi_Rpc_" + method.Name, typeof(object), dynamicArgs, typeof(RpcServerCodeEmitter).Module);

                ILGenerator il = dynamicMethod.GetILGenerator(64);
                il.Emit(OpCodes.Ldarg_0);
                il.EmitCall(OpCodes.Call, method, null);
                il.Emit(OpCodes.Ret);

                Func<object, object> methodFunc =
                (Func<object, object>)dynamicMethod.CreateDelegate(typeof(Func<object, object>));

                var retType = method.ReturnType;

                if (retType.IsValueType)
                    throw new ArgumentException("不能是值类型");

                return new ProcedureInfo()
                {
                    FuncParamVoidRetObj = methodFunc,
                    ParamType = typeof(void),
                    MethodName = methodName,
                };
            }
            //void PlayerXXX(object)
            else if (!hasRet && hasParam)
            {
                Type[] dynamicArgs = { typeof(object), typeof(object) };
                DynamicMethod dynamicMethod = new DynamicMethod("Nagi_Rpc_" + method.Name, typeof(void), dynamicArgs, typeof(RpcServerCodeEmitter).Module);

                ILGenerator il = dynamicMethod.GetILGenerator(64);
                il.Emit(OpCodes.Ldarg_0);
                il.Emit(OpCodes.Ldarg_1);
                il.EmitCall(OpCodes.Call, method, null);
                il.Emit(OpCodes.Ret);

                Action<object, object> methodFunc =
                (Action<object, object>)dynamicMethod.CreateDelegate(typeof(Action<object, object>));

                var paramType = method.GetParameters().Single().ParameterType;

                if (paramType.IsValueType)
                    throw new ArgumentException("不能是值类型");

                return new ProcedureInfo()
                {
                    FuncParamObjRetVoid = methodFunc,
                    ParamType = paramType,
                    MethodName = methodName,
                };
            }
            //void PlayerXXX()
            else if (!hasRet && !hasParam)
            {
                Type[] dynamicArgs = { typeof(object) };
                DynamicMethod dynamicMethod = new DynamicMethod("Nagi_Rpc_" + method.Name, typeof(void), dynamicArgs, typeof(RpcServerCodeEmitter).Module);

                ILGenerator il = dynamicMethod.GetILGenerator(64);
                il.Emit(OpCodes.Ldarg_0);
                il.EmitCall(OpCodes.Call, method, null);
                il.Emit(OpCodes.Ret);

                Action<object> methodFunc =
                (Action<object>)dynamicMethod.CreateDelegate(typeof(Action<object>));


                return new ProcedureInfo()
                {
                    FuncParamVoidRetVoid = methodFunc,
                    ParamType = typeof(void),
                    MethodName = methodName,
                };
            }

            return null;

        }

    }

}
