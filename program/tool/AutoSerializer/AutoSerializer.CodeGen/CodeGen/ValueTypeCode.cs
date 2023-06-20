using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoSerializer
{
    internal class ValueTypeCode
    {
        //值类型
        public Type Type { get; set; }

        public List<string> SerializerCodeLines { get; set; }

        public List<string> DeserializerCodeLines { get; set; }


        public List<string> SerializerCodes(string varName)
        {
            var list = new List<string>();
            foreach (var item in SerializerCodeLines)
            {
                var code = item.Replace(ValueTypeCodeGen._TypeVariableName_, varName);
                //if (isTopValType)
                //    code = code.Replace(ValueTypeCodeGen._VariableEndMark_, ";");
                //else
                //    code = code.Replace(ValueTypeCodeGen._VariableEndMark_, ",");
                list.Add(code);
            }
            return list;
        }

        public List<string> DeserializerCodes(string varName, bool isTopValType)
        {
            var list = new List<string>();
            foreach (var item in DeserializerCodeLines)
            {
                var code = item;
                code = code.Replace(ValueTypeCodeGen._TypeVariableName_, varName);
                if (isTopValType)
                    code = code.Replace(ValueTypeCodeGen._VariableEndMark_, ";");
                else
                    code = code.Replace(ValueTypeCodeGen._VariableEndMark_, ",");
                        

                list.Add(code);
            }
            return list;
        }
    }


    internal class ValueTypeCodeCache
    {
        //值类型
        public static Dictionary<Type, ValueTypeCode> Dict = new Dictionary<Type, ValueTypeCode>();

        public static void Reset()
        {
            Dict.Clear();
        }

    }
}
