using Block.RPC;
using Block.RPC.Attr;
using Block.RPC.Task;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace Block.RPC.Emitter
{
    public interface IRpcInvoker
    {
        MethodCallTask<TResponse> SendRequestParamObjRetObj<TRequest, TResponse>(ushort methodID, TRequest request)
            where TResponse : class;

        MethodCallTask<TResponse> SendRequestParamVoidRetObj<TResponse>(ushort methodID)
            where TResponse : class;

        void SendRequestParamObjRetVoid<TRequest>(ushort methodID, TRequest request);

        void SendRequestParamVoidRetVoid(ushort methodID);

        void SendResponse(object ret, ushort taskID);
    }

    public partial class RpcClientEmitter
    {
        private static readonly Dictionary<ushort, RpcMethodInfo> methodId2InfoDict = new Dictionary<ushort, RpcMethodInfo>();

        private static void RegisterMethodInfo(ushort methodId, Type paramType, Type RetType)
        {
            var info = new RpcMethodInfo();
            info.methodId = methodId;
            info.ParamType = paramType;
            info.RetType = RetType;

            methodId2InfoDict.Add(methodId, info);
        }

        public static Type GetMethodRetType(ushort methodId)
        {
            return methodId2InfoDict[methodId].RetType;
        }

    }

    public partial class RpcClientEmitter
    {
        private static readonly string dllName;
        private static ModuleBuilder moduleBuilder = null;
        private static AssemblyBuilder assemblyBuilder = null;

        private static readonly Dictionary<Type, Type> interfaceType2ImplDict = new Dictionary<Type, Type>();
        static RpcClientEmitter()
        {
            var assemblyName = new AssemblyName(nameof(RpcClientEmitter));
            dllName = assemblyName.Name + ".dll";
#if NET6_0_OR_GREATER
            assemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.Run);
#else
            assemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.RunAndSave);
#endif
            moduleBuilder = assemblyBuilder.DefineDynamicModule(dllName);
        }

        /// <summary>
        /// 开销挺大的，需要注意
        /// </summary>
        /// <typeparam name="IService"></typeparam>
        /// <param name="rpcInvoker"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public static IService Resolve<IService>(IRpcInvoker rpcInvoker) where IService : class
        {
            var interfaceType = typeof(IService);
            if (!interfaceType.IsInterface)
                throw new ArgumentException("param needs to be an interface type");


            //TODO 不加lock由于网络会话层是多线程的，会出Bug
            lock(interfaceType2ImplDict)
            {
                interfaceType2ImplDict.TryGetValue(interfaceType, out Type implType);
                if (implType == null)
                {
                    implType = CreateType<IService>(rpcInvoker);
                    interfaceType2ImplDict.Add(interfaceType, implType);
                }
                return (IService)Activator.CreateInstance(implType, rpcInvoker);
            }
        }

        private static Type CreateType<IService>(IRpcInvoker rpcInvoker)
        {
            var interfaceType = typeof(IService);
            var typeName = string.Format("{0}.{1}Proxy", typeof(RpcClientEmitter).FullName, interfaceType.Name);
            var type = moduleBuilder.GetType(typeName);
            if (type != null)
                return type;

            var typeBuilder = moduleBuilder.DefineType(typeName);
            typeBuilder.AddInterfaceImplementation(interfaceType);

            var fieldBuilder = typeBuilder.DefineField("_peerBaseInvoker", rpcInvoker.GetType(), FieldAttributes.Private);

            CreateConstructor(typeBuilder, fieldBuilder);
            CreateMethods(rpcInvoker, interfaceType, typeBuilder, fieldBuilder);

            return typeBuilder.CreateType();
        }

        private static void CreateConstructor(TypeBuilder typeBuilder, FieldBuilder fieldBuilder)
        {
            var ctor = typeBuilder.DefineConstructor(MethodAttributes.Public, CallingConventions.Standard, new Type[] { typeof(object) });
            var il = ctor.GetILGenerator(16);

            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldarg_1);
            il.Emit(OpCodes.Stfld, fieldBuilder);
            il.Emit(OpCodes.Ret);
        }

        private static void CreateMethods(IRpcInvoker rpcInvoker,Type interfaceType, TypeBuilder typeBuilder, FieldBuilder fieldBuilder)
        {
            var serviceAttribute = interfaceType.GetCustomAttribute<RpcServiceAttribute>();
            if(serviceAttribute == null)
                serviceAttribute = RpcEmitterHelper.GetRpcServiceAttribute(interfaceType);
            
            var methodList = RpcEmitterHelper.GetSortedMethods(interfaceType);
            ushort methodID = serviceAttribute.MinMethodID;

            foreach (MethodInfo methodInfo in methodList)
            {
                CreateMethod(rpcInvoker, methodInfo, typeBuilder, fieldBuilder,methodID);
                methodID++;
            }
        }


        /// <summary>
        /// 暂时处理形式如以下格式的RPC调用
        /// Object PlayerXXX(Object arg0) √
        /// Object PlayerXXX()
        /// void PlayerXXX(Object arg0)
        /// void PlayerXXX() √
        /// 这里是客户端调用，对应服务端见
        /// <see cref="RpcTool.CreateMethodMeta(string, object)"/>
        /// </summary>
        /// <param name="methodInfo"></param>
        /// <param name="typeBuilder"></param>
        /// <param name="fieldBuilder"></param>
        /// <param name="methodId"></param>
        /// <returns></returns>
        private static MethodBuilder CreateMethod(IRpcInvoker rpcInvoker, MethodInfo methodInfo, TypeBuilder typeBuilder, FieldBuilder fieldBuilder, ushort methodId)
        {
            var args = methodInfo.GetParameters();
            var methodBuilder = typeBuilder.DefineMethod(methodInfo.Name, MethodAttributes.Public | MethodAttributes.NewSlot | MethodAttributes.Virtual | MethodAttributes.Final | MethodAttributes.HideBySig,
                methodInfo.CallingConvention, methodInfo.ReturnType, args.Select(t => t.ParameterType).ToArray());
            var il = methodBuilder.GetILGenerator(256);

            bool hasParam = args.Count() > 0;
            bool hasRet = methodInfo.ReturnType != typeof(void);


            if(hasRet)
            {
                var retType = methodInfo.ReturnType.GetGenericArguments().Single();

                Type paramType = null;
                if (hasParam)
                    paramType = args.Single().ParameterType;

                RegisterMethodInfo(methodId, paramType, retType);
            }

            //MethodCallTask<Object> PlayerXXX(Object arg0)
            if (hasRet && hasParam)
            {
                var method = rpcInvoker.GetType().GetMethod(nameof(IRpcInvoker.SendRequestParamObjRetObj));

                MethodInfo genericMethod = method.MakeGenericMethod(args.Single().ParameterType, methodInfo.ReturnType.GetGenericArguments().Single());

                il.Emit(OpCodes.Ldarg_0);
                il.Emit(OpCodes.Ldfld, fieldBuilder);
                il.Emit(OpCodes.Ldc_I4, methodId);
                il.Emit(OpCodes.Ldarg_1);
                il.Emit(OpCodes.Callvirt, genericMethod);
                il.Emit(OpCodes.Ret);

                return methodBuilder;
            }
            //MethodCallTask<object> PlayerXXX()
            else if (hasRet && !hasParam)
            {
                var method = rpcInvoker.GetType().GetMethod(nameof(IRpcInvoker.SendRequestParamVoidRetObj));
                MethodInfo genericMethod = method.MakeGenericMethod(methodInfo.ReturnType.GetGenericArguments().Single());

                il.Emit(OpCodes.Ldarg_0);
                il.Emit(OpCodes.Ldfld, fieldBuilder);
                il.Emit(OpCodes.Ldc_I4, methodId);
                il.Emit(OpCodes.Callvirt, genericMethod);
                il.Emit(OpCodes.Ret);

                return methodBuilder;
            }
            //void PlayerXXX(Object arg0)
            else if (!hasRet && hasParam)
            {
                var method = rpcInvoker.GetType().GetMethod(nameof(IRpcInvoker.SendRequestParamObjRetVoid));
                MethodInfo genericMethod = method.MakeGenericMethod(args.Single().ParameterType);

                il.Emit(OpCodes.Ldarg_0);
                il.Emit(OpCodes.Ldfld, fieldBuilder);
                il.Emit(OpCodes.Ldc_I4, methodId);
                il.Emit(OpCodes.Ldarg_1);
                il.Emit(OpCodes.Callvirt, genericMethod);
                il.Emit(OpCodes.Ret);

                return methodBuilder;
            }

            //void PlayerXXX()
            else if (!hasRet && !hasParam)
            {
                MethodInfo sendMethod = rpcInvoker.GetType().GetMethod(nameof(IRpcInvoker.SendRequestParamVoidRetVoid));

                il.Emit(OpCodes.Ldarg_0);
                il.Emit(OpCodes.Ldfld, fieldBuilder);
                il.Emit(OpCodes.Ldc_I4, methodId);
                il.Emit(OpCodes.Callvirt, sendMethod);
                il.Emit(OpCodes.Ret);

                return methodBuilder;
            }

            return null;

        }



        public static void Save()
        {
#if NET462
            assemblyBuilder.Save(dllName);
#endif
        }


    }
}
