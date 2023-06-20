using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace AutoSerializer
{
    internal class ValueTypeCodeGen
    {
        public static ValueTypeCodeGen Instance = new ValueTypeCodeGen();

        public const string _TypeVariableName_ = "_TypeVariableName_";
        public const string _VariableEndMark_ = "_VariableEndMark_";


        private HashSet<Type> initAsmTypeSet = new HashSet<Type>();

        internal void CollectValueTypeCode(Assembly assembly)
        {
            var typeList = assembly.GetTypes().ToList();
            typeList.RemoveAll(t => !t.IsPublic);
            typeList.RemoveAll(t => t.IsInterface || t.IsEnum || !t.IsValueType);
            typeList.ForEach(t=> initAsmTypeSet.Add(t));

            foreach (var type in typeList)
            {
                TryCollectValueTypeCode(type);

            }
        }

        private void TryCollectValueTypeCode(Type type)
        {
            if (!type.IsValueType)
                return;
            if (ValueTypeCodeCache.Dict.ContainsKey(type))
                return;

            TryCollectPropertyTypeCode(type);

            ValueTypeCode codeCache = new ValueTypeCode();
            codeCache.Type = type;
            codeCache.SerializerCodeLines = GetSerializerCode(type);
            codeCache.DeserializerCodeLines = GetDeserializerCode(type);

            ValueTypeCodeCache.Dict.Add(type, codeCache);
        }

        private void TryCollectPropertyTypeCode(Type type)
        {
            foreach (var property in type.GetProperties())
            {
                var propertyType = property.PropertyType;
                if (!initAsmTypeSet.Contains(propertyType))
                    continue ;

                TryCollectValueTypeCode(propertyType);
            }

        }



        private List<string> GetSerializerCode(Type type)
        {
            List<string> codeLine = new List<string>();

            foreach (var property in type.GetProperties())
            {
                var propertyType = property.PropertyType;

                if (ValueTypeCodeCache.Dict.TryGetValue(propertyType, out var cache))
                {
                    codeLine.AddRange(cache.SerializerCodes($"{_TypeVariableName_}.{property.Name}"));
                    continue;
                }


                if (propertyType.IsEnum)
                {
                    var enumUnderlyingType = Enum.GetUnderlyingType(propertyType);
                    string code = $"writer.Write(({enumUnderlyingType.Name}){_TypeVariableName_}.{property.Name});";
                    codeLine.Add(code);
                    continue;
                }


                if(TypeHelper.IsPrimitive(propertyType))
                {
                    var code = $"writer.Write({_TypeVariableName_}.{property.Name});";
                    codeLine.Add(code);
                    continue;
                }

                if(TypeHelper.GenCodeIfIsRef(propertyType))
                {
                    var code = $"AutoSerializer.GetSerializer(typeof({propertyType.Name})).Serialize(writer, {_TypeVariableName_}.{property.Name})";

                    codeLine.Add(code);
                    continue;
                }

            }

            return codeLine;
        }

        private List<string> GetDeserializerCode(Type type)
        {

            List<string> codeLine = new List<string>();

            codeLine.Add($"{_TypeVariableName_} = new {type.FullName}()" + "{");
            foreach (var property in type.GetProperties())
            {
                var propertyType = property.PropertyType;
                if (ValueTypeCodeCache.Dict.TryGetValue(propertyType, out var cache))
                {
                    codeLine.AddRange(cache.DeserializerCodes(property.Name, false));
                    continue;
                }

                if (propertyType.IsEnum)
                {
                    var enumUnderlyingType = Enum.GetUnderlyingType(propertyType);
                    var readerCode = TypeHelper.GetReaderCodebyType(enumUnderlyingType);
                    string code = $"{property.Name} = ({propertyType.FullName}){readerCode},";

                    codeLine.Add(code);
                    continue;
                }
                if (TypeHelper.IsPrimitive(propertyType))
                {
                    string code = $"{property.Name} = {TypeHelper.GetReaderCodebyType(propertyType)},";

                    codeLine.Add(code);
                    continue;
                }

                if (TypeHelper.GenCodeIfIsRef(propertyType))
                {
                    var code = $"{property.Name} =({propertyType.Name})AutoSerializer.GetSerializer(typeof({propertyType.Name})).Deserialize(reader),";

                    codeLine.Add(code);
                    continue;
                }

            }

            codeLine.Add("}"+nameof(_VariableEndMark_));


            return codeLine;
        }

    }

}
