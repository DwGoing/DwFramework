using System;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Primitives;

using DwFramework.Core.Extensions;

namespace DwFramework.Core
{
    public sealed class Environment
    {
        public readonly EnvironmentType EnvironmentType;

        private readonly Dictionary<string, ConfigurationBuilder> _configurationBuilders;
        private readonly Dictionary<string, IConfiguration> _configurations;


        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="environmentType"></param>
        /// <param name="configFilePath"></param>
        public Environment(EnvironmentType environmentType = EnvironmentType.Develop, string configFilePath = null)
        {
            EnvironmentType = environmentType;
            _configurationBuilders = new Dictionary<string, ConfigurationBuilder>
            {
                ["Global"] = new ConfigurationBuilder()
            };
            _configurations = new Dictionary<string, IConfiguration>();
            if (configFilePath != null) AddJsonConfig(configFilePath);
        }

        /// <summary>
        /// 添加配置
        /// </summary>
        /// <param name="configFilePath"></param>
        /// <param name="key"></param>
        /// <param name="onChange"></param>
        public void AddJsonConfig(string configFilePath, string key = null, Action onChange = null)
        {
            key ??= "Global";
            if (!_configurationBuilders.ContainsKey(key)) _configurationBuilders[key] = new ConfigurationBuilder();
            IConfigurationBuilder builder = _configurationBuilders[key];
            builder.AddJsonFile(configFilePath);
            if (onChange != null) ChangeToken.OnChange(() => builder.GetFileProvider().Watch(configFilePath), () => onChange());
        }

        /// <summary>
        /// 构建环境配置
        /// </summary>
        public void Build() => _configurationBuilders.ForEach(item => _configurations[item.Key] = item.Value.Build());

        /// <summary>
        /// 获取配置
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public IConfiguration GetConfiguration(string key = null)
        {
            key ??= "Global";
            key = _configurations.ContainsKey(key) ? key : "Global";
            return _configurations[key];
        }
    }
}
