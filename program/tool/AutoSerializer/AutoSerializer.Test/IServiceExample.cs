using Param;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoSerializer.Test
{
    public interface IServiceExample
    {
        MethodTask<Class1> Service1(Class6 service1);
        MethodTask<List<Class1>> Service2(Class2 service1);
    }
}
