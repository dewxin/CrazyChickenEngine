using SqlDataCommon;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ConfigTool.Gen
{
    internal class CSharpGenNew : GenBase
    {

        protected override void GenerateImpl(string outPath)
        {
            foreach (var st in SqlTableDict)
            {
                BuildFile(st.Value, outPath);
            }
        }


        void BuildFile(SqlTable sqlTable, string outPath)
        {
            string filestr = GenerateFileContent(sqlTable);


            string filename = sqlTable.Name + ".cs";
            string filepath = Path.Combine(outPath, filename);
            File.WriteAllText(filepath, filestr, Encoding.UTF8);
            Console.WriteLine("BuildFile: " + filepath);
        }

        string ConvertType(string st)
        {
            switch (st)
            {
                case "guid":
                    return "Guid";
                case "int64":
                    return "Int64";
                case "uint64":
                    return "UInt64";
                case "date":
                    return "DateTime";
                case "map":
                    return "Dictionary";
                case "list":
                    return "List";
                case "smap":
                    return "SortedDictionary";
                case "slist":
                    return "SortedList";
                default:
                    return st;
            }

        }

        private string GenerateFileContent(SqlTable sqlTable)
        {
            StringBuilder stringBuilder = new StringBuilder();
            var stream = typeof(_TemplateClass_).Assembly.GetManifestResourceStream($"ConfigTool.Template.{nameof(_TemplateClass_)}.cs");
    

            using (var streamReader = new StreamReader(stream))
            {

                string line;
                while ((line = streamReader.ReadLine()) != null)//读取每一行数据
                {
                    foreach(var retLine in HandleOneLine(sqlTable, line, streamReader))
                    {
                        stringBuilder.AppendLine(retLine);
                    }
                }
            }

            return stringBuilder.ToString();
        }

        // 虽然是读一行，但可能会由于当前一行的语义 又接着读了几行
        private List<string> HandleOneLine(SqlTable sqlTable, string line, StreamReader streamReader)
        {
            List<string> lineList = new List<string>();

            if (line.Contains(nameof(_TemplateClass_)))
            {
                line = line.Replace(nameof(_TemplateClass_), sqlTable.Name);
            }


            if (line.StartsWith("#if"))
            {
                if (line.Contains("_ClassField_"))
                {
                    var lines = ReadUntilEndif(streamReader);

                    foreach (var sqlField in sqlTable.Fields)
                    {
                        foreach(var l in lines)
                        {
                            var innerLine= l.Replace("_Comment_", sqlField.Desc)
                                            .Replace("_FieldType_", ConvertType(sqlField.Type))
                                            .Replace("_FieldName_", sqlField.Name);

                            lineList.Add(innerLine);
                        }
                   }
                }

                else if (line.Contains("_Map_"))
                {
                    int tabCount = 3;
                    lineList.Add(GetTab(tabCount) + string.Format("Table(\"{0}\");", sqlTable.Data.TableName));
                    foreach (var sqlField in sqlTable.Fields)
                    {
                        var mapLine = BuildClassMapField(sqlField, tabCount);
                        lineList.Add(mapLine);
                    }
                    lineList.Add(GetTab(tabCount) + "DynamicUpdate();");

                    ReadUntilEndif(streamReader);
                }

                else if (line.Contains("_ExtField_"))
                {
                    var lines = ReadUntilEndif(streamReader);

                    foreach (var sqlField in sqlTable.Fields)
                    {
                        foreach(var l in lines)
                        {
                            var fieldLine = l.Replace("_FieldType_", ConvertType(sqlField.Type));
                            fieldLine = fieldLine.Replace("_FieldName_", sqlField.Name);
                            lineList.Add(fieldLine);
                        }
                    }
                }

                else if (line.Contains("_ExtJson_"))
                {
                    var lines = ReadUntilEndif(streamReader);
                    foreach (var sqlField in sqlTable.Fields)
                    {
                        if (string.IsNullOrEmpty(sqlField.ContainerType))
                            continue;

                        foreach(var l in lines)
                        {
                            var innerLine= l.Replace("_FieldType_", ConvertType(sqlField.Attributes[sqlField.ContainerType]))
                                            .Replace("_FieldName_", sqlField.Name)
                                            .Replace("_ContainerType_", ConvertType(sqlField.ContainerType));

                            if (sqlField.ContainerType.Contains("map"))
                                innerLine = innerLine.Replace("_ContainerTrick_", "int,");
                            else if (sqlField.ContainerType.Contains("list"))
                                innerLine = innerLine.Replace("_ContainerTrick_", "");


                            lineList.Add(innerLine);
                        }
                    }
                }

                else if(line.Contains("_ExtTable_"))
                {
                    foreach (var l in ReadUntilEndif(streamReader))
                    {
                        var innerLine = l.Replace("_Key_", sqlTable.GetKeyField().Name);
                        innerLine = innerLine.Replace(nameof(_TemplateClass_), sqlTable.Name);

                        lineList.Add(innerLine);
                    }
                }

            }
            else
            {
                lineList.Add(line);
            }

            return lineList;
        }


        private List<string> ReadUntilEndif(StreamReader reader)
        {
            List<string> lineList = new List<string>();

            string innerLine;
            while (!(innerLine = reader.ReadLine()).StartsWith("#endif"))
            {
                lineList.Add(innerLine);
            }
            return lineList;
        }


        #region 生成 Fluent-NHibernate使用的ClassMap注册类

        string BuildClassMapField(SqlField sqlField, int tab = 1)
        {
            if (sqlField.Attributes.ContainsKey("id"))
            {
                return BuildClassMapKeyField(sqlField, tab);
            }
            else
            {
                return BuildClassMapNormalField(sqlField, tab);
            }
 
        }

        string BuildClassMapKeyField(SqlField sqlField, int tab = 1)
        {
            if (!IdTypeValid(sqlField))
                throw new Exception("id type error: " + sqlField.Type);

            string mapFieldStr = GetTab(tab);

            mapFieldStr += string.Format("Id(x => x.{0})", sqlField.Name);
            //如果不是自动增长的id
            if (!sqlField.Attributes["id"].Contains("inc"))
            {
                mapFieldStr += ".GeneratedBy.Assigned();";
                return mapFieldStr;
            }

            if (sqlField.Type == "guid")
            {
                mapFieldStr += ".GeneratedBy.GuidComb();";
            }
            else if (sqlField.Type == "string")
            {
                mapFieldStr += ".GeneratedBy.UuidHex(\"N\");";
            }
            else if (sqlField.Attributes.ContainsKey("default"))
            {
                mapFieldStr += $".GeneratedBy.Native(x=>x.AddParam(\"initial_value\", \"{sqlField.Attributes["default"]}\"));";
            }
            else
            {
                mapFieldStr += ".GeneratedBy.Native();";
            }

            return mapFieldStr;
        }

        string BuildClassMapNormalField(SqlField sqlField, int tab = 1)
        {
            string mapFieldStr = GetTab(tab);

            mapFieldStr += string.Format("Map(x => x.{0})", sqlField.Name);
            if (sqlField.Attributes.ContainsKey("default"))
            {
                mapFieldStr += string.Format(".Default(\"{0}\")", sqlField.Attributes["default"]);
            }
            else
            {
                mapFieldStr += AddDefault(sqlField);
            }

            if (sqlField.Type == "string" && sqlField.Attributes.ContainsKey("text"))
            {
                mapFieldStr += ".CustomSqlType(\"text\")";
            }
            else if (sqlField.Attributes.ContainsKey("index"))
            {
                mapFieldStr += string.Format(".Index(\"_{0}_\")", sqlField.Name.ToLower());
            }
            else if (sqlField.Attributes.ContainsKey("unique"))
            {
                mapFieldStr += ".Unique()";
            }
            return mapFieldStr += ";";
        }

        bool IdTypeValid(SqlField sqlField)
        {
            switch (sqlField.Type)
            {
                case "float":
                case "double":
                case "bool":
                case "date":
                    {
                        return false;
                    }
            }
            return true;
        }

        string AddDefault(SqlField sqlField)
        {
            switch (sqlField.Type)
            {
                case "string":
                    {
                        return ".Default(\"\")";
                    }
                case "short":
                case "ushort":
                case "int":
                case "uint":
                case "int64":
                case "uint64":
                case "float":
                case "double":
                    {
                        return ".Default(\"0\")";
                    }
                case "bool":
                    {
                        return ".Default(\"false\")";
                    }
                //case "date":
                //    {
                //        return ".Default(DateTime.MinValue)";
                //    }
                default:
                    {
                        return "";
                    }
            }
        }
        #endregion


    }
}
