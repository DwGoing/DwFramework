using System;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Primitives;

using DwFramework.Core.Extensions;

namespace DwFramework.Core
{
    public class Environment
    {
        public readonly EnvironmentType EnvironmentType;
        public IConfiguration Configuration { get; private set; }

        private readonly Dictionary<string, ConfigurationBuilder> _configurationBuilders;
        private readonly ConfigurationBuilder _globalConfigurationBuilder;
        private readonly Dictionary<string, IConfiguration> _configurations;


        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="environmentType"></param>
        /// <param name="configFilePath"></param>
        public Environment(EnvironmentType environmentType = EnvironmentType.Develop, string configFilePath = null)
        {
            EnvironmentType = environmentType;
            _configurationBuilders = new Dictionary<string, ConfigurationBuilder>();
            _globalConfigurationBuilder = new ConfigurationBuilder();
            _configurationBuilders["Global"] = _globalConfigurationBuilder;
            _configurations = new Dictionary<string, IConfiguration>();
            if (configFilePath != null) AddJsonConfig(configFilePath);
        }

        /// <summary>
        /// 添加配置
        /// </summary>
        /// <param name="configFilePath"></param>
        /// <param name="key"></param>
        /// <param name="onChange"></param>
        public void AddJsonConfig(string configFilePath, string key = "Global", Action onChange = null)
        {
            var builder = _globalConfigurationBuilder;
            if (key != "Global" && _configurationBuilders.ContainsKey(key)) builder = _configurationBuilders[key];
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
        public IConfiguration GetConfiguration(string key = "Global")
        {
            if (!_configurations.ContainsKey(key)) return null;
            return _configurations[key];
        }
    }
}
