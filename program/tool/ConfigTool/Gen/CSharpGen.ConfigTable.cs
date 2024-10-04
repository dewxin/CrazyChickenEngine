using AutoConfig;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using NPOI.Util;
using System.ComponentModel;
using ConfigTool.ConfigInfo;

namespace ConfigTool.Gen
{
    internal partial class CSharpGen
    {

        protected  void GenerateConfigTable(string outPath)
        {
            foreach (var st in name2ConfigTableDict)
            {
                GenerateConfigTableFile(st.Value, outPath);
            }
        }


        void GenerateConfigTableFile(ConfigTable configTable, string outPath)
        {
            string fileStr = string.Empty;
            if (configTable.IsKVTable)
            {
                fileStr = GenerateKVTableFileContent(configTable);
            }
            else
            {

                fileStr = GenerateConfigTableFileContent(configTable);
            }


            string filename = configTable.Name + ".cs";
            string filepath = Path.Combine(outPath, filename);
            File.WriteAllText(filepath, fileStr, Encoding.UTF8);
            Console.WriteLine("BuildFile: " + filepath);
        }


        private string GenerateConfigTableFileContent(ConfigTable configTable)
        {
            StringBuilder stringBuilder = new StringBuilder();
            var stream = typeof(_TemplateClass_).Assembly.GetManifestResourceStream($"ConfigTool.Template.{nameof(_TemplateClass_)}.cs");


            using (var streamReader = new StreamReader(stream))
            {

                string line;
                while ((line = streamReader.ReadLine()) != null)//读取每一行数据
                {
                    foreach (var retLine in ConfigTableHandleOneLine(configTable, line, streamReader))
                        stringBuilder.AppendLine(retLine);
                }
            }

            return stringBuilder.ToString();
        }

        // 虽然是读一行，但可能会由于当前一行的语义 又接着读了几行
        private List<string> ConfigTableHandleOneLine(ConfigTable configTable, string line, StreamReader streamReader)
        {
            if (line.StartsWith("#if"))
            {
                return ConfigTableHandleOneLineExpand(configTable, line, streamReader);
            }

            List<string> lineList = new List<string>();

            if (line.Contains(nameof(_TemplateClass_)))
            {
                line = line.Replace(nameof(_TemplateClass_), configTable.Name);
            }

            lineList.Add(line);
            return lineList;

        }


        private List<string> ConfigTableHandleOneLineExpand(ConfigTable configTable, string lineContainIF, StreamReader streamReader)
        {
            List<string> lineList = new List<string>();
            if (lineContainIF.Contains("_ClassField_"))
            {
                var lines = ReadUntilEndIf(streamReader);

                foreach (var configField in configTable.FieldList)
                {

                    foreach (var l in lines)
                    {
                        var innerLine = l.Replace("_Comment_", configField.Comment)
                                        .Replace("_FieldType_", ConvertType(configField.Type))
                                        .Replace("_FieldName_", configField.Name);

                        lineList.Add(innerLine);
                    }
                }
            }
             
            else if(lineContainIF.Contains("_TableKeyEnum_") )
            {
                var lines = ReadUntilEndIf(streamReader);

                if(configTable.IDEnumCodeList.Count > 0)
                {
                    foreach (var l in lines)
                    {
                        var innerLine = l;

                        if (innerLine.Contains("_TableKeyEnumContent_"))
                        {
                            foreach (var IDEnumCode in configTable.IDEnumCodeList)
                            {
                                var tmpLine = innerLine;
                                tmpLine = tmpLine.Replace("_TableKeyEnumContent_", IDEnumCode);
                                lineList.Add(tmpLine);
                            }

                        }
                        else
                        {
                            innerLine = innerLine.Replace("_TemplateClassEnum_", configTable.IDEnumType);
                            lineList.Add(innerLine);
                        }

                    }
                }

            }

            else if (lineContainIF.Contains("_ExtField_"))
            {
                var lines = ReadUntilEndIf(streamReader);

                foreach (var configField in configTable.FieldList)
                {
                    foreach (var l in lines)
                    {
                        var fieldLine = l.Replace("_FieldType_", ConvertType(configField.Type));
                        fieldLine = fieldLine.Replace("_FieldName_", configField.Name);
                        fieldLine = fieldLine.Replace("_Comment_", configField.Comment);
                        lineList.Add(fieldLine);
                    }
                }
            }


            else if (lineContainIF.Contains("_ExtTable_"))
            {
                foreach (var l in ReadUntilEndIf(streamReader))
                {
                    var innerLine = l.Replace("_Key_", configTable.GetKeyField().Name);
                    innerLine = innerLine.Replace("_KeyType_",configTable.GetKeyField().Type);
                    innerLine = innerLine.Replace(nameof(_TemplateClass_), configTable.Name);

                    lineList.Add(innerLine);
                }
            }

            ///TODO 这里 <see cref="KVTableHandleOneLineExpand"/>也有一份对应的代码
            else if (lineContainIF.Contains("_TableNameSpace_"))
            {
                var lines = ReadUntilEndIf(streamReader);

                foreach (var nameSpace in configTable.NameSpaceList)
                {
                    foreach (var l in lines)
                    {
                        var fieldLine = l.Replace("__TableNameSpaceContent_", nameSpace);
                        lineList.Add(fieldLine);
                    }
                }
            }

            else if(lineContainIF.Contains("_ExtTableData_"))
            {
                int index = 0;

                var extTableDataLines = ReadUntilEndIf(streamReader);

                foreach(var record in configTable.RecordList)
                {
                    index++;

                    foreach (var l in extTableDataLines)
                    {
                        var innerLine = l;

                        if(innerLine.Contains("_InitField_"))
                        {
                            foreach(var recordField in record.RecordFieldList)
                            {
                                var stringBuilder = new StringBuilder();
                                var fieldInfo = configTable.GetFieldByColumn(recordField.ColumnIndex);

                                //field = value
                                stringBuilder.Append(fieldInfo.Name).Append("=");

                                using(var decorator = new TableInitDataDecorator(fieldInfo.Type, stringBuilder))
                                {
                                    stringBuilder.Append(recordField.Value);
                                }



                                stringBuilder.Append(",");
                                var tmpLine = innerLine.Replace("_InitField_", stringBuilder.ToString());

                                lineList.Add(tmpLine);
                            }

                        }
                        else
                        {
                            innerLine = innerLine.Replace("_Key_", configTable.GetKeyField().Name);
                            innerLine = innerLine.Replace("_Index_", index.ToString());
                            innerLine = innerLine.Replace("_KeyType_", configTable.GetKeyField().Type);
                            innerLine = innerLine.Replace(nameof(_TemplateClass_), configTable.Name);

                            lineList.Add(innerLine);
                        }

                    }
                }


            }

            return lineList;
        }


    }


}
