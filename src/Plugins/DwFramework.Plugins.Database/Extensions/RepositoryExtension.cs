using System;

using DwFramework.Core;
using DwFramework.Database;

namespace DwFramework.Extensions.Database
{
    public static class RepositoryExtension
    {
        /// <summary>
        /// 注册服务
        /// </summary>
        /// <param name="host"></param>
        public static void RegisterRepositories(this ServiceHost host)
        {
            host.RegisterDatabaseService();
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            foreach (var assembly in assemblies)
            {
                var types = assembly.GetTypes();
                foreach (var type in types)
                {
                    if (type.BaseType != null && type.BaseType.ToString().Contains("BaseRepository"))
                        host.RegisterType(type);
                }
            }
        }
    }
}
