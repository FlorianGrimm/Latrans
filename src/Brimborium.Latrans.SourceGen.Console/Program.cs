using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Brimborium.Latrans.SourceGen {
    public class Program {
        public static async Task Main(string[] args) {
            string sln = @"G:\github\FlorianGrimm\Latrans\Brimborium.Latrans.sln";
            string csproj = @"G:\github\FlorianGrimm\Latrans\src\DemoWebApp.Logic\DemoWebApp.Logic.csproj";
            csproj = System.IO.Path.GetFullPath(csproj);
            Console.WriteLine($"SourceGen csproj:{csproj};");
            MSBuildLocatorHelper.Init();

            var workspace = MSBuildLocatorHelper.CreateWorkspace(null);

            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource(); ;
            var cancellationToken = cancellationTokenSource.Token;
            cancellationToken.ThrowIfCancellationRequested();
            System.Console.CancelKeyPress += delegate (object sender, ConsoleCancelEventArgs e) {
                cancellationTokenSource.Cancel();
            };
            var solution = await workspace.OpenSolutionAsync(sln, null, cancellationToken);
            var project = solution.Projects.Where(prj => string.Equals(prj.FilePath, csproj, StringComparison.OrdinalIgnoreCase)).FirstOrDefault();
            if (project is null) {
                System.Console.Error.WriteLine($"csproj: {csproj} not found.");
                return;
            } else {
                //var project = await workspace.OpenProjectAsync(csproj, null, cancellationToken);
                // project.AnalyzerReferences
                // project.ProjectReferences
                //foreach (var projectReference in project.ProjectReferences) {
                //    var projectId = projectReference.ProjectId;
                //}
                var inputCompilation = await project.GetCompilationAsync(cancellationToken);
                if (inputCompilation is null) {
                    //
                } else {
                    var generator = new ConfigureHandlersSourceGenerator();
                    GeneratorDriver driver = CSharpGeneratorDriver.Create(generator);
                    driver = driver.RunGeneratorsAndUpdateCompilation(inputCompilation, out var outputCompilation, out var diagnostics);
                    var lstDiagnostics = diagnostics.Where(d => d.Severity == DiagnosticSeverity.Error).ToList();
                    if (lstDiagnostics.Count > 0) {
                        foreach (var diagnostic in lstDiagnostics) {
                            System.Console.Error.WriteLine(diagnostic.GetMessage());
                        }
                    } else {
                    }
                }
            }
        }
    }
}
