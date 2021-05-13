using System;
using System.IO;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;

using DwFramework.Core.Extensions;

namespace DwFramework.Core
{
    public sealed class Environment
    {
        private readonly Dictionary<string, ConfigurationBuilder> _configurationBuilders;
        private readonly Dictionary<string, IConfiguration> _configurations;

        public EnvironmentType EnvironmentType { get; init; }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="environmentType"></param>
        public Environment(EnvironmentType environmentType = EnvironmentType.Develop)
        {
            EnvironmentType = environmentType;
            _configurationBuilders = new Dictionary<string, ConfigurationBuilder>
            {
                ["Global"] = new ConfigurationBuilder()
            };
            _configurations = new Dictionary<string, IConfiguration>();
        }

        /// <summary>
        /// 添加配置
        /// </summary>
        /// <param name="configHandler"></param>
        /// <param name="key"></param>
        public void AddConfig(Action<IConfigurationBuilder> configHandler, string key = null)
        {
            key ??= "Global";
            if (!_configurationBuilders.ContainsKey(key)) _configurationBuilders[key] = new ConfigurationBuilder();
            configHandler?.Invoke(_configurationBuilders[key]);
        }

        /// <summary>
        /// 添加配置
        /// </summary>
        /// <param name="configFilePath"></param>
        /// <param name="key"></param>
        public void AddJsonConfig(string configFilePath, string key = null)
        {
            AddConfig(builder => builder.AddJsonFile(configFilePath), key);
        }

        /// <summary>
        /// 添加配置
        /// </summary>
        /// <param name="configStream"></param>
        /// <param name="key"></param>
        public void AddJsonConfig(Stream configStream, string key = null)
        {
            AddConfig(builder => builder.AddJsonStream(configStream), key);
        }

        /// <summary>
        /// 构建环境配置
        /// </summary>
        public void Build()
        {
            _configurations.Clear();
            _configurationBuilders.ForEach(item => _configurations[item.Key] = item.Value.Build());
            _configurationBuilders.Clear();
        }

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

        /// <summary>
        /// 获取配置
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="path"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public T GetConfiguration<T>(string path = null, string key = null)
        {
            return GetConfiguration(key).GetConfig<T>(path);
        }
    }
}
