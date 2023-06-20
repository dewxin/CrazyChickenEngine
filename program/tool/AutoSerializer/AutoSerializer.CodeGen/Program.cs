using CommandLine.Text;
using CommandLine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Configuration.Assemblies;
using AutoSerializer.CodeGen.Unit;

namespace AutoSerializer
{


    public class Options
    {
        [Option('a', "AssemblyPath", Required = true, HelpText = "需要生成代码的接口所在程序集")]
        public string AssemblyPath { get; set; }

        [Option('c', "CodePath", Required = true, HelpText ="生成代码放置的目录")]
        public string CodePath { get; set; }

        [Option('n', "NameSpace", Required = false, HelpText ="生成代码的命名空间")]
        public string NameSpace { get; set; } = nameof(AutoSerializer);
    }

    internal class Program
    {
        static void Main(string[] args)
        {
            Parser.Default.ParseArguments<Options>(args).WithParsed(Run);
        }

        private static void Run(Options option)
        {
            Assembly assembly = Assembly.LoadFrom(option.AssemblyPath);

            Context.Instance.Init(option.NameSpace);

            GenCore.Instance.Work(assembly, option.CodePath);
        }


    }

}
