using ConfigTool.ConfigInfo;
using NPOI.SS.UserModel;
using System;
using System.Collections.Generic;
using System.Linq;
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
            ReadTableColumn(configTable, tableHeaderCell.Sheet, tableHeaderCell.RowIndex + 2);
            ReadTableRecords(configTable, tableHeaderCell.Sheet, tableHeaderCell.RowIndex + 3);

            return configTable;
        }

        private void ReadTableHeader(ConfigTable configTable, ICell tableHeaderCell)
        {
            string cellValue = GetValue(tableHeaderCell);

            var entryList = cellValue.Split(ConfigKeyWord.Seperator);
            foreach(var entry in entryList)
            {
                if (entry.Contains(ConfigKeyWord.TableMarker))
                    continue;
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

                if(key != ConfigKeyWord.TableName && key != ConfigKeyWord.TableRefClassFile)
                {
                    Debug.LogError($"{key} in {tableHeaderCell.Address} is not valid");
                }

                if(key == ConfigKeyWord.TableName)
                {
                    configTable.Name = value;
                }

                if(key == ConfigKeyWord.TableRefClassFile)
                {
                    var fileList = value.Split(',');
                    foreach(var file in fileList)
                    {
                        var fileTrim = file.Trim();
                        if (fileTrim == string.Empty)
                            continue;

                        configTable.ReferenceClassFileList.Add(fileTrim);
                    }
                }

            }

            if(configTable.Name == string.Empty)
            {
                Debug.LogError($"tableName is Empty File={absoluteFileName} ");
            }


        }

        private void ReadTableColumn(ConfigTable configTable,  ISheet sheet, int rowIndexStart)
        {
            IRow row = sheet.GetRow(rowIndexStart);


            for (int i = row.FirstCellNum; i <= row.LastCellNum; ++i)
            {
                var cell = row.GetCell(i);

                if (cell == null)
                    continue;

                string cellValue = GetValue(cell);
                if (cellValue == null || cellValue.Trim() == string.Empty)
                    continue;

                var entryList = cellValue.Split(ConfigKeyWord.Seperator);

                var configTableColumn = new ConfigTableField();
                configTableColumn.ColumnIndex = i;
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


                    string cellValue = string.Empty; 
                    if(cell !=null)
                        cellValue = GetValue(cell);
                    if(cellValue == string.Empty)
                        cellValue= GetDefaultValue(fieldInfo.Type);


                    var recordColumn = new ConfigRecordFieldValue()
                    {
                        ColumnIndex = columnindex,
                        Value = cellValue,
                    };
                    record.RecordFieldList.Add(recordColumn);

                }
            }


        }


    }
}
