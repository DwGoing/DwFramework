using System;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;
using System.IO;

namespace DwFramework.Core
{
    public static class ModuleManager
    {
        public abstract class ModuleInfo
        {
            public string Name { get; set; }
            public string Path { get; set; }
        }

        public sealed class ModuleInfo<T> : ModuleInfo
        {
            public T Instance { get; set; }
        }

        private static Dictionary<string, ModuleInfo> _loadedModules = new Dictionary<string, ModuleInfo>();

        /// <summary>
        /// 加载模块
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="path"></param>
        /// <returns></returns>
        private static T LoadModule<T>(string path) where T : IModule
        {
            if (!_loadedModules.ContainsKey(path))
            {
                if (!File.Exists(path)) throw new NotFoundException(path);
                var loadContext = new ModuleLoadContext(path);
                var assembly = loadContext.LoadFromAssemblyName(new AssemblyName(Path.GetFileNameWithoutExtension(path)));
                var targetType = typeof(T);
                var types = assembly.GetTypes().Where(item => targetType.IsAssignableFrom(item));
                if (types.Count() <= 0) throw new NotFoundException(targetType.Name);
                if (!(Activator.CreateInstance(types.First()) is T instance)) throw new TypeNotMatchException(types.First(), typeof(T));
                _loadedModules[path] = new ModuleInfo<T>()
                {
                    Name = targetType.Name,
                    Path = path,
                    Instance = instance
                };
                return instance;
            }
            else return ((ModuleInfo<T>)_loadedModules[path]).Instance;
        }

        /// <summary>
        /// 卸载所有模块(动态模块)
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