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
        public string Value { get; set; }

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
            foreach(var field in RecordFieldList)
            {
                if (field.ColumnIndex == tableField.ColumnIndex)
                    return field.Value;
            }

            return null;
        }

        public override string ToString()
        {
            StringBuilder stringBuilder= new StringBuilder();

            foreach(var field in RecordFieldList)
            {
                stringBuilder.AppendLine(field.ToString());
            }

            return stringBuilder.ToString();
        }

    }
}
