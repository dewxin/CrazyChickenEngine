using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConfigTool
{
    internal class ExcelReader
    {
        private ISheet sheet;
        private NPOI.SS.Formula.BaseFormulaEvaluator excelEvaluator = null;

        private bool PrepareSucceed(String fileName, string sheetName = "sheet1")
        {
            var fileStream = new FileStream(fileName, FileMode.Open, FileAccess.Read);
            IWorkbook workbook = null;

            if (fileName.IndexOf(".xlsx") > 0) // 2007版本
            {
                workbook = new XSSFWorkbook(fileStream);
                excelEvaluator = new XSSFFormulaEvaluator(workbook);
            }
            else if (fileName.IndexOf(".xls") > 0) // 2003版本
            {
                workbook = new HSSFWorkbook(fileStream);
                excelEvaluator = new HSSFFormulaEvaluator(workbook);
            }
            else
            {
                return false;
            }

            if (!string.IsNullOrEmpty(sheetName))
                sheet = workbook.GetSheet(sheetName);

            if (sheet == null) //如果没有找到指定的sheetName对应的sheet，则尝试获取第一个sheet                
                sheet = workbook.GetSheetAt(0);

            return sheet != null;
        }


        private DataRow ReadOneRow(IRow row, DataTable dataTable, string fileName, out bool rowIsEmpty)
        {
            DataRow dataRow = dataTable.NewRow();

            rowIsEmpty = true; 

            for (int j = row.FirstCellNum; j < dataTable.Columns.Count; j++)
            {
                if (row.GetCell(j) == null)
                {
                    //dataRow[j] = string.Empty;
                    continue;
                }

                switch (row.GetCell(j).CellType)
                {
                    case CellType.Numeric:
                        if (HSSFDateUtil.IsCellDateFormatted(row.GetCell(j)))//日期类型
                        {
                            dataRow[j] = row.GetCell(j).DateCellValue.ToString("yyyy-MM-dd HH:mm:ss");
                        }
                        else//其他数字类型
                        {
                            dataRow[j] = row.GetCell(j).NumericCellValue.ToString();
                        }
                        break;
                    case CellType.Blank:
                        dataRow[j] = string.Empty;
                        break;
                    case CellType.Formula:   //此处是处理公式数据，获取公式执行后的值
                        if (Path.GetExtension(fileName).ToLower().Trim() == ".xlsx")
                        {
                            //XSSFFormulaEvaluator eva = new XSSFFormulaEvaluator(workbook);
                            if (excelEvaluator.Evaluate(row.GetCell(j)).CellType == CellType.Numeric)
                            {
                                dataRow[j] = excelEvaluator.Evaluate(row.GetCell(j)).NumberValue.ToString();
                            }
                            else
                            {
                                dataRow[j] = excelEvaluator.Evaluate(row.GetCell(j)).StringValue;
                            }
                        }
                        else
                        {
                            //HSSFFormulaEvaluator eva = new HSSFFormulaEvaluator(workbook);
                            if (excelEvaluator.Evaluate(row.GetCell(j)).CellType == CellType.Numeric)
                            {
                                dataRow[j] = excelEvaluator.Evaluate(row.GetCell(j)).NumberValue.ToString();
                            }
                            else
                            {
                                dataRow[j] = excelEvaluator.Evaluate(row.GetCell(j)).StringValue;
                            }
                        }
                        break;
                    case CellType.Boolean:
                        {
                            dataRow[j] = row.GetCell(j).BooleanCellValue.ToString().ToLower();
                        }
                        break;
                    default:
                        dataRow[j] = row.GetCell(j).StringCellValue;
                        break;

                }

                if (dataRow[j] != null && !string.IsNullOrEmpty(dataRow[j].ToString().Trim()))
                {
                    rowIsEmpty = false;
                }
            }

            return dataRow;
        }

        public DataTable NpoiLoadExcel(String fileName, string sheetName = "sheet1")
        {
            if (Path.GetFileName(fileName).StartsWith("~$"))
                return null;

            if (!PrepareSucceed(fileName, sheetName))
                return null;

            DataTable dataTable = new DataTable();
            dataTable.TableName = Path.GetFileNameWithoutExtension(fileName);

            try
            {
                IRow firstRow = sheet.GetRow(0);

                for (int i = firstRow.FirstCellNum; i < firstRow.LastCellNum; ++i)
                {
                    ICell cell = firstRow.GetCell(i);
                    if (cell != null)
                    {
                        string cellValue = cell.StringCellValue;
                        if (!string.IsNullOrEmpty(cellValue))
                        {
                            DataColumn column = new DataColumn(cellValue);
                            dataTable.Columns.Add(column);
                        }
                    }
                    else
                    {
                        DataColumn column = new DataColumn("null");
                        dataTable.Columns.Add(column);
                    }
                }

                int cellCount = dataTable.Columns.Count;

                int startRowIndex = 1;
                int endRowIndex = sheet.LastRowNum;
                for(int i = startRowIndex; i <= endRowIndex; ++i)
                {
                    IRow row = sheet.GetRow(i);
                    // 空行之后的数据无视
                    if (row == null)
                        break;

                    bool rowIsEmpty;
                    DataRow dataRow = ReadOneRow(row, dataTable, fileName, out rowIsEmpty);

                    if (!rowIsEmpty)
                        dataTable.Rows.Add(dataRow);

                }

                return dataTable;
            }
            catch(Exception ex)
            {
                Console.WriteLine(string.Format("读取{0}出错，{1}", fileName, ex.Message));
                return null;
            }
        }
    }
}
