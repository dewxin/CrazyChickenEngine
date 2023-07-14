using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace AutoSerializer
{
    internal class RefTypeCodeGen
    {
        public static RefTypeCodeGen Instance = new RefTypeCodeGen();

        private HashSet<Type> initAsmTypeSet = new HashSet<Type>();

        internal void CollectRefTypeCode(Assembly assembly)
        {
            var typeList = assembly.GetTypes().ToList();
            typeList.RemoveAll(t => t.IsInterface || t.IsEnum || t.IsValueType);
            typeList.RemoveAll(t => !t.IsPublic);
            typeList.RemoveAll(t => t.GetCustomAttribute<IgnoreAttribute>() != null);
            typeList.ForEach(t => initAsmTypeSet.Add(t));

            foreach (var type in typeList)
            {
                TryCollectRefTypeCode(type);
            }
        }


        public void TryCollectRefTypeCode(Type type)
        {
            if (type.IsValueType)
                return;
            if (TypeHelper.Is<IgnoreAttribute>(type))
                return;
            if (TypeHelper.IsPrimitive(type))
                return;
            if (RefTypeCodeCache.Dict.ContainsKey(type))
                return;

            TryCollectPropertyTypeCode(type);


            if(!TypeHelper.Is<TransparentAttribute>(type))
            {
                RefTypeCode codeCache = new RefTypeCode();
                codeCache.Type = type;
                codeCache.SerializerName = TypeHelper.GetSerializerName(type);
                codeCache.CodeLines = GetTypeCode(type);

                RefTypeCodeCache.Dict.Add(type, codeCache);
            }

        }


        private void TryCollectPropertyTypeCode(Type type)
        {
            foreach (var property in type.GetProperties())
            {
                var propertyType = property.PropertyType;

                if (!initAsmTypeSet.Contains(propertyType))
                    continue;

                TryCollectRefTypeCode(propertyType);
            }

        }

        private List<string> GetTypeCode(Type type)
        {
            //if it's List
            if(type.IsGenericType) {
                using (var typeHandler = new ListSerializerTempalteHandler(type))
                {
                    return typeHandler.Parse();
                }
            }

            using (var typeHandler = new TypeSerializerTempalteHandler(type))
            {
                return typeHandler.Parse();
            }

        }


        public void GetSerializerCenterCode()
        {
            using(var handler = new SerializerCenterTemplateHandler(null))
            {
                RefTypeCode codeCache = new RefTypeCode();
                codeCache.CodeLines = handler.Parse();
                RefTypeCodeCache.SerializerCenter = codeCache;
            }

        }

    }

}
