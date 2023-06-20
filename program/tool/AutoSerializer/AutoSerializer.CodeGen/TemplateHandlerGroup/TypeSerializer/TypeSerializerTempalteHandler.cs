using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace AutoSerializer
{
    internal partial class TypeSerializerTempalteHandler : TemplateHandlerBase
    {
        public TypeSerializerTempalteHandler(Type targetType) : base(targetType)
        {
            GetTemplate<_Type_Serializer>();
        }


        protected override bool Handle1LineEx(string line)
        {
            if(line.Contains("_Type_"))
            {
                var code = line.Replace("_Type_", targetType.Name);
                this.result.Add(code);
                return true;
            }
            else if (line.Contains(typeof(_TypeFullName_).Name))
            {
                var code = line.Replace(typeof(_TypeFullName_).Name, targetType.FullName);
                this.result.Add(code);
                return true;
            }
            else if (line.Contains("_Serialize_Property_"))
            {
                ReadUntilEndregion();
                HandleSerializeProperty();
                return true;
            }
            else if (line.Contains("_Deserialize_Property_"))
            {
                ReadUntilEndregion();
                HandleDeserializeProperty();
                return true;
            }
            return false;
        }


        private void HandleSerializeProperty()
        {
            foreach (var propertyInfo in targetType.GetProperties())
            {
                var type = propertyInfo.PropertyType;
                if (ValueTypeCodeCache.Dict.ContainsKey(type))
                {
                    result.Add("//type.IsValueType");
                    result.AddRange(ValueTypeCodeCache.Dict[type].SerializerCodes($"val.{propertyInfo.Name}"));
                    continue;
                }

                if (TypeHelper.IsPrimitive(type))
                {
                    result.Add("//type.IsPrimitive");
                    result.Add(DoSerializePrimitiveTemplate(type, "val", propertyInfo.Name));
                    continue;
                }

                if (TypeHelper.GenCodeIfIsRef(type))
                {
                    result.Add("//type.IsRefType");
                    result.AddRange(DoSerializeRefTemplate(type, "val", propertyInfo.Name));
                    continue;
                }


            }
        }

        private void HandleDeserializeProperty()
        {
            foreach (var propertyInfo in targetType.GetProperties())
            {
                var type = propertyInfo.PropertyType;
                if (ValueTypeCodeCache.Dict.ContainsKey(type))
                {
                    result.Add("//type.IsValueType");
                    result.AddRange(ValueTypeCodeCache.Dict[type].DeserializerCodes($"val.{propertyInfo.Name}",true));
                    continue;
                }

                if (TypeHelper.GenCodeIfIsRef(type))
                {
                    result.Add("//type.IsRefType");
                    result.AddRange(DoDeserializeRefTemplate(type, "val", propertyInfo.Name));
                    continue;
                }

                if (TypeHelper.IsPrimitive(type))
                {
                    result.Add("//type.IsPrimitive");
                    result.Add(DoDeserializePrimitiveTemplate(type, "val", propertyInfo.Name));
                    continue;
                }

            }
        }


    }
}
