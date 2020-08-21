using System;
using System.Collections.Generic;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Primitives;

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
        private readonly ConfigurationBuilder _configurationBuilder;
        private readonly List<Action> _configWatch;

        public IConfiguration Configuration { get; private set; }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="environmentType"></param>
        /// <param name="configFilePath"></param>
        public Environment(EnvironmentType environmentType, string configFilePath = null)
        {
            _environmentType = environmentType;
            _configurationBuilder = new ConfigurationBuilder();
            _configWatch = new List<Action>();
            if (configFilePath != null) AddJsonConfig(configFilePath);
        }

        /// <summary>
        /// 添加配置
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="onChange"></param>
        public void AddJsonConfig(string fileName, Action onChange = null)
        {
            _configurationBuilder.AddJsonFile(fileName);
            if (onChange != null) ChangeToken.OnChange(() => _configurationBuilder.GetFileProvider().Watch(fileName), () => onChange());
        }

        /// <summary>
        /// 构建环境配置
        /// </summary>
        public void Build() => Configuration = _configurationBuilder.Build();

        /// <summary>
        /// 获取环境类型
        /// </summary>
        /// <returns></returns>
        public EnvironmentType GetEnvironmentType()
        {
            return _environmentType;
        }
    }
}
