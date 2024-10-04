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
        private string GenerateKVTableFileContent(ConfigTable configTable)
        {
            StringBuilder stringBuilder = new StringBuilder();
            var stream = typeof(_TemplateClassKV_).Assembly.GetManifestResourceStream($"ConfigTool.Template.{nameof(_TemplateClassKV_)}.cs");


            using (var streamReader = new StreamReader(stream))
            {

                string line;
                while ((line = streamReader.ReadLine()) != null)//读取每一行数据
                {
                    foreach (var retLine in KVTableHandleOneLine(configTable, line, streamReader))
                        stringBuilder.AppendLine(retLine);
                }
            }

            return stringBuilder.ToString();
        }

        // 虽然是读一行，但可能会由于当前一行的语义 又接着读了几行
        private List<string> KVTableHandleOneLine(ConfigTable configTable, string line, StreamReader streamReader)
        {
            if (line.StartsWith("#if"))
            {
                return KVTableHandleOneLineExpand(configTable, line, streamReader);
            }

            List<string> lineList = new List<string>();

            if (line.Contains(nameof(_TemplateClassKV_)))
            {
                line = line.Replace(nameof(_TemplateClassKV_), configTable.Name);
            }

            lineList.Add(line);
            return lineList;

        }

        private List<string> KVTableHandleOneLineExpand(ConfigTable configTable, string lineContainIF, StreamReader streamReader)
        {
            List<string> lineList = new List<string>();

            if (lineContainIF.Contains("_KVTableData_"))
            {
                var lines = ReadUntilEndIf(streamReader);
                foreach (var kvRecord in configTable.KVRecordList)
                {
                    foreach(var innerLine in lines)
                    {
                        var l = innerLine;
                        l = l.Replace("_Comment_", kvRecord.Comment);
                        l = l.Replace("_FieldType_", kvRecord.Type);
                        l = l.Replace("_FieldName_", kvRecord.Name);

                        if(l.Contains("_FieldValue_"))
                        {
                            var strBuilder = new StringBuilder();
                            using( var decorator = new TableInitDataDecorator(kvRecord.Type, strBuilder))
                            {
                                strBuilder.Append(kvRecord.Value);
                            }

                            l = l.Replace("_FieldValue_", strBuilder.ToString());
                        }

                        lineList.Add(l);
                    }

                }


            }

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



            return lineList;
        }

    }


}
