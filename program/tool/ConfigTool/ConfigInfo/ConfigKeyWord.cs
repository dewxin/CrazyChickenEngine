using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConfigTool.ConfigInfo
{
    public class ConfigKeyWord
    {
        public const char Seperator = ';';

        public const string TableMarker = "<Table>";
        public const string TableName = "Name";
        public const string TableNameSpace = "NameSpace";
        public const string TableRefClassFile = "RefClass";
        public const string KeyValueTable = "KVTable";


        public const string RecordColumnName = "name";
        public const string RecordColumnNameAbbr = "n";
        public const string RecordColumnType = "type";
        public const string RecordColumnTypeAbbr = "t";
        public const string RecordColumnKey = "key";
        public const string RecordColumnGetter = "getter";
        public const string RecordColumnKeyGenerateEnum = "keyGenerateEnum";
        public const string RecordColumnImportData = "import";
    }
}
