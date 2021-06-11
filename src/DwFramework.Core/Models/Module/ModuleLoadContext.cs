using System;
using System.Reflection;
using System.Runtime.Loader;

namespace DwFramework.Core
{
    public sealed class ModuleLoadContext : AssemblyLoadContext
    {
        private readonly AssemblyDependencyResolver _resolver;

        public ModuleLoadContext(string modulePath) : base(isCollectible: true)
        {
            _resolver = new AssemblyDependencyResolver(modulePath);
        }

        protected override Assembly Load(AssemblyName assemblyName)
        {
            string assemblyPath = _resolver.ResolveAssemblyToPath(assemblyName);
            if (assemblyPath != null) return LoadFromAssemblyPath(assemblyPath);
            return null;
        }

        protected override IntPtr LoadUnmanagedDll(string unmanagedDllName)
        {
            string libraryPath = _resolver.ResolveUnmanagedDllToPath(unmanagedDllName);
            if (libraryPath != null) return LoadUnmanagedDllFromPath(libraryPath);
            return IntPtr.Zero;
        }
    }
}
