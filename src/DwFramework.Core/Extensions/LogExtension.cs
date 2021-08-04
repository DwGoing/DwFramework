using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;

namespace DwFramework.Core
{
    public static class LogExtension
    {
        /// <summary>
        /// 注册NLog服务
        /// </summary>
        /// <param name="builder"></param>
        /// <returns></returns>
        public static ILoggingBuilder UserNLog(this ILoggingBuilder builder)
        {
            builder.ClearProviders();
            builder.SetMinimumLevel(LogLevel.Trace);
            builder.AddNLog();
            return builder;
        }

        /// <summary>
        /// 注册NLog服务
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="configuration"></param>
        /// <returns></returns>
        public static ILoggingBuilder UserNLog(this ILoggingBuilder builder, IConfiguration configuration)
        {
            builder.ClearProviders();
            builder.SetMinimumLevel(LogLevel.Trace);
            builder.AddNLog(configuration);
            return builder;
        }

        /// <summary>
        /// 注册NLog服务
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="configPath"></param>
        /// <returns></returns>
        public static ILoggingBuilder UserNLog(this ILoggingBuilder builder, string configPath)
        {
            builder.ClearProviders();
            builder.SetMinimumLevel(LogLevel.Trace);
            builder.AddNLog(configPath);
            return builder;
        }

        /// <summary>
        /// 获取Logger工厂
        /// </summary>
        /// <param name="provider"></param>
        /// <typeparam name="ILoggerFactory"></typeparam>
        /// <returns></returns>
        public static ILoggerFactory GetLoggerFactory(this IServiceProvider provider) => provider.GetService<ILoggerFactory>();

        /// <summary>
        /// 获取Logger
        /// </summary>
        /// <param name="provider"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static ILogger<T> GetLogger<T>(this IServiceProvider provider)
            => provider.GetService<ILogger<T>>();

        /// <summary>
        /// 获取Logger
        /// </summary>
        /// <param name="provider"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static ILogger GetLogger(this IServiceProvider provider, Type type)
            => provider.GetLoggerFactory().CreateLogger(type);

        /// <summary>
        /// 获取Logger
        /// </summary>
        /// <param name="provider"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static ILogger GetLogger(this IServiceProvider provider, string name)
            => provider.GetLoggerFactory().CreateLogger(name);
    }
}
