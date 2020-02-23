using System;

using DwFramework.Core;
using DwFramework.Core.Extensions;

namespace DwFramework.Database.Extensions
{
    public static class RepositoryExtension
    {
        public static void RegisterRepositories(this ServiceHost host)
        {
            host.RegisterType<IDatabaseService, DatabaseService>().SingleInstance();
        }
    }
}
