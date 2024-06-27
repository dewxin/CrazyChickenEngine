using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConfigTool.ConfigInfo
{
    public class ConfigTable
    {
        public string SourceFile { get; set; }
        public string SourceFileDir => Path.GetDirectoryName(SourceFile);

        public string Name { get; set; } = string.Empty;
        //XXX.cs
        public List<string> ReferenceClassFileList { get; set; } = new List<string>();

        public List<ConfigTableField> FieldList { get; set; } = new List<ConfigTableField>();

        public List<ConfigRecord> RecordList { get; set; } = new List<ConfigRecord>();

        public string IDEnumType { get; set; }
        public List<string> IDEnumCodeList { get; set; } = new List<string>();

        public ConfigTableField GetKeyField()
        {
            foreach(var column in FieldList)
            {
                if(column.IsKey)
                    return column;
            }
            
            throw new Exception("no key field");
        }

        public ConfigTableField GetField(Predicate<ConfigTableField> predicate)
        {
            foreach (var column in FieldList)
            {
                if (predicate(column))
                    return column;
            }
            return null;
        }

        public ConfigTableField GetFieldByColumn(int column)
        {
            foreach(var filed in FieldList)
            {
                if(filed.ColumnIndex== column) return filed;
            }
            return null;
        }

        public void RemoveFiled(ConfigTableField tableField)
        {
            FieldList.Remove(tableField);

            foreach(var record in RecordList)
            {
                record.RecordFieldList.RemoveAll(field => field.ColumnIndex == tableField.ColumnIndex);
            }

        }

        public override string ToString()
        {
            string headerStr = $"TableName={Name} RefClassFile={ReferenceClassFileList.ToString()}";

            StringBuilder columnsStrBuilder = new StringBuilder();
            foreach(var column in FieldList)
            {
                columnsStrBuilder.AppendLine(column.ToString());
            }

            StringBuilder recordStrBuilder= new StringBuilder();
            foreach (var record in RecordList)
            {
                recordStrBuilder.AppendLine(record.ToString());
            }

            return headerStr+Environment.NewLine+columnsStrBuilder.ToString()+Environment.NewLine+ recordStrBuilder.ToString();
        }
    }
}
