using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace AutoSerializer
{
    public class TypeHelper
    {
        public static string GetReaderCodebyType(Type type)
        {

            if (type == typeof(byte))
                return "reader.ReadByte()";
            else if (type == typeof(short))
                return "reader.ReadInt16()";
            else if (type == typeof(ushort))
                return "reader.ReadUInt16()";
            else if (type == typeof(int))
                return "reader.ReadInt32()";
            else if (type == typeof(uint))
                return "reader.ReadUInt32()";
            else if (type == typeof(long))
                return "reader.ReadInt64()";
            else if (type == typeof(ulong))
                return "reader.ReadUInt64()";
            else if (type == typeof(float))
                return "reader.ReadSingle()";
            else if (type == typeof(double))
                return "reader.ReadDouble()";
            else if (type == typeof(string))
                return "reader.ReadString()";
            else if (type.IsEnum)
                return GetReaderCodebyType(type.GetEnumUnderlyingType());


            throw new NotSupportedException($"{type} is not supported");
        }


        public static bool IsPrimitive(Type type)
        {
            if (type == typeof(byte))
                return true;
            else if (type == typeof(short))
                return true;
            else if (type == typeof(ushort))
                return true;
            else if (type == typeof(int))
                return true;
            else if (type == typeof(uint))
                return true;
            else if (type == typeof(long))
                return true;
            else if (type == typeof(ulong))
                return true;
            else if (type == typeof(float))
                return true;
            else if (type == typeof(double))
                return true;
            else if (type == typeof(string))
                return true;
            else if (type.IsEnum)
                return true;

            return false;

        }

        public static bool GenCodeIfIsRef(Type type)
        {
            bool result =  !IsPrimitive(type) && !type.IsValueType && !type.Equals(typeof(object));
            if(result)
                RefTypeCodeGen.Instance.TryCollectRefTypeCode(type);

            return result;
        }

        public static string GetTypeName(Type type)
        {
            if(!type.IsGenericType || type.GenericTypeArguments.Count() == 0)
                return type.FullName;

            //如果是List<int>, type.name 会返回 List`1
            var genericTypeName = type.Namespace +"."+GenericTypeName(type.Name);
            var genericArgumentTypeName = GetTypeName(type.GenericTypeArguments.Single());


            return genericTypeName.Replace("_Type_", genericArgumentTypeName);
        }

        private static string GenericTypeName(string name)
        {
            if (name.Contains("`1"))
                return name.Replace("`1", "<_Type_>");

            throw new NotSupportedException();
        }


        /// <summary>
        /// 获取生成的参数类的序列化类的名字
        /// </summary>
        /// <param name="paramType"></param>
        /// <returns></returns>
        public static string GetSerializerName(Type paramType)
        {
            //TODO
            if(!paramType.IsGenericType || paramType.GenericTypeArguments.Count() ==0)
                return paramType.Name+"Serializer";


            var genericTypeName = GenericSerializerTypeName(paramType);
            var genericArgumentTypeName = GetSerializerName(paramType.GenericTypeArguments.Single());

            return genericTypeName + genericArgumentTypeName;

        }

        private static string GenericSerializerTypeName(Type type)
        {
            var name = type.Name;
            if(name.Contains("`1"))
                return name.Replace("`1","_");


            //switch (name)
            //{
            //    case "List`1":
            //        return "List_";
            //}

            throw new NotSupportedException(name);
        }

        public static bool IsTransparent(Type type)
        {
            return type.GetCustomAttribute<TransparentAttribute>() != null; 
        }



    }
}
