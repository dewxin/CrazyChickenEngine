using ConfigTool.Gen;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConfigTool
{
    class Program
    {

        static void Main(string[] args)
        {
            //TestExcelHelper();

            Console.WriteLine("ConfigTool: Start...");
            Bootstrap bootStrap = new Bootstrap();
            bootStrap.Start(args);

            Console.WriteLine("ConfigTool: End...");

        }

        static void TestExcelHelper()
        {
            var excelHelper = new ExcelReader();

            var dataTable = excelHelper.NpoiLoadExcel("com_item_info.xlsx");

            if(dataTable != null)
                Console.WriteLine(dataTable.Rows.Count);
        }
    }
}
