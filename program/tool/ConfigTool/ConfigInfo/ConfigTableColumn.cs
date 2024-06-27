using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConfigTool.ConfigInfo
{
    public class ConfigTableField
    {
        public ConfigTable Table { get; set; }

        public int ColumnIndex { get; set; }

        public string Name { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public string Desc { get; set; }

        public bool IsKey { get; set; }

        public bool NeedKeyGenerateEnum { get; set; }

        public bool NeedImportData { get; set; }
        // import=MonsterConfig_MonsterLoot.cs
        public string ImportDataFile { get; set; }



        public override string ToString()
        {
            var result = $"TableColumnIndex={ColumnIndex} :";
            if (Name != string.Empty)
                result += $" Name={Name}";
            if (Type != string.Empty)
                result += $" Type={Type}";
            if (IsKey)
                result += $" IsKey={IsKey}";

            if(NeedKeyGenerateEnum)
                result += $" NeedKeyGenerateEnum={ NeedKeyGenerateEnum}";
            if (NeedImportData)
                result += $" NeedImportData={NeedImportData} ImportFile={ImportDataFile}";

            return result ;
        }

    }
}
