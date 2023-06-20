using System;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoSerializer
{
    internal partial class TemplateHandlerBase:IDisposable
    {
        protected Dictionary<string, List<string>> name2TemplateDict = new Dictionary<string, List<string>>();

        public StreamReader reader;


        public void GetTemplate<T>()
            where T : class
        {
            var stream = typeof(T).Assembly.GetManifestResourceStream($"AutoSerializer.CodeGen._template.{typeof(T).Name}.cs");

            reader = new StreamReader(stream);
        }

        public void GetTemplate(List<string> templateList)
        {
            var stream = new MemoryStream();
            var writer = new StreamWriter(stream);
            templateList.ForEach(line => writer.WriteLine(line));

            reader = new StreamReader(stream);
        }

        public void HandleTemplate()
        {
            string line;
            while (!(line = reader.ReadLine()).Contains("#endregion"))
            {
                HandleTemplate(line);
            }
        }

        protected abstract void HandleTemplate(string line);

        protected void TryAddTemplate(string line, string templateName)
        {
            if (line.Contains(templateName))
            {
                var codeLines = ReadUntilEndif();
                name2TemplateDict.Add(templateName, codeLines);
            }
        }


        public string ReadLine()
        {
            return reader.ReadLine();
        }

        public List<string> ReadUntilEndif()
        {
            return ReadUntil("#endif");
        }

        public List<string> ReadUntilEndregion()
        {
            return ReadUntil("#endregion");
        }


        public List<string> ReadUntil(string str)
        {
            List<string> lineList = new List<string>();

            string oneLine;
            while (!(oneLine = reader.ReadLine()).Contains(str))
            {
                lineList.Add(oneLine);
            }
            return lineList;
        }

        public void Dispose()
        {
            if(reader!=null)
            {
                reader.Dispose();
            }
        }
    }
}
