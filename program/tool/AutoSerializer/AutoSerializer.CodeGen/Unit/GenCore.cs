using AutoSerializer.CodeGen.CodeGen;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace AutoSerializer
{
    public class GenCore
    {
        public static GenCore Instance = new GenCore();

        public void Work(Assembly sourceAssembly, string outputDir)
        {
            RefTypeCodeCache.Reset();
            ValueTypeCodeCache.Reset();

            //生成值类型的代码
            ValueTypeCodeGen.Instance = new ValueTypeCodeGen();
            ValueTypeCodeGen.Instance.CollectValueTypeCode(sourceAssembly);

            //生成引用类型的代码
            RefTypeCodeGen.Instance = new RefTypeCodeGen();
            RefTypeCodeGen.Instance.CollectRefTypeCode(sourceAssembly);

            FuncParamCodeGen.Instance.CollectParamTypeCode(sourceAssembly);

            //获取注册的代码
            RefTypeCodeGen.Instance.GetSerializerCenterCode();

            //生成集合类型的代码

            OutputRefTypeCode(outputDir);

        }


        private void GenerateGroupCode()
        {

        }

        [Obsolete]
        public void OutputRefTypeCode(string outputDir)
        {
            if (!File.Exists(outputDir))
            {
                Directory.CreateDirectory(outputDir);
            }

            foreach (var codeCache in RefTypeCodeCache.Dict.Values)
            {
                var fileName = Path.Combine(outputDir, codeCache.SerializerName + ".cs");
                File.WriteAllLines(fileName, codeCache.CodeLines);
            }

            var centerFileName = Path.Combine(outputDir, typeof(SerializerCenter).Name + ".cs");
            File.WriteAllLines(centerFileName, RefTypeCodeCache.SerializerCenter.CodeLines);


        }

        [Obsolete]
        public void OutputValueTypeCode(string outputDir)
        {
            if(!File.Exists(outputDir)) { 
                Directory.CreateDirectory(outputDir);
            }

            foreach (var codeCache in ValueTypeCodeCache.Dict.Values)
            {
                var fileName = Path.Combine(outputDir, codeCache.Type.Name+".cs");

                var codeList = new List<string>();
                codeList.Add("Serializer:");
                codeList.AddRange(codeCache.SerializerCodeLines);

                codeList.Add(" ");
                codeList.Add(" ");
                codeList.Add("DeSerializer:");
                codeList.AddRange(codeCache.DeserializerCodeLines);


                File.WriteAllLines(fileName, codeList);

            }


        }



    }
}
