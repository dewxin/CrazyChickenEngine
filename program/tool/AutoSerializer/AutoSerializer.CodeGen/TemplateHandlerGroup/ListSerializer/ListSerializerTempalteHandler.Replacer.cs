using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace AutoSerializer
{
    internal partial class ListSerializerTempalteHandler
    {

        public string DoSerializePrimitiveTemplate(Type type, string varName)
        {
            var template = SerializePrimitiveTemplate;

            if (type.IsEnum)
                template = template.Replace("_EnumCast_", $"({type.GetEnumUnderlyingType().FullName})");
            else
                template = template.Replace("_EnumCast_", string.Empty);

            template = template.Replace("_VariableName_", varName);

            return template;
        }

        public string DoDeserializePrimitiveTemplate(Type type, string varName)
        {
            var template = DeserializePrimitiveTemplate;

            if (type.IsEnum)
                template = template.Replace("_EnumCast_", $"({type.FullName})");
            else
                template = template.Replace("_EnumCast_", string.Empty);

            template = template.Replace("_VariableName_", varName);

            template = template.Replace("_Reader_", TypeHelper.GetReaderCodebyType(type));

            return template;
        }

        public List<string> DoSerializeRefTemplate(Type type, string varName)
        {
            List<string> list = new List<string>();
            foreach(var line in SerializeRefTemplate)
            {
                var template = line;
                template = template.Replace("_Type_", type.Name);
                template = template.Replace(nameof(_TypeFullName_), TypeHelper.GetTypeName(type));

                template = template.Replace("_VariableName_", varName);

                list.Add(template);
            }

            return list;
        }

        public List<string> DoDeserializeRefTemplate(Type type, string varName)
        {
            List<string> list = new List<string>();
            foreach (var line in DeserializeRefTemplate)
            {
                var template = line;
                template = template.Replace(nameof(_TypeFullName_), TypeHelper.GetTypeName(type));
                template = template.Replace("_Type_", type.Name);

                template = template.Replace("_VariableName_", varName);

                list.Add(template);
            }

            return list;
        }

        public List<string> DoSerializeListTemplate(Type elementType)
        {
            List<string> list = new List<string>();

            for(int i = 0; i< SerializeListTemplate.Count; ++i)
            {
                var template = SerializeListTemplate[i];
                if(template.Contains("_Serialize_List_Element_"))
                {
                    ++i; // skip endif line
                    var listCode = HandleSerializeListElement(elementType);
                    list.AddRange(listCode);
                    continue;
                }
                var code = template.Replace(nameof(_TypeFullName_._List_Property_Name_), elementType.Name);
                list.Add(code);
            }          
            return list;
        }

        public List<string> DoDeserializeListTemplate(Type listType)
        {
            List<string> list = new List<string>();

            var elementType = listType.GenericTypeArguments.Single();
            for (int i = 0; i < DeserializeListTemplate.Count; ++i)
            {
                var template = DeserializeListTemplate[i];
                if (template.Contains("_Deserialize_List_Element_"))
                {
                    ++i; // skip endif line
                    var listCode = HandleDeserializeListElement(elementType);
                    list.AddRange(listCode);
                    continue;
                }
                template = template.Replace(nameof(_TypeFullName_._List_Property_Name_), elementType.Name);
                template = template.Replace("_ListType_", TypeHelper.GetTypeName(listType));

                template = template.Replace(nameof(_TypeFullName_), TypeHelper.GetTypeName(elementType));
                list.Add(template);
            }
            return list;
        }


        private List<string> HandleSerializeListElement(Type type)
        {
            var list = new List<string>();

            if (ValueTypeCodeCache.Dict.ContainsKey(type))
            {
                list.Add("//listType.IsValueType");
                list.AddRange(ValueTypeCodeCache.Dict[type].SerializerCodes("element"));
                return list;
            }

            if (TypeHelper.GenCodeIfIsRef(type))
            {
                list.Add("//listType.IsListRefType");
                list.AddRange(DoSerializeRefTemplate(type, "element"));
                return list;
            }

            if(TypeHelper.IsPrimitive(type))
            {
                list.Add("//listType.default");
                list.Add(DoSerializePrimitiveTemplate(type, "element"));
                return list;
            }

            throw new NotSupportedException(type.Name);
        }


        private List<string> HandleDeserializeListElement(Type type)
        {
            var list = new List<string>();

            if (ValueTypeCodeCache.Dict.ContainsKey(type))
            {
                list.Add("//listType.IsValueType");
                list.AddRange(ValueTypeCodeCache.Dict[type].DeserializerCodes("element",true));
                return list;
            }

            if (TypeHelper.GenCodeIfIsRef(type))
            {
                list.Add("//listType.IsListRefType");
                list.AddRange(DoDeserializeRefTemplate(type, "element"));
                return list;
            }

            if (TypeHelper.IsPrimitive(type))
            {
                list.Add("//listType.default");
                list.Add(DoDeserializePrimitiveTemplate(type, "element"));
                return list;
            }

            throw new NotSupportedException(type.Name);
        }

    }
}
