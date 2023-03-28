using ConfigTool.Gen;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConfigTool
{
    internal class Bootstrap
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
            && !argsDict.ContainsKey("-update")
            && !argsDict.ContainsKey("-init")
            ))
            {
                Console.WriteLine("Options error: keymap");
                ShowOptions();
                return;
            }


            DoWork(argsDict);
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

                    //文件
                    case "-update":
                    case "-init":
                        {
                            string path = Path.Combine(Environment.CurrentDirectory, optionValue);
                            if (File.Exists(path))
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
                        break;
                }
            }



            return argDict;
        }

        void ShowOptions()
        {
            //todo 后面再改下描述
            Console.WriteLine("ConfigTool Options:");
            Console.WriteLine("-input=<path>            input *.xls/*.xlsx from <path>.");
            Console.WriteLine("-update=<file>           update data to mysql.");
            Console.WriteLine("-init=<file>             only insert data to mysql.");
            Console.WriteLine("-cs=<path>               build cs file to <path>.");
        }
        

        void DoWork(Dictionary<string,string> argDict)
        {
            var inputFiles = Directory.EnumerateFiles(argDict["-input"], "*.xls*", SearchOption.TopDirectoryOnly);
            if (argDict.ContainsKey("-cs"))
            {
                //CSharpGen gb = new CSharpGen();
                var gb = new CSharpGenNew();
                gb.StartGenerate(inputFiles, argDict["-cs"]);
            }
            //else if (argDict.ContainsKey("-update"))
            //{
            //    UpdateHelper gb = new UpdateHelper();
            //    gb.Update = true;
            //    gb.StartBuild(files, argDict["-update"]);
            //}
            else if (argDict.ContainsKey("-init"))
            {
                MySqlGen gb = new MySqlGen();
                gb.StartGenerate(inputFiles, argDict["-init"]);
            }
        }
    }
}
