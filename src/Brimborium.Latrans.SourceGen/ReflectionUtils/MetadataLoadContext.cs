// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

using Microsoft.CodeAnalysis;

namespace System.Reflection {
    public class MetadataLoadContext {
        private readonly Dictionary<string, IAssemblySymbol> _Assemblies = new Dictionary<string, IAssemblySymbol>(StringComparer.OrdinalIgnoreCase);

        private readonly Compilation _Compilation;

        private IAssemblySymbol? _CollectionsAssemblySymbol;

        public MetadataLoadContext(Compilation compilation) {
            this._Compilation = compilation;
            Dictionary<AssemblyName, IAssemblySymbol> assemblies = compilation.References
                .OfType<PortableExecutableReference>()
                .ToDictionary(
                    r => string.IsNullOrWhiteSpace(r.FilePath) ?
                        new AssemblyName(r.Display) : AssemblyName.GetAssemblyName(r.FilePath),
                    r => (IAssemblySymbol)compilation.GetAssemblyOrModuleSymbol(r)!);

            foreach (var item in assemblies) {
                string key = item.Key.Name;
                this._Assemblies[key] = item.Value!;

                if (this._CollectionsAssemblySymbol == null && key == "System.Collections") {
                    this._CollectionsAssemblySymbol = item.Value!;
                }
            }

            this.CoreAssembly = new AssemblyWrapper(compilation.GetTypeByMetadataName("System.Object")!.ContainingAssembly, this);
            this.MainAssembly = new AssemblyWrapper(compilation.Assembly, this);
        }

        public Type Resolve<T>() => this.Resolve(typeof(T))!;

        public Type? Resolve(Type type) {
            string assemblyName = type.Assembly.GetName().Name;
            IAssemblySymbol assemblySymbol;

            if (assemblyName == "System.Private.CoreLib" || assemblyName == "mscorlib" || assemblyName == "System.Runtime" || assemblyName == "System.Private.Uri") {
                Type? resolvedType = this.ResolveFromAssembly(type, this.CoreAssembly.Symbol);
                if (resolvedType != null) {
                    return resolvedType;
                }

                if (this._CollectionsAssemblySymbol != null && typeof(IEnumerable).IsAssignableFrom(type)) {
                    resolvedType = this.ResolveFromAssembly(type, this._CollectionsAssemblySymbol);
                    if (resolvedType != null) {
                        return resolvedType;
                    }
                }
            }

            CustomAttributeData? typeForwardedFrom = type.GetCustomAttributeData(typeof(TypeForwardedFromAttribute));
            if (typeForwardedFrom != null) {
                assemblyName = typeForwardedFrom.GetConstructorArgument<string>(0);
            }

            if (!this._Assemblies.TryGetValue(new AssemblyName(assemblyName).Name, out assemblySymbol)) {
                return null;
            }

            return this.ResolveFromAssembly(type, assemblySymbol);
        }

        private Type? ResolveFromAssembly(Type type, IAssemblySymbol assemblySymbol) {
            if (type.IsArray) {
                var typeSymbol = assemblySymbol.GetTypeByMetadataName(type.GetElementType().FullName);
                if (typeSymbol == null) {
                    return null!;
                }

                return this._Compilation.CreateArrayTypeSymbol(typeSymbol).AsType(this);
            }

            // Resolve the full name
            return assemblySymbol.GetTypeByMetadataName(type.FullName)!.AsType(this);
        }

        // TODO: this should be Assembly.
        private AssemblyWrapper CoreAssembly { get; }
        public Assembly MainAssembly { get; }

        internal Assembly LoadFromAssemblyName(string fullName) {
            if (this._Assemblies.TryGetValue(new AssemblyName(fullName).Name, out var assembly)) {
                return new AssemblyWrapper(assembly, this);
            }
            return null!;
        }
    }
}
