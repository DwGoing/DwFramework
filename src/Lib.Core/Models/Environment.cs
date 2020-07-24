using System;
using System.IO;
using System.Collections.Generic;

using Microsoft.Extensions.Configuration;

namespace DwFramework.Core
{
    public enum EnvironmentType
    {
        Develop = 0,
        Release = 1
    }

    public class Environment
    {
        private readonly EnvironmentType _environmentType;
        private readonly Dictionary<string, IConfiguration> _configurations = new Dictionary<string, IConfiguration>();

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="environmentType"></param>
        public Environment(EnvironmentType environmentType)
        {
            _environmentType = environmentType;
        }

        /// <summary>
        /// 获取环境类型
        /// </summary>
        /// <returns></returns>
        public EnvironmentType GetEnvironmentType()
        {
            return _environmentType;
        }

        /// <summary>
        /// 添加配置
        /// </summary>
        /// <param name="key"></param>
        /// <param name="configFilePath"></param>
        public void LoadConfiguration(string key, string configFilePath)
        {
            if (key == "ServiceHost") throw new Exception("ServiceHost为系统保留Key");
            if (File.Exists(configFilePath)) _configurations[key] = new ConfigurationBuilder().AddJsonFile(configFilePath).Build();
        }

        /// <summary>
        /// 获取配置
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public IConfiguration GetConfiguration(string key)
        {
            if (!_configurations.ContainsKey(key)) throw new Exception("未读取到配置");
            return _configurations[key];
        }
    }
}
