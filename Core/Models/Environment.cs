using System;

using Microsoft.Extensions.Configuration;

namespace DwFramework.Core
{
    public enum EnvironmentType
    {
        Develop = 0,
        Release = 1
    }

    public class Environment : IEnvironment
    {
        public EnvironmentType EnvironmentType { get; private set; }
        public IConfiguration Configuration { get; private set; }

        public Environment(EnvironmentType environmentType, IConfiguration configuration)
        {
            EnvironmentType = environmentType;
            Configuration = configuration;
        }

        public EnvironmentType GetEnvironmentType()
        {
            return EnvironmentType;
        }

        public IConfiguration GetConfiguration()
        {
            if (Configuration == null)
                throw new Exception("未读取到配置");
            return Configuration;
        }
    }
}
