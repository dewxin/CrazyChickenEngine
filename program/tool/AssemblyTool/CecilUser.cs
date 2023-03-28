using MessagePack;
using Mono.Cecil;
using Mono.Collections.Generic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tool.AssemblyMod
{
    public class CecilUser
    {

        public static void Work(string fileName)
        {
            using (var module = ModuleDefinition.ReadModule(fileName, new ReaderParameters { ReadWrite = true }))
            {
                // Modify the assembly
                foreach (TypeDefinition type in module.Types)
                {
                    if (!type.IsPublic)
                        continue;
                    if (!type.IsClass)
                        continue;

                    if (ContainsAttribute(type.CustomAttributes, nameof(MessagePackObjectAttribute)))
                        continue;


                    ModifyType(module, type);

                }

                module.Write(); // Write to the same file that was used to open the file
            }
        }

        public static bool ContainsAttribute(Collection<CustomAttribute> attrCollection, string attributeName)
        {
            foreach (var typeAttr in attrCollection)
            {
                if (typeAttr.AttributeType.FullName.Contains(attributeName))
                    return true;
            }
            return false;
        }

        private static void ModifyType(ModuleDefinition module, TypeDefinition type)
        {
            MethodReference classAttrCtor = module.ImportReference(typeof(MessagePackObjectAttribute).GetConstructor(new Type[] { typeof(bool) }));
            var typeAttr = new CustomAttribute(classAttrCtor);
            var boolTypeRef = module.ImportReference(typeof(bool));
            var attrParamType = new CustomAttributeArgument(boolTypeRef, false);
            typeAttr.ConstructorArguments.Add(attrParamType);
            type.CustomAttributes.Add(typeAttr);

            int keyIndex = 0;

            foreach(var property in type.Properties)
            {
                if (ContainsAttribute(property.CustomAttributes, nameof(KeyAttribute)))
                    continue;

                MethodReference fieldAttrCtor = module.ImportReference(typeof(KeyAttribute).GetConstructor(new Type[] { typeof(int) }));

                var attr = new CustomAttribute(fieldAttrCtor);
                var intTypeRef = module.ImportReference(typeof(int));
                var attrParam = new CustomAttributeArgument(intTypeRef, keyIndex++);
                attr.ConstructorArguments.Add(attrParam);

                property.CustomAttributes.Add(attr);
            }

        }


    }
}
