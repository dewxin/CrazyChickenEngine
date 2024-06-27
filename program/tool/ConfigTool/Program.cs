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
            Debug.Log += Console.WriteLine;
            Debug.LogInfo += Console.WriteLine;
            Debug.LogWarning += Console.WriteLine;
            Debug.LogError += Console.WriteLine;

            //TestExcelHelper();



            Start(args);
        }

        private static void Start(string[] args)
        {
            Console.WriteLine("ConfigTool: Start...");
            Bootstrap bootStrap = new Bootstrap();
            bootStrap.Start(args);

            Console.WriteLine("ConfigTool: End...");
        }

        static void TestExcelHelper()
        {
            var configTableList = ExcelReader.ReadTablesFromFile("MonsterConfig.xlsx");


        }
    }
}
