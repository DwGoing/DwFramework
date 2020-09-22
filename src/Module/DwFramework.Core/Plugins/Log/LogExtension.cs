using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;

namespace DwFramework.Core.Plugins
{
    public static class LogExtension
    {
        /// <summary>
        /// 注册Log服务
        /// </summary>
        /// <param name="builder"></param>
        /// <returns></returns>
        public static ServiceHost RegisterLog(this ServiceHost host)
        {
            host.RegisterService(services => services.AddLogging(builder =>
            {
                builder.ClearProviders();
                builder.SetMinimumLevel(LogLevel.Trace);
                builder.AddNLog();
            }));
            return host;
        }

        /// <summary>
        /// 获取Log服务
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="provider"></param>
        /// <returns></returns>
        public static ILogger<T> GetLogger<T>(this IServiceProvider provider) => (ILogger<T>)provider.GetService(typeof(ILogger<T>));
    }
}
