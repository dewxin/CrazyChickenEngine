using AutoConfig;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using ConfigTool.ConfigInfo;

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
                var enumRecordField = record.GetRecordField(enumField);

                //这里 如果 enum字段为空就跳过，不然会生成 " = 1" 这种编译不通过的代码
                if(enumRecordField.OriginValue != string.Empty)
                {
                    configTable.IDEnumCodeList.Add($"{enumRecordField.OriginValue}={keyFieldValue},");
                }

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

                var recordField = record.GetRecordField(importTableField);
                var fieldImportRegion = recordField.Value;
                if(fieldImportRegion.Trim() == string.Empty)
                {
                    recordField.Value = ExcelReader.GetDefaultValue(importTableField.Type);
                    continue;
                }

                bool InDataRegion = false;
                bool foundData = false;
                StringBuilder stringBuilder= new StringBuilder();
                stringBuilder.AppendLine();
                foreach (var dataSourceLine in dataSourceLines)
                {
                    if(dataSourceLine.StartsWith("#region") && 
                        dataSourceLine.Replace("#region","").Trim().Equals(fieldImportRegion))
                    {
                        InDataRegion = true;
                        foundData = true;
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

                if(!foundData)
                {
                    recordField.Value = ExcelReader.GetDefaultValue(importTableField.Type);

                    var table = importTableField.Table;
                    Debug.LogError($"{fieldImportRegion} at {recordField.Cell.Address} of {table.SourceFile} cannot be found in {dataFileName}");
                }

                if(foundData)
                {
                    record.GetRecordField(importTableField).Value = stringBuilder.ToString();
                }


            }
        }

    }


}
