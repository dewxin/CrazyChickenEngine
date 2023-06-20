using AutoSerializer.CodeGen.Unit;
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
    /// <summary>
    /// 支持 _Generate_Time_,
    /// </summary>

    internal abstract partial class TemplateHandlerBase
    {
        protected Type targetType;
        protected List<string> result = new List<string>();


        public TemplateHandlerBase(Type targetType)
        {
            this.targetType = targetType;
        }


        public List<string> Parse()
        {
            ParseInner();

            return result;

        }


        private void ParseInner()
        {
            string line;
            while ((line = ReadLine()) != null)
            {
                //先用派生类处理，不行再自己处理
                if(!Handle1LineEx(line)) 
                    Handle1Line(line);
            }
        }

        protected abstract bool Handle1LineEx(string line);


        //可能会由于上下文处理这一行文本时，又接着读了几行。
        private void Handle1Line(string line)
        {
            if (line.Contains("_Generate_Time_"))
            {
                ReadUntilEndregion();
                this.result.Add($"// Genereated at time: {DateTime.Now.ToString()}");
            }
            else if(line.Contains("namespace"))
            {
                this.result.Add($"namespace {Context.Instance.NameSpace}");

            }
            else if (line.Contains("_Template_"))
            {
                HandleTemplate();
            }
            else //default
            {
                result.Add(line);
            }
        }


    }
}
