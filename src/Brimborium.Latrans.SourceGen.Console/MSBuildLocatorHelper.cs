// borrowed from roslyn AnalyzerRunner.AnalyzerRunnerHelper
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.Build.Locator;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Host;
using Microsoft.CodeAnalysis.MSBuild;

namespace Brimborium.Latrans.SourceGen {
    public class MSBuildLocatorHelper {
        public static void Init() {
            // QueryVisualStudioInstances returns Visual Studio installations on .NET Framework, and .NET Core SDK
            // installations on .NET Core. We use the one with the most recent version.
            var msBuildInstance = MSBuildLocator.QueryVisualStudioInstances().OrderByDescending(x => x.Version).First();

#if NETCOREAPP
            // Since we do not inherit msbuild.deps.json when referencing the SDK copy
            // of MSBuild and because the SDK no longer ships with version matched assemblies, we
            // register an assembly loader that will load assemblies from the msbuild path with
            // equal or higher version numbers than requested.
            LooseVersionAssemblyLoader.Register(msBuildInstance.MSBuildPath);
#endif

            MSBuildLocator.RegisterInstance(msBuildInstance);
        }

        public static MSBuildWorkspace CreateWorkspace(Dictionary<string, string>? properties) {
            if (properties is null) {
                properties = new Dictionary<string, string>();
            }

#if NETCOREAPP
            // This property ensures that XAML files will be compiled in the current AppDomain
            // rather than a separate one. Any tasks isolated in AppDomains or tasks that create
            // AppDomains will likely not work due to https://github.com/Microsoft/MSBuildLocator/issues/16.
            properties["AlwaysCompileMarkupFilesInSeparateDomain"] = bool.FalseString;
#endif
            // Use the latest language version to force the full set of available analyzers to run on the project.
            properties["LangVersion"] = "latest";

            return MSBuildWorkspace.Create(properties);
        }
    }
}
