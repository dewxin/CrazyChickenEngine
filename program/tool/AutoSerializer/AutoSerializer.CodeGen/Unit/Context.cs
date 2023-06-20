using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoSerializer.CodeGen.Unit
{
    internal class Context
    {
        public static Context Instance { get; set; } = new Context();


        public void Init(string classNamespace)
        {
            NameSpace= classNamespace;
        }


        //自动生成的类的命名空间
        public string NameSpace { get; set; } = nameof(AutoSerializer);
    }
}
