using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoSerializer
{
    internal class SerializerCenterTemplateHandler : TemplateHandlerBase
    {
        public SerializerCenterTemplateHandler(Type targetType) : base(targetType)
        {
            GetTemplate<SerializerCenter>();
        }

        protected override bool Handle1LineEx(string line)
        {
            if (line.Contains("_AddSerializer_"))
            {
                var codeLines = ReadUntilEndif();

                foreach (var codeCache in RefTypeCodeCache.Dict.Values)
                {
                    var type = codeCache.Type;
                    foreach (var item in codeLines)
                    {
                        var code = item;
                        code = code.Replace("_TypeSerializerName_", TypeHelper.GetSerializerName(type));
                        code = code.Replace(typeof(_TypeFullName_).Name, TypeHelper.GetTypeName(type));
                        this.result.Add(code);
                    }
                }

                return true;
            }

            return false;
        }

        protected override void HandleTemplate(string line)
        {
            throw new NotImplementedException();
        }
    }
}
