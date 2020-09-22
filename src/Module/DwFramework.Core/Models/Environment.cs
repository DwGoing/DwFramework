using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Primitives;

namespace DwFramework.Core
{
    public class Environment
    {
        public readonly EnvironmentType EnvironmentType;
        public IConfiguration Configuration { get; private set; }

        private readonly ConfigurationBuilder _configurationBuilder;


        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="environmentType"></param>
        /// <param name="configFilePath"></param>
        public Environment(EnvironmentType environmentType = EnvironmentType.Develop, string configFilePath = null)
        {
            EnvironmentType = environmentType;
            _configurationBuilder = new ConfigurationBuilder();
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
    }
}
