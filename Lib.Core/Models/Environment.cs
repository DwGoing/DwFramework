using System;
using System.IO;

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
        private readonly EnvironmentType _environmentType;
        private readonly IConfiguration _configuration;

        public readonly string ConfigFilePath;

        public Environment(EnvironmentType environmentType, string configFilePath = null)
        {
            _environmentType = environmentType;
            if (configFilePath != null && File.Exists(configFilePath))
            {
                ConfigFilePath = configFilePath;
                _configuration = new ConfigurationBuilder().AddJsonFile(configFilePath).Build();
            }
        }

        public EnvironmentType GetEnvironmentType()
        {
            return _environmentType;
        }

        public IConfiguration GetConfiguration()
        {
            if (_configuration == null)
                throw new Exception("未读取到配置");
            return _configuration;
        }
    }
}
