namespace DwFramework.Core
{
    public abstract class ConfigableService
    {
        /// <summary>
        /// 读取配置
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="path"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        protected T ReadConfig<T>(string path = null, string key = null)
        {
            var config = ServiceHost.Environment.GetConfiguration<T>(path, key);
            return config;
        }
    }
}
