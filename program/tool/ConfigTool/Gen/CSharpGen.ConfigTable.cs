using ConfigTool.ConfigInfo;
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
            string filestr = GenerateConfigTableFileContent(configTable);


            string filename = configTable.Name + ".cs";
            string filepath = Path.Combine(outPath, filename);
            File.WriteAllText(filepath, filestr, Encoding.UTF8);
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


        private List<string> ConfigTableHandleOneLineExpand(ConfigTable configTable, string line, StreamReader streamReader)
        {
            List<string> lineList = new List<string>();
            if (line.Contains("_ClassField_"))
            {
                var lines = ReadUntilEndIf(streamReader);

                foreach (var configField in configTable.FieldList)
                {

                    foreach (var l in lines)
                    {
                        var innerLine = l.Replace("_Comment_", configField.Desc)
                                        .Replace("_FieldType_", ConvertType(configField.Type))
                                        .Replace("_FieldName_", configField.Name);

                        lineList.Add(innerLine);
                    }
                }
            }
             
            else if(line.Contains("_TableKeyEnum_") )
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

            else if (line.Contains("_ExtField_"))
            {
                var lines = ReadUntilEndIf(streamReader);

                foreach (var configField in configTable.FieldList)
                {
                    foreach (var l in lines)
                    {
                        var fieldLine = l.Replace("_FieldType_", ConvertType(configField.Type));
                        fieldLine = fieldLine.Replace("_FieldName_", configField.Name);
                        lineList.Add(fieldLine);
                    }
                }
            }


            else if (line.Contains("_ExtTable_"))
            {
                foreach (var l in ReadUntilEndIf(streamReader))
                {
                    var innerLine = l.Replace("_Key_", configTable.GetKeyField().Name);
                    innerLine = innerLine.Replace("_KeyType_",configTable.GetKeyField().Type);
                    innerLine = innerLine.Replace(nameof(_TemplateClass_), configTable.Name);

                    lineList.Add(innerLine);
                }
            }

            else if(line.Contains("_ExtTableData_"))
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


    internal class TableInitDataDecorator : IDisposable
    {
        private string type;
        private StringBuilder stringBuilder;
        public TableInitDataDecorator(string type, StringBuilder stringBuilder) 
        {
            this.type = type;
            this.stringBuilder = stringBuilder;

            Init();
        }

        private void Init()
        {
            if (IsContainerType(type))
            {
                stringBuilder.Append("new ").Append(type).Append("(){");
            }

            if(IsText(type))
            {
                stringBuilder.Append("\"");
            }


        }

        public void Dispose()
        {
            if (IsContainerType(type))
                stringBuilder.Append("}");

            if(IsText(type))
            {
                stringBuilder.Append("\"");
            }

            if(IsFloat(type))
            {
                stringBuilder.Append("f");
            }

        }




        private bool IsContainerType(string type)
        {
            if (type.Contains("List"))
                return true;

            return false;

        }

        private bool IsText(string type)
        {
            if (type.ToLower().Equals("string"))
                return true;

            return false;
        }

        private bool IsFloat(string type)
        {
            if (type.ToLower().Equals("float") || type.ToLower().Equals("double"))
                return true;

            return false;
        }

    }
}
