﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoSerializer
{
    [AttributeUsage(AttributeTargets.Class)]
    public class IgnoreAttribute:Attribute
    {

    }
}
