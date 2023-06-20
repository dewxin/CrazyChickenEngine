using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace AutoSerializer.CodeGen.CodeGen
{
    internal class FuncParamCodeGen
    {
        public static FuncParamCodeGen Instance = new FuncParamCodeGen();

        internal void CollectParamTypeCode(Assembly assembly)
        {
            var typeList = assembly.GetTypes().ToList();
            typeList.RemoveAll(t => !t.IsInterface);
            foreach (var type in typeList)
            {
                TryCollectFuncParamCode(type);
            }
            
        }

        private void TryCollectFuncParamCode(Type type)
        {

            foreach(var methodInfo in type.GetMethods())
            {
                var retType = methodInfo.ReturnParameter.ParameterType;
                RefTypeCodeGen.Instance.TryCollectRefTypeCode(retType);

                var paramInfo = methodInfo.GetParameters().SingleOrDefault();
                if(paramInfo != null)
                {
                    RefTypeCodeGen.Instance.TryCollectRefTypeCode(paramInfo.ParameterType);
                }
            }

        }
    }
}
