using System;
using System.Reflection;
using Autofac;
using DwFramework.Core;

namespace DwFramework.SqlSugar.Repository
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
            foreach (var assembly in assemblies)
            {
                var types = assembly.GetTypes();
                foreach (var type in types)
                {
                    var attr = type.GetCustomAttribute<RepositoryAttribute>();
                    if (attr == null) return;
                    host.ConfigureContainer(builder => builder.RegisterType(type));
                };
            };
        }
    }
}
