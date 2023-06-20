using AutoSerializer.CodeGen.Unit;
using System;
using System.IO;
using System.Reflection;
using System.Security.Policy;

namespace AutoSerializer
{
    //TODO 改成Exe直接调用。。msBuild的task bug太多了
    public class SerializerGenTask : Microsoft.Build.Utilities.Task
    {
        public override bool Execute()
        {
            if (!File.Exists(AssemblyPath))
            {
                Log.LogWarning($"file {AssemblyPath} not exist");
                return true;
            }

            Log.LogMessage($"handling {AssemblyPath}");

            Assembly assembly = Assembly.LoadFrom(AssemblyPath);

            Context.Instance.Init(NameSpace);

            GenCore.Instance.Work(assembly, CodePath);


            return true;
        }



        public string AssemblyPath { get; set; }
        public string CodePath { get; set; }

        //自动生成的类的命名空间
        public string NameSpace { get; set; } = nameof(AutoSerializer);
    }
}
