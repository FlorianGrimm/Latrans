// borrowed from roslyn AnalyzerRunner.LooseVersionAssemblyLoader

#if NETCOREAPP

using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.Loader;

namespace Brimborium.Latrans.SourceGen {
    internal static class LooseVersionAssemblyLoader {
        private static readonly Dictionary<string, Assembly> _PathsToAssemblies = new Dictionary<string, Assembly>(StringComparer.OrdinalIgnoreCase);
        private static readonly Dictionary<string, Assembly> _NamesToAssemblies = new Dictionary<string, Assembly>();

        private static readonly object _Guard = new object();
        private static readonly string[] _Extensions = new[] { "ni.dll", "ni.exe", "dll", "exe" };

        /// <summary>
        /// Register an assembly loader that will load assemblies with higher version than what was requested.
        /// </summary>
        public static void Register(string searchPath) {
            AssemblyLoadContext.Default.Resolving += (AssemblyLoadContext context, AssemblyName assemblyName) => {
                lock (_Guard) {
                    if (_NamesToAssemblies.TryGetValue(assemblyName.FullName, out var assembly)) {
                        return assembly;
                    }

                    return TryResolveAssemblyFromPaths_NoLock(context, assemblyName, searchPath);
                }
            };
        }

        private static Assembly? TryResolveAssemblyFromPaths_NoLock(
                AssemblyLoadContext context, 
                AssemblyName assemblyName, 
                string searchPath
            ) {
            foreach (var cultureSubfolder in string.IsNullOrEmpty(assemblyName.CultureName)
                // If no culture is specified, attempt to load directly from
                // the known dependency paths.
                ? new[] { string.Empty }
                // Search for satellite assemblies in culture subdirectories
                // of the assembly search directories, but fall back to the
                // bare search directory if that fails.
                : new[] { assemblyName.CultureName, string.Empty }) {
                foreach (var extension in _Extensions) {
                    var candidatePath = Path.Combine(
                        searchPath, cultureSubfolder, $"{assemblyName.Name}.{extension}");

                    var isAssemblyLoaded = _PathsToAssemblies.ContainsKey(candidatePath);
                    if (isAssemblyLoaded || !File.Exists(candidatePath)) {
                        continue;
                    }

                    var candidateAssemblyName = AssemblyLoadContext.GetAssemblyName(candidatePath);
                    if (candidateAssemblyName.Version < assemblyName.Version) {
                        continue;
                    }

                    return LoadAndCache_NoLock(context, candidatePath);
                }
            }

            return null;
        }

        private static Assembly LoadAndCache_NoLock(AssemblyLoadContext context, string fullPath) {
            var assembly = context.LoadFromAssemblyPath(fullPath);
            var name = assembly.FullName;

            _PathsToAssemblies[fullPath] = assembly;
            if (name is object) { 
                _NamesToAssemblies[name] = assembly;
            }

            return assembly;
        }
    }
}

#endif
