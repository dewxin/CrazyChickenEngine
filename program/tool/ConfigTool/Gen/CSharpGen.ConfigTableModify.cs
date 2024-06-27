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

        private void ModifyTables()
        {
            foreach(var table in name2ConfigTableDict.Values) 
            {

                var enumFieldInfo = table.GetField(field => field.NeedKeyGenerateEnum);
                GenerateIDEnumAndRemoveEnumField(table, enumFieldInfo);


                foreach(var fieldInfo in table.FieldList)
                {
                    if(fieldInfo.NeedImportData)
                    {
                        ImportFieldData(fieldInfo);
                    }
                }

            }
        }

        private void GenerateIDEnumAndRemoveEnumField(ConfigTable configTable, ConfigTableField enumField)
        {
            if (enumField == null)
                return;

            var keyField = configTable.GetKeyField();

            configTable.IDEnumType = enumField.Type;

            foreach (var record in configTable.RecordList)
            {
                string keyFieldValue = record.GetRecordFieldValue(keyField);
                string enumFieldValue = record.GetRecordFieldValue(enumField);

                configTable.IDEnumCodeList.Add($"{enumFieldValue}={keyFieldValue},");

            }

            configTable.RemoveFiled(enumField);


        }

        private void ImportFieldData(ConfigTableField importTableField)
        {

            var configTable = importTableField.Table;
            var fileName = importTableField.ImportDataFile;

            var dataFileName = Path.Combine(configTable.SourceFileDir, fileName);

            if(!File.Exists(dataFileName))
            {
                Debug.LogError($"data file {dataFileName} not exists");

            }
            var dataSourceLines = File.ReadAllLines(dataFileName);



            foreach (var record in configTable.RecordList)
            {

                var fieldImportRegion = record.GetRecordFieldValue(importTableField);

                bool InDataRegion = false;
                StringBuilder stringBuilder= new StringBuilder();
                stringBuilder.AppendLine();
                foreach (var dataSourceLine in dataSourceLines)
                {
                    if(dataSourceLine.StartsWith("#region") && dataSourceLine.Contains(fieldImportRegion))
                    {
                        InDataRegion= true;
                        continue;
                    }

                    //如果已经开始读了，并且遇到endregion，那就不读
                    if(dataSourceLine.StartsWith("#endregion") && InDataRegion)
                    {
                        break;
                    }

                    if(InDataRegion)
                    {
                        stringBuilder.AppendLine(dataSourceLine);
                    }
                }

                record.GetRecordField(importTableField).Value = stringBuilder.ToString();



            }
        }

    }


}
