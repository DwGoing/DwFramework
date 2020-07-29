using System.IO;

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
        private readonly ConfigurationBuilder _configurationBuilder;

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
            AddJsonConfig(configFilePath);
        }

        /// <summary>
        /// 添加配置
        /// </summary>
        /// <param name="configFilePath"></param>
        public void AddJsonConfig(string configFilePath)
        {
            if (configFilePath != null && File.Exists(configFilePath)) _configurationBuilder.AddJsonFile(configFilePath);
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
