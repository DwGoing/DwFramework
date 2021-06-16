using System;
using System.IO;
using System.Reflection;
using System.Collections.Generic;

namespace DwFramework.Core
{
    public static class ModuleManager
    {
        private sealed class ModuleInfo
        {
            public string Path { get; set; }
            public object Instance { get; set; }
        }

        private static Dictionary<Type, ModuleInfo> _loadedModules = new();

        /// <summary>
        /// 加载模块
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="modulePath"></param>
        /// <returns></returns>
        public static T LoadModule<T>(string modulePath)
        {
            var moduleType = typeof(T);
            if (_loadedModules.ContainsKey(moduleType)) return (T)_loadedModules[moduleType].Instance;
            if (File.Exists(modulePath))
            {
                var loadContext = new ModuleLoadContext(modulePath);
                var assembly = loadContext.LoadFromAssemblyName(new AssemblyName(Path.GetFileNameWithoutExtension(modulePath)));
                foreach (Type type in assembly.GetTypes())
                {
                    if (moduleType.IsAssignableFrom(type))
                    {
                        if (Activator.CreateInstance(type) is T instance)
                        {
                            _loadedModules[moduleType] = new ModuleInfo()
                            {
                                Path = modulePath,
                                Instance = instance
                            };
                            return instance;
                        }
                    }
                }
                throw new NotFoundException(moduleType.Name);
            }
            else throw new NotFoundException(modulePath);
        }

        /// <summary>
        /// 卸载模块
        /// </summary>
        public static void UnloadModule<T>()
        {
            var moduleType = typeof(T);
            if (!_loadedModules.ContainsKey(moduleType)) return;
            var loadContext = new ModuleLoadContext(_loadedModules[moduleType].Path);
            loadContext.Unloading += context => _loadedModules.Remove(moduleType);
            loadContext.Unload();
        }

        /// <summary>
        /// 卸载所有模块
        /// </summary>
        public static void UnloadAllModules()
        {
            foreach (var item in _loadedModules)
            {
                var loadContext = new ModuleLoadContext(item.Value.Path);
                loadContext.Unloading += context => _loadedModules.Remove(item.Key);
                loadContext.Unload();
            }
        }
    }
}