using ConfigTool.Gen;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConfigTool
{
    public class Bootstrap
    {

        public void Start(string[] args)
        {
            Console.WriteLine("work path: " + Environment.CurrentDirectory);

            if (args.Length <= 0)
            {
                Console.WriteLine("Options error: args.Length <= 0 ");
                ShowOptions();
                return;
            }

            var argsDict = ParseArgDict(args);
            if (argsDict == null)
                return;

            if (!argsDict.ContainsKey("-input") ||
            (!argsDict.ContainsKey("-cs")
            ))
            {
                Console.WriteLine("Options error: keymap");
                ShowOptions();
                return;
            }


            DoWork(argsDict["-input"], argsDict["-cs"]);
        }

        private Dictionary<string,string> ParseArgDict(string[] args)
        {
            Dictionary<string, string> argDict = new Dictionary<string, string>();

            foreach(var arg in args)
            {
                string[] nameValue = arg.Split('=');
                if (nameValue == null)
                    continue;

                string optionName = nameValue[0];
                string optionValue = nameValue[1];

                switch(optionName)
                {
                    // 文件目录
                    case "-input":
                    case "-cs":
                        {
                            if(optionValue ==".")
                            {
                                argDict.Add(optionName, Environment.CurrentDirectory);
                                Console.WriteLine("Options: " + optionName + " , " + Environment.CurrentDirectory);
                            }
                            else
                            {
                                string path = Path.Combine(Environment.CurrentDirectory, optionValue);
                                if (Directory.Exists(path))
                                {
                                    argDict.Add(optionName, Path.GetFullPath(path));
                                    Console.WriteLine("Options: " + optionName + " , " + path);
                                }
                                else
                                {
                                    Console.WriteLine("check path error key: " + optionName + " , path: " + path);
                                    return null;
                                }

                            }
                        }
                        break;

                }
            }



            return argDict;
        }

        void ShowOptions()
        {
            Console.WriteLine("ConfigTool Options:");
            Console.WriteLine("-input=<path>            input *.xls/*.xlsx from <path>.");
            Console.WriteLine("-cs=<path>               build cs file to <path>.");
        }
        

        public static void DoWork(string inputExcelDir, string outputCodeDir)
        {
            var inputFiles = Directory.EnumerateFiles(inputExcelDir, "*.xls*", SearchOption.TopDirectoryOnly);

            var gb = new CSharpGen();
            gb.StartGenerate(inputFiles, outputCodeDir);
        }
    }
}
