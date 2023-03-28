#if NET6_0_OR_GREATER
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.IO;
using System.Reflection;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Emit;
using Block.Assorted;
using System.Diagnostics;

namespace Block.Server.Common
{
    public static class RoslynCompiler
    {
        public static Assembly GenerateAssembly(List<string> codesArray)
        {

            List<SyntaxTree> syntaxTreeList = new List<SyntaxTree>();
            foreach(var code in codesArray)
            {
                var tree = CSharpSyntaxTree.ParseText(code);
                syntaxTreeList.Add(tree);
            }

            string assemblyName = Path.GetRandomFileName();
            List<MetadataReference> references = new List<MetadataReference>()
            {
                //MetadataReference.CreateFromFile(typeof(object).Assembly.Location),
                //MetadataReference.CreateFromFile(typeof(List<>).Assembly.Location),
            };


            AppDomain.CurrentDomain.GetAssemblies().ToList()
            .ForEach(a => references.Add(MetadataReference.CreateFromFile(a.Location)));


            CSharpCompilation compilation = CSharpCompilation.Create(
                assemblyName,
                syntaxTrees: syntaxTreeList,
                references: references,
                options: new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

            var ms = new MemoryStream();
            {
                EmitResult result = compilation.Emit(ms);

                if (!result.Success)
                {
                    IEnumerable<Diagnostic> failures = result.Diagnostics.Where(diagnostic =>
                        diagnostic.IsWarningAsError ||
                        diagnostic.Severity == DiagnosticSeverity.Error);

                    foreach (Diagnostic diagnostic in failures)
                    {
                        Console.Error.WriteLine("{0}: {1}", diagnostic.Id, diagnostic.GetMessage());
                    }

                    throw new ApplicationException("Compile Code fail");
                    return null;
                }


                ms.Seek(0, SeekOrigin.Begin);
                Assembly assembly = Assembly.Load(ms.ToArray());
                return assembly;
            }


        }
    }

}

#endif
