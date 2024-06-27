using ConfigTool.ConfigInfo;
using NPOI.HSSF.UserModel;
using NPOI.SS.Formula.Functions;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConfigTool
{
    public partial class ExcelReader
    {
        private string absoluteFileName;
        private string fileName => Path.GetFileName(absoluteFileName);
        private XSSFWorkbook workbook;
        private List<ISheet> sheetList = new List<ISheet>();
        private XSSFFormulaEvaluator excelEvaluator = null;

        private List<ConfigTable> resultTableList = new List<ConfigTable>();


        public static List<ConfigTable> ReadTablesFromFile(string fileName)
        {

            var excelReader = new ExcelReader();

            excelReader.InitWorkbook(fileName);

            excelReader.ReadTableFromSheets();

            return excelReader.resultTableList;

        }

        private bool InitWorkbook(string fileName)
        {
            var fileStream = new FileStream(fileName, FileMode.Open, FileAccess.Read);
            this.absoluteFileName = fileStream.Name;

            if (fileName.IndexOf(".xlsx") > 0) // 2007版本
            {
                workbook = new XSSFWorkbook(fileStream);
                excelEvaluator = new XSSFFormulaEvaluator(workbook);
            }
            else
            {
                return false;
            }


            var sheetCount = workbook.NumberOfSheets;

            for (int i = 0; i < sheetCount; ++i)
            {
                var sheet = workbook.GetSheetAt(i);
                sheetList.Add(sheet);
            }

            return true;
        }

        private void ReadTableFromSheets()
        {
            foreach (var sheet in sheetList)
            {

                ReadConfigTablesFrom1Sheet(sheet);
            }

            //LogResultTableList();
        }


        private void ReadConfigTablesFrom1Sheet(ISheet sheet)
        {
            int rowIndex = 0;
            while(TryLocateTableHeader(sheet, rowIndex, out ICell tableHeaderCell))
            {
                var configTable = ReadTable(tableHeaderCell);
                Debug.Log($">>>>> Locate table {configTable.Name} In {fileName}.{sheet.SheetName} <<<<<");
                configTable.SourceFile = absoluteFileName;
                resultTableList.Add(configTable);

                //Debug.LogInfo(configTable.ToString());

                rowIndex =tableHeaderCell.RowIndex+ 3 + configTable.RecordList.Count();
            }
        }

        private void LogResultTableList()
        {
            Debug.LogInfo("Print Config Table");
            foreach(var resultTable in resultTableList)
            {
                Debug.LogInfo(resultTable.ToString());
            }

        }

        private string GetDefaultValue(string type)
        {
            type = type.ToLower();

            switch (type)
            {
                case "int":
                case "long":
                case "float":
                case "double":
                    return "0";
            }

            return "";
        }


        private string GetValue(ICell cell)
        {
            if (cell.CellType == CellType.Numeric)
            {
                return cell.NumericCellValue.ToString();
            }
            else if (cell.CellType == CellType.String)
            {
                return cell.StringCellValue.ToString();
            }
            else if (cell.CellType == CellType.Blank)
            {
                return string.Empty;
            }
            else if(cell.CellType == CellType.Formula)
            {
                if (excelEvaluator.Evaluate(cell).CellType == CellType.Numeric)
                {
                    return excelEvaluator.Evaluate(cell).NumberValue.ToString();
                }
                else
                {
                    return excelEvaluator.Evaluate(cell).StringValue;
                }
            }
            else if(cell.CellType == CellType.Boolean)
            {
                return cell.BooleanCellValue.ToString().ToLower();
            }

            return cell.StringCellValue;
        }



    }
}
