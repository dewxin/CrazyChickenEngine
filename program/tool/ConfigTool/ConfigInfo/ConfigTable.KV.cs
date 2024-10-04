using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConfigTool.ConfigInfo
{
    public partial class ConfigTable
    {

        //是否是特殊的 keyValue Table
        public bool IsKVTable { get; set; }

        public List<KVTableRecord> KVRecordList { get; set; } = new List<KVTableRecord>();
    }
}
