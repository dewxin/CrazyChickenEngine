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
    internal partial class ListSerializerTempalteHandler : TemplateHandlerBase
    {
        public ListSerializerTempalteHandler(Type targetType) : base(targetType)
        {
            GetTemplate<_ListSerializer_>();
        }


        protected override bool Handle1LineEx(string line)
        {
            if(line.Contains("_ListSerializer_"))
            {
                var code = line.Replace("_ListSerializer_", TypeHelper.GetSerializerName(targetType));
                this.result.Add(code);
                return true;
            }
            else if(line.Contains("_Type_"))
            {
                var code = line.Replace("_Type_", targetType.Name);
                this.result.Add(code);
                return true;
            }
            else if (line.Contains(typeof(_TypeFullName_).Name))
            {
                var code = line.Replace(typeof(_TypeFullName_).Name, TypeHelper.GetTypeName(targetType));
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
            var type = targetType.GenericTypeArguments.Single();

            var codeList = DoSerializeListTemplate(type);
            this.result.AddRange(codeList);

        }

        private void HandleDeserializeProperty()
        {
            var codeList = DoDeserializeListTemplate(targetType);
            this.result.AddRange(codeList);
        }


    }
}
