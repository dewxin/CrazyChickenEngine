using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConfigTool.Gen
 {
    /// <summary>
    /// 表结构
    /// </summary>
    class SqlTable
    {
        public string Name { get; set; }
        public List<SqlField> Fields = new List<SqlField>();
        public DataTable Data { get; set; }


        public SqlField GetKeyField()
        {
            foreach (var field in Fields)
            {
                if (field.Attributes.ContainsKey("id"))
                    return field;
            }

            return null;
        }
    }

    /// <summary>
    /// 字段
    /// </summary>
    class SqlField
    {
        public SqlField(SqlTable st)
        {
            Owner = st;
        }
        public SqlTable Owner { get; private set; }

        public string Name { get; set; }
        public string Type { get; set; }
        public string Desc { get; set; }

        public Dictionary<string, string> Attributes = new Dictionary<string, string>();

        private static List<string> containterList = new List<string> {"list", "slist", "map", "smap"};

        public string ContainerType { get; private set; }

        public void ParseContainerAttribute()
        {
            int attributeCount = 0;

            foreach (var container in containterList)
            {
                if (Attributes.ContainsKey(container))
                {
                    ContainerType = container;
                    attributeCount++;
                }
            }

            Debug.Assert(attributeCount <= 1);
        }
    }

    internal abstract class GenBase
    {
        protected Dictionary<string, SqlTable> SqlTableDict = new Dictionary<string, SqlTable>();

        public void StartGenerate(IEnumerable<string> files, string outpath = "")
        {
            foreach (var file in files)
            {
                var excelReader = new ExcelReader();
                var dataTable = excelReader.NpoiLoadExcel(file);

                if (dataTable == null)
                    continue;

                ParseAndStoreInDict(dataTable);
            }


            try
            {
                GenerateImpl(outpath);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }


        private void ParseAndStoreInDict(DataTable data)
        {
            SqlTable sqlTable = new SqlTable();
            sqlTable.Name = NameStyleFromUnderscoreToCamel(data.TableName);
            sqlTable.Data = data;

            foreach (var col in data.Columns)
            {
                DataColumn dataColumn = col as DataColumn;
                if (string.IsNullOrEmpty(dataColumn.ColumnName) || dataColumn.ColumnName == "null")
                    break;

                SqlField field = new SqlField(sqlTable);
                field.Name = dataColumn.ColumnName;

                //由于原来第一行作为字段名，没有读入，所以这里第一行(index0)其实是第二行，类型
                field.Type = data.Rows[0][dataColumn.Ordinal].ToString().ToLower().Trim();

                if (!TypeIsValid(field.Type))
                    throw new Exception("type error: " + field.Type);

                //类型下面的一行，属性
                var attributeArray = data.Rows[1][dataColumn.Ordinal].ToString().ToLower().Trim().Split(',');
                if (attributeArray[0] != "")
                {
                    foreach (var attrStr in attributeArray)
                    {
                        if (string.IsNullOrEmpty(attrStr))
                            continue;

                        var attrs = attrStr.Split('&');

                        if (attrs.Length > 1)
                            field.Attributes.Add(attrs[0], attrs[1]);
                        else
                            field.Attributes.Add(attrs[0], string.Empty);
                    }

                    field.ParseContainerAttribute();
                }

                field.Desc = data.Rows[2][dataColumn.Ordinal].ToString();
                sqlTable.Fields.Add(field);
            }

            SqlTableDict.Add(sqlTable.Name, sqlTable);
        }

        bool TypeIsValid(string type)
        {
            switch (type)
            {
                case "short":
                case "ushort":
                case "int":
                case "uint":
                case "int64":
                case "uint64":
                case "double":
                case "float":
                case "bool":
                case "date":
                case "guid":
                case "string":
                    {
                        return true;
                    }
            }
            return false;
        }

        protected string GetTab(int count = 1)
        {
            return new string('\t', count);
        }

        private string NameStyleFromUnderscoreToCamel(string text)
        {
            StringBuilder strBuilder = new StringBuilder();
            var arr = text.Split('_');
            for (int i = 0; i < arr.Length; ++i)
            {
                var temp = arr[i].Substring(0, 1).ToUpper() + arr[i].Substring(1, arr[i].Length - 1);
                strBuilder.Append(temp);
            }
            return strBuilder.ToString();
        }

        protected abstract void GenerateImpl(string outPath);
    }
}
