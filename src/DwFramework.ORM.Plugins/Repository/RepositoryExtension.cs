using System;
using System.Reflection;

using DwFramework.Core;

namespace DwFramework.ORM.Plugins
{
    public static class RepositoryExtension
    {
        /// <summary>
        /// 注册仓储
        /// </summary>
        /// <param name="host"></param>
        public static void RegisterRepositories(this ServiceHost host)
        {
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            assemblies.ForEach(assembly =>
            {
                var types = assembly.GetTypes();
                types.ForEach(type =>
                {
                    var attr = type.GetCustomAttribute<RepositoryAttribute>();
                    if (attr == null) return;
                    host.RegisterType(type);
                });
            });
        }
    }
}
