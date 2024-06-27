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
        private Dictionary<string, ConfigTable> name2ConfigTableDict = new Dictionary<string, ConfigTable>();
        private Dictionary<string, int> refClassFile2IntDict = new Dictionary<string, int>();

        public void StartGenerate(IEnumerable<string> files, string outpath = "")
        {
            name2ConfigTableDict.Clear();
            refClassFile2IntDict.Clear();


            foreach (var file in files)
            {
                var tablesList = ExcelReader.ReadTablesFromFile(file);
                foreach (var table in tablesList)
                    name2ConfigTableDict.Add(table.Name, table);

            }

            GenerateRefClass(outpath);
            ModifyTables();

            GenerateConfigTable(outpath);
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

        
        private List<string> ReadUntilEndIf(StreamReader reader)
        {
            List<string> lineList = new List<string>();

            string innerLine;
            while (!(innerLine = reader.ReadLine()).StartsWith("#endif"))
            {
                lineList.Add(innerLine);
            }
            return lineList;
        }



    }
}
