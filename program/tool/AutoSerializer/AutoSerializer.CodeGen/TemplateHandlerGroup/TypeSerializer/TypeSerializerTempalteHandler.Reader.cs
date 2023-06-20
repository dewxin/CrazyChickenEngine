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
    internal partial class TypeSerializerTempalteHandler 
    {

        public const string SerializePrimitiveTemplateName = "_Serialize_Property_Template_";
        public const string SerializeRefTemplteName = "_Serialize_Ref_Template_";

        public const string DeserializePrimitiveTemplateName = "_Deserialize_Property_Template_";
        public const string DeserializeRefTemplteName = "_Deserialize_Ref_Template_";

        public string SerializePrimitiveTemplate => name2TemplateDict[SerializePrimitiveTemplateName].Single();
        public List<string> SerializeRefTemplate => name2TemplateDict[SerializeRefTemplteName];

        public string DeserializePrimitiveTemplate => name2TemplateDict[DeserializePrimitiveTemplateName].Single();
        public List<string> DeserializeRefTemplate => name2TemplateDict[DeserializeRefTemplteName];


        protected override void HandleTemplate(string line)
        {
            TryAddTemplate(line, SerializeRefTemplteName);
            TryAddTemplate(line, SerializePrimitiveTemplateName);

            TryAddTemplate(line, DeserializeRefTemplteName);
            TryAddTemplate(line, DeserializePrimitiveTemplateName);
        }
    }
}
