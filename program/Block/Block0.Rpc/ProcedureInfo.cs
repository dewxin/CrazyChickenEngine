using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Block.RPC
{
    public class ProcedureInfo
    {
        public ushort Id { get; set; }
        public Type ParamType { get; set; }
        public string MethodName { get; set; }
        public bool HasRet { get; set; }
        public bool HasParam { get; set; }

        public object Invoke(object handler, object param)
        {
            if (HasParam && HasRet)
            {
                return FuncParamObjRetObj.Invoke(handler, param);
            }
            else if (!HasRet && !HasParam)
            {
                FuncParamVoidRetVoid.Invoke(handler);
            }
            else if (HasRet && !HasParam)
            {
                return FuncParamVoidRetObj.Invoke(handler);
            }
            else if (!HasRet && HasParam)
            {
                FuncParamObjRetVoid.Invoke(handler, param);
            }

            return null;
        }

        private Func<object, object, object> _FuncParamObjRetObj;
        public Func<object, object, object> FuncParamObjRetObj
        {
            get => _FuncParamObjRetObj;
            set
            {
                HasParam = true;
                HasRet = true;
                _FuncParamObjRetObj = value;
            }
        }


        private Func<object, object> _FuncParamVoidRetObj;
        public Func<object, object> FuncParamVoidRetObj
        {
            get => _FuncParamVoidRetObj;
            set
            {
                HasParam = false;
                HasRet = true;
                _FuncParamVoidRetObj = value;
            }
        }

        private Action<object, object> _FuncParamObjRetVoid;
        public Action<object, object> FuncParamObjRetVoid
        {
            get => _FuncParamObjRetVoid;
            set
            {
                HasParam = true;
                HasRet = false;
                _FuncParamObjRetVoid = value;
            }
        }


        private Action<object> _FuncParamVoidRetVoid;
        public Action<object> FuncParamVoidRetVoid
        {
            get => _FuncParamVoidRetVoid;
            set
            {
                HasParam = false;
                HasRet = false;
                _FuncParamVoidRetVoid = value;
            }
        }

    }

}
