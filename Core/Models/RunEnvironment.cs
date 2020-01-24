using System;

using Microsoft.Extensions.Configuration;

namespace DwFramework.Core
{
    public enum EnvironmentType
    {
        UNKNOW = 0,
        Develop = 1,
        Produce = 2
    }

    public interface IRunEnvironment
    {
        EnvironmentType GetEnvironmentType();
        IConfiguration GetConfiguration();
    }

    public class RunEnvironment : IRunEnvironment
    {
        public EnvironmentType EnvironmentType { get; private set; }
        public IConfiguration Configuration { get; private set; }

        public RunEnvironment(EnvironmentType environmentType, IConfiguration configuration)
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
