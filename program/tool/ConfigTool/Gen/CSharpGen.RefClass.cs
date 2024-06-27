using ConfigTool.ConfigInfo;
using AutoConfig;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ConfigTool.Gen
{
    internal partial class CSharpGen
    {



        private void GenerateRefClass(string outPath = "")
        {
            string filestr = GenerateRefClassFileContent();

            string filename = "RefClassCollection.cs";
            string filepath = Path.Combine(outPath, filename);
            File.WriteAllText(filepath, filestr, Encoding.UTF8);
            Console.WriteLine("BuildFile: " + filepath);

        }

        private string GenerateRefClassFileContent()
        {
            StringBuilder stringBuilder = new StringBuilder();
            var stream = typeof(_TemplateClass_).Assembly.GetManifestResourceStream("ConfigTool.Template.RefClassCollection.cs");


            using (var streamReader = new StreamReader(stream))
            {

                string line;
                while ((line = streamReader.ReadLine()) != null)//读取每一行数据
                {
                    if (line.StartsWith("#if"))
                    {
                        if (line.Contains("_RefClassCollection_"))
                        {
                            ReadUntilEndIf(streamReader);
                            AppendRefClasses(stringBuilder);
                        }
                    }
                    else
                    {
                        stringBuilder.AppendLine(line);
                    }
                }
            }

            return stringBuilder.ToString();
        }


        private void AppendRefClasses(StringBuilder stringBuilder)
        {
            foreach(var configTable in name2ConfigTableDict.Values)
            {
                if (configTable.ReferenceClassFileList.Count() == 0)
                    continue;

                var dirPath = Path.GetDirectoryName(configTable.SourceFile);

                foreach(var classFile in configTable.ReferenceClassFileList)
                {
                    var refClassFile = Path.Combine(dirPath, classFile);

                    if (refClassFile2IntDict.ContainsKey(refClassFile))
                        continue;

                    refClassFile2IntDict.Add(refClassFile, 1);

                    var refClassLines = File.ReadAllLines(refClassFile);

                    foreach (var oneLine in refClassLines)
                    {
                        stringBuilder.AppendLine(oneLine.ToString());
                    }
                }


            }


        }


    }


}
