using System;

using DwFramework.Core;

namespace DwFramework.Kafka
{
    public sealed class KafkaService
    {
        public class Config
        {

        }

        private readonly Config _config;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="environment"></param>
        /// <param name="configKey"></param>
        public KafkaService(Core.Environment environment, string configKey = null)
        {
            var configuration = environment.GetConfiguration(configKey ?? "Kafka");
            _config = configuration.GetConfig<Config>(configKey);
            if (_config == null) throw new Exception("未读取到Kafka配置");
        }
    }
}
