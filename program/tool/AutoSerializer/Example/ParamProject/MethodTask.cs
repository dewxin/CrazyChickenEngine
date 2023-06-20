using AutoSerializer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParamProject
{
    [Ignore]
    [Transparent]
    public class MethodTask<T>
        where T : class
    {
        T Value;
    }
}
