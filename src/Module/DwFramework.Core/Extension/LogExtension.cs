using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;

namespace DwFramework.Core
{
    public static class LogExtension
    {
        /// <summary>
        /// 注册NLog服务
        /// </summary>
        /// <param name="host"></param>
        /// <returns></returns>
        public static ServiceHost RegisterNLog(this ServiceHost host)
        {
            host.ConfigureLogging((context, builder) =>
            {
                builder.ClearProviders();
                builder.SetMinimumLevel(LogLevel.Trace);
                builder.AddNLog();
            });
            return host;
        }

        /// <summary>
        /// 注册NLog服务
        /// </summary>
        /// <param name="host"></param>
        /// <param name="configuration"></param>
        /// <returns></returns>
        public static ServiceHost RegisterNLog(this ServiceHost host, IConfiguration configuration)
        {
            host.ConfigureLogging((context, builder) =>
            {
                builder.ClearProviders();
                builder.SetMinimumLevel(LogLevel.Trace);
                builder.AddNLog(configuration);
            });
            return host;
        }

        /// <summary>
        /// 注册NLog服务
        /// </summary>
        /// <param name="host"></param>
        /// <param name="configPath"></param>
        /// <returns></returns>
        public static ServiceHost RegisterNLog(this ServiceHost host, string configPath)
        {
            host.ConfigureLogging((context, builder) =>
            {
                builder.ClearProviders();
                builder.SetMinimumLevel(LogLevel.Trace);
                builder.AddNLog(configPath);
            });
            return host;
        }
    }
}
