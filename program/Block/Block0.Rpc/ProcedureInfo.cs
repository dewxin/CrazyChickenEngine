using Block.Rpc;
using Block.RPC.Task;
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

        public MethodCallTask Invoke(MessageServiceHandler handler, object param)
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

        private Func<MessageServiceHandler, object, MethodCallTask> _FuncParamObjRetObj;
        public Func<MessageServiceHandler, object, MethodCallTask> FuncParamObjRetObj
        {
            get => _FuncParamObjRetObj;
            set
            {
                HasParam = true;
                HasRet = true;
                _FuncParamObjRetObj = value;
            }
        }


        private Func<MessageServiceHandler, MethodCallTask> _FuncParamVoidRetObj;
        public Func<MessageServiceHandler, MethodCallTask> FuncParamVoidRetObj
        {
            get => _FuncParamVoidRetObj;
            set
            {
                HasParam = false;
                HasRet = true;
                _FuncParamVoidRetObj = value;
            }
        }

        private Action<MessageServiceHandler, object> _FuncParamObjRetVoid;
        public Action<MessageServiceHandler, object> FuncParamObjRetVoid
        {
            get => _FuncParamObjRetVoid;
            set
            {
                HasParam = true;
                HasRet = false;
                _FuncParamObjRetVoid = value;
            }
        }


        private Action<MessageServiceHandler> _FuncParamVoidRetVoid;
        public Action<MessageServiceHandler> FuncParamVoidRetVoid
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
