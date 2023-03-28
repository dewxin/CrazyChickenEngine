using System;
using System.IO;

namespace Tool.AssemblyMod 
{
    public class ModifyAssembly : Microsoft.Build.Utilities.Task
    {
        public override bool Execute()
        {
            //CecilUser.Work(FileName);
            if(!File.Exists(AssemblyFile))
            {
                Log.LogWarning($"file {AssemblyFile} not exist");
                return true;
            }

            Log.LogMessage($"handling {AssemblyFile}");


            CecilUser.Work(AssemblyFile);

            return true;
        }

        public string AssemblyFile { get; set; }
    }
}