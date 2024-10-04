
/* Unmerged change from project 'ConfigTool (net6.0)'
Before:
using NPOI.SS.UserModel;
After:
using ConfigTool;
using ConfigTool.ConfigInfo;
using ConfigTool.ConfigInfo;
using ConfigTool.ConfigInfo.Common;
using NPOI.SS.UserModel;
*/
using NPOI.SS.UserModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConfigTool.ConfigInfo
{
    public class ConfigRecordFieldValue
    {
        public int ColumnIndex { get; set; }
        public ICell Cell { get; set; }

        //修改后的值，如果是string.empty，会变成各种默认值
        /// <see cref="ExcelReader.GetDefaultValue"/>
        public string Value { get; set; }

        //未经修改的值， 如果没填，那通常是 string.empty
        public string OriginValue { get; set; }

        public override string ToString()
        {
            return $"{ColumnIndex}: {Value}";
        }
    }

    public class ConfigRecord
    {
        public List<ConfigRecordFieldValue> RecordFieldList = new List<ConfigRecordFieldValue>();

        public ConfigRecordFieldValue GetRecordField(ConfigTableField tableField)
        {
            foreach (var field in RecordFieldList)
            {
                if (field.ColumnIndex == tableField.ColumnIndex)
                    return field;
            }

            return null;
        }

        public string GetRecordFieldValue(ConfigTableField tableField)
        {
            foreach (var field in RecordFieldList)
            {
                if (field.ColumnIndex == tableField.ColumnIndex)
                    return field.Value;
            }

            return null;
        }

        public override string ToString()
        {
            StringBuilder stringBuilder = new StringBuilder();

            foreach (var field in RecordFieldList)
            {
                stringBuilder.AppendLine(field.ToString());
            }

            return stringBuilder.ToString();
        }

    }
}
