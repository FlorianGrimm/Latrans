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
                return;
            } else {
                //var project = await workspace.OpenProjectAsync(csproj, null, cancellationToken);
                // project.AnalyzerReferences
                // project.ProjectReferences
                //foreach (var projectReference in project.ProjectReferences) {
                //    var projectId = projectReference.ProjectId;
                //}
                var compilation = await project.GetCompilationAsync(cancellationToken);
                // compilation.D
                // compilation.GetSemanticModel(project.c)
            }
        }
    }
}
