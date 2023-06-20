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
    internal partial class TypeSerializerTempalteHandler
    {

        public string DoSerializePrimitiveTemplate(Type type, string varHolderName, string varName)
        {
            var template = SerializePrimitiveTemplate;

            if (type.IsEnum)
                template = template.Replace("_EnumCast_", $"({type.GetEnumUnderlyingType().FullName})");
            else
                template = template.Replace("_EnumCast_", string.Empty);

            if(varHolderName == null || varHolderName == string.Empty)
                template = template.Replace("_VariableHolder_.", string.Empty);
            else
                template = template.Replace("_VariableHolder_", varHolderName);

            template = template.Replace("_VariableName_", varName);

            return template;
        }

        public string DoDeserializePrimitiveTemplate(Type type, string varHolderName, string varName)
        {
            var template = DeserializePrimitiveTemplate;

            if (type.IsEnum)
                template = template.Replace("_EnumCast_", $"({type.FullName})");
            else
                template = template.Replace("_EnumCast_", string.Empty);

            if (varHolderName == null || varHolderName == string.Empty)
                template = template.Replace("_VariableHolder_.", string.Empty);
            else
                template = template.Replace("_VariableHolder_", varHolderName);

            template = template.Replace("_VariableName_", varName);

            template = template.Replace("_Reader_", TypeHelper.GetReaderCodebyType(type));

            return template;
        }

        public List<string> DoSerializeRefTemplate(Type type, string varHolderName, string varName)
        {
            List<string> list = new List<string>();
            foreach(var line in SerializeRefTemplate)
            {
                var template = line;
                template = template.Replace("_Type_", type.Name);
                template = template.Replace(nameof(_TypeFullName_), TypeHelper.GetTypeName(type));

                if (varHolderName == null || varHolderName == string.Empty)
                    template = template.Replace("_VariableHolder_.", string.Empty);
                else
                    template = template.Replace("_VariableHolder_", varHolderName);

                template = template.Replace("_VariableName_", varName);

                list.Add(template);
            }

            return list;
        }

        public List<string> DoDeserializeRefTemplate(Type type, string varHolderName, string varName)
        {
            List<string> list = new List<string>();
            foreach (var line in DeserializeRefTemplate)
            {
                var template = line;
                template = template.Replace(nameof(_TypeFullName_), TypeHelper.GetTypeName(type));
                template = template.Replace("_Type_", type.Name);

                if (varHolderName == null || varHolderName == string.Empty)
                    template = template.Replace("_VariableHolder_.", string.Empty);
                else
                    template = template.Replace("_VariableHolder_", varHolderName);

                template = template.Replace("_VariableName_", varName);

                list.Add(template);
            }

            return list;
        }


    }
}
