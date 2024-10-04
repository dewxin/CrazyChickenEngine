using ConfigTool.ConfigInfo;
using NPOI.SS.UserModel;
using Org.BouncyCastle.Cms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ConfigTool
{
    public partial class ExcelReader
    {


        private bool TryLocateTableHeader(ISheet sheet, int rowIndexStart, out ICell tableHeaderCell)
        {
            for (int rowIndex = rowIndexStart; rowIndex <= sheet.LastRowNum; ++rowIndex)
            {
                IRow row = sheet.GetRow(rowIndex);

                if (row == null)
                {
                    //ConfigDebug.LogInfo($"Row {rowIndex} is empty, Skip <<<<<<<<<<");
                    continue;
                }

                if (row.FirstCellNum < 0 || row.LastCellNum < 0)
                {
                    //ConfigDebug.LogInfo($"Row {rowIndex} FirstCellNum negative, Skip <<<<<<<<<<");
                    continue;
                }


                for (int i = row.FirstCellNum; i <= row.LastCellNum; ++i)
                {
                    var cell = row.GetCell(i);

                    if (cell == null)
                        continue;

                    string cellValue = GetValue(cell);

                    //ConfigDebug.Log($"Cell{cell.Address} Type={cell.CellType} Value={cellValue}");

                    if(cellValue.Contains(ConfigKeyWord.TableMarker))
                    {
                        tableHeaderCell = cell;
                        return true;
                    }

                }
            }


            tableHeaderCell = null;
            return false;
        }

        private ConfigTable ReadTable(ICell tableHeaderCell)
        {
            //<Table>   //表信息
            //怪物ID ... //中文注释
            //name=ID,key,type=int  ...  //字段信息

            var configTable = new ConfigTable();
            ReadTableHeader(configTable, tableHeaderCell);
            Debug.Log($">>>>> Locate table {configTable.Name} In {fileName}.{tableHeaderCell.Sheet.SheetName} <<<<<");
            if(configTable.IsKVTable)
            {
                ReadKVTableRecords(configTable, tableHeaderCell.Sheet, tableHeaderCell.RowIndex + 3);
            }
            else
            {
                ReadTableColumn(configTable, tableHeaderCell.Sheet, tableHeaderCell.RowIndex + 2);
                ReadTableRecords(configTable, tableHeaderCell.Sheet, tableHeaderCell.RowIndex + 3);
            }

            return configTable;
        }

        private void ReadTableHeader(ConfigTable configTable, ICell tableHeaderCell)
        {
            string cellValue = GetValue(tableHeaderCell);

            var entryList = cellValue.Split(ConfigKeyWord.Seperator);
            foreach(var entry in entryList)
            {
                bool tableEntryIsValid = false;
                if (entry.Contains(ConfigKeyWord.TableMarker))
                {
                    tableEntryIsValid = true;
                    continue;
                }
                if (entry.Contains(ConfigKeyWord.KeyValueTable))
                {
                    tableEntryIsValid = true;
                    configTable.IsKVTable = true;
                    continue;
                }
                if (entry.Trim() == string.Empty)
                    continue;

                var keyValue = entry.Trim().Split('=');

                if(keyValue.Count() != 2)
                {
                    Debug.LogError($"{entry} in {tableHeaderCell.Address} is not valid");
                    continue;
                }

                string key = keyValue[0].Trim();
                string value = keyValue[1].Trim();



                if(key == ConfigKeyWord.TableName)
                {
                    tableEntryIsValid = true;
                    configTable.Name = value;
                }

                if(key == ConfigKeyWord.TableRefClassFile)
                {
                    tableEntryIsValid = true;
                    var fileList = value.Split(',');
                    foreach(var file in fileList)
                    {
                        var fileTrim = file.Trim();
                        if (fileTrim == string.Empty)
                            continue;

                        configTable.ReferenceClassFileList.Add(fileTrim);
                    }
                }

                if(key == ConfigKeyWord.TableNameSpace)
                {
                    tableEntryIsValid = true;
                    var namespaceList = value.Split(',');
                    foreach (var OneNameSpace in namespaceList)
                    {
                        var nameSpaceTrim = OneNameSpace.Trim();
                        if (nameSpaceTrim == string.Empty)
                            continue;

                        configTable.NameSpaceList.Add(nameSpaceTrim);
                    }
                }


                if (!tableEntryIsValid)
                {
                    Debug.LogError($"{key} in {tableHeaderCell.Address} is not valid");
                }

            }

            if(configTable.Name == string.Empty)
            {
                Debug.LogError($"tableName is Empty File={absoluteFileName} ");
            }


        }

        private void ReadTableColumn(ConfigTable configTable, ISheet sheet, int rowIndexStart)
        {
            IRow row = sheet.GetRow(rowIndexStart);
            IRow commentRow = sheet.GetRow(rowIndexStart-1);


            for (int i = row.FirstCellNum; i <= row.LastCellNum; ++i)
            {
                var cell = row.GetCell(i);
                var commentCell = commentRow.GetCell(i);

                if (cell == null)
                    continue;

                string cellValue = GetValue(cell);
                if (cellValue == null || cellValue.Trim() == string.Empty)
                    continue;

                string commentValue = GetValue(commentCell);

                var entryList = cellValue.Split(ConfigKeyWord.Seperator);

                var configTableColumn = new ConfigTableField();
                configTableColumn.ColumnIndex = i;
                configTableColumn.Comment = commentValue;
                configTableColumn.Table = configTable;
                configTable.FieldList.Add(configTableColumn);


                foreach (var entry in entryList)
                {
                    var entryTrimed = entry.Trim();
                    if (entryTrimed.Equals(ConfigKeyWord.RecordColumnKey))
                    {
                        configTableColumn.IsKey = true;
                        continue;
                    }

                    if(entryTrimed.Equals(ConfigKeyWord.RecordColumnGetter))
                    {
                        configTableColumn.IsGetter = true;
                        continue;
                    }

                    if(entryTrimed.Equals(ConfigKeyWord.RecordColumnKeyGenerateEnum))
                    {
                        configTableColumn.NeedKeyGenerateEnum = true;
                        continue;
                    }

                    if(entryTrimed.Equals(string.Empty))
                    {
                        continue;
                    }



                    var keyValue = entryTrimed.Split('=');

                    if (keyValue.Count() != 2)
                    {
                        Debug.LogError($"{entryTrimed} in {cell.Address} is not valid");
                        continue;
                    }

                    string key = keyValue[0].Trim();
                    string value = keyValue[1].Trim();

                    if (key == ConfigKeyWord.RecordColumnName || key == ConfigKeyWord.RecordColumnNameAbbr)
                    {
                        configTableColumn.Name = value;
                    }

                    if (key == ConfigKeyWord.RecordColumnType || key == ConfigKeyWord.RecordColumnTypeAbbr)
                    {
                        configTableColumn.Type = value;
                    }

                    if(key == ConfigKeyWord.RecordColumnImportData)
                    {
                        configTableColumn.NeedImportData = true;
                        configTableColumn.ImportDataFile = value;
                    }

                }

                if(configTableColumn.Type == string.Empty && !configTableColumn.NeedKeyGenerateEnum)
                    Debug.LogError($"{cell.Address}: column type is empty");

                if(configTableColumn.Name == string.Empty && !configTableColumn.NeedKeyGenerateEnum)
                    Debug.LogError($"{cell.Address}: column name is empty");

            }
        }

        public void ReadTableRecords(ConfigTable configTable, ISheet sheet, int rowIndexStart)
        {
            for (int rowIndex = rowIndexStart; rowIndex <= sheet.LastRowNum; ++rowIndex)
            {
                IRow row = sheet.GetRow(rowIndex);

                if (row == null)
                {
                    //Debug.LogInfo($"Row {rowIndex} is empty, Stop Read Table Records");
                    return;
                }

                if (row.FirstCellNum < 0 || row.LastCellNum < 0)
                {
                    //Debug.LogInfo($"Row {rowIndex} is empty, Stop Read Table Records");
                    return;
                }
                var keyField = configTable.GetKeyField();
                var keyCell = row.GetCell(keyField.ColumnIndex);
                if (keyCell == null)
                {
                    //Debug.LogInfo($"Row {rowIndex} key cell is null, Stop Read Table Records");
                    return;
                }
                var keyValue = GetValue(keyCell);
                if (keyValue == string.Empty)
                {
                    //Debug.LogInfo($"Row {rowIndex} key cell is empty, Stop Read Table Records");
                    return;
                }

                var record = new ConfigRecord();
                configTable.RecordList.Add(record);



                foreach(var fieldInfo in configTable.FieldList)
                {
                    int columnindex = fieldInfo.ColumnIndex;
                    var cell = row.GetCell(columnindex);


                    string originValue = string.Empty; 
                    string value = string.Empty; 
                    if(cell !=null)
                        originValue = GetValue(cell);

                    value = originValue;
                    ///ImportDataFiled 会在 <see cref="Gen.CSharpGen.ImportFieldData"/> 里面再处理
                    if (value == string.Empty && fieldInfo.NeedImportData==false)
                        value= GetDefaultValue(fieldInfo.Type);


                    var recordColumn = new ConfigRecordFieldValue()
                    {
                        ColumnIndex = columnindex,
                        Cell = cell,
                        Value= value,
                        OriginValue = originValue,
                    };
                    record.RecordFieldList.Add(recordColumn);

                }
            }


        }


        public void ReadKVTableRecords(ConfigTable configTable, ISheet sheet, int rowIndexStart)
        {
            for (int rowIndex = rowIndexStart; rowIndex <= sheet.LastRowNum; ++rowIndex)
            {
                IRow row = sheet.GetRow(rowIndex);

                if (row == null)
                {
                    //Debug.LogInfo($"Row {rowIndex} is empty, Stop Read Table Records");
                    return;
                }

                if (row.FirstCellNum < 0 || row.LastCellNum < 0)
                {
                    //Debug.LogInfo($"Row {rowIndex} is empty, Stop Read Table Records");
                    return;
                }

                //0 是注释
                var commentCell = row.GetCell(0);
                string commentValue = string.Empty;
                if (commentCell != null)
                {
                    commentValue = GetValue(commentCell);
                }

                //1是字段类型
                var typeCell = row.GetCell(1);
                string typeValue = string.Empty;
                if (typeCell != null)
                {
                    typeValue = GetValue(typeCell);
                    if (string.IsNullOrEmpty(typeValue))
                    {
                        continue;
                    }
                }

                //2是字段名
                var nameCell = row.GetCell(2);
                string nameValue = string.Empty;
                if (nameCell != null)
                {
                    nameValue = GetValue(nameCell);
                    if (string.IsNullOrEmpty(typeValue))
                    {
                        continue;
                    }
                }

                //3是字段值
                var valueCell = row.GetCell(3);
                string valueValue = string.Empty;
                if (valueCell != null)
                {
                    valueValue = GetValue(valueCell);
                    if (string.IsNullOrEmpty(valueValue))
                    {
                        continue;
                    }
                }


                var record = new KVTableRecord()
                {
                    Comment = commentValue,
                    Type = typeValue,
                    Name = nameValue,
                    Value = valueValue,
                };

                configTable.KVRecordList.Add(record);





            }
        }


    }
}
