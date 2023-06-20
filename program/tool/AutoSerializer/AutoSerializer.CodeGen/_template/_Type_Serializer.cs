#region _Generate_Time_
#endregion

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AutoSerializer;

namespace AutoSerializer //namespace会被替换
{
    internal class _Type_Serializer : ISerializer
    {
        #region _Template_
        private void Template(BinaryWriter writer, _TypeFullName_ val)
        {

#if _Serialize_Property_Template_
            writer.Write(_EnumCast_ _VariableHolder_._VariableName_);
#endif

#if _Serialize_Ref_Template_
            var _VariableName_Serializer = SerializerProxy.GetSerializer(typeof(_TypeFullName_));
            _VariableName_Serializer.Serialize(writer, _VariableHolder_._VariableName_);
#endif
        }


        private object Template(BinaryReader reader)
        {
            _TypeFullName_ val = new _TypeFullName_();

#if _Deserialize_Property_Template_
            _VariableHolder_._VariableName_ =_EnumCast_ _Reader_;
#endif

#if _Deserialize_Ref_Template_
            var _VariableName_Serializer = SerializerProxy.GetSerializer(typeof(_TypeFullName_));
            _VariableHolder_._VariableName_ =(_TypeFullName_)_VariableName_Serializer.Deserialize(reader);
#endif
            return val;
        }

        #endregion

        public void Serialize(BinaryWriter writer, object obj)
        {
            _TypeFullName_ val = (_TypeFullName_)obj;
            #region _Serialize_Property_
            #endregion
        }

        public object Deserialize(BinaryReader reader)
        {
            _TypeFullName_ val = new _TypeFullName_();
            #region _Deserialize_Property_
            #endregion
            return val;
        }

    }

}
