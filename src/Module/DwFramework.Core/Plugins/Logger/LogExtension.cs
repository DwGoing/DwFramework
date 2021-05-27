using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
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
        /// <param name="host"></param>
        /// <param name="configure"></param>
        /// <returns></returns>
        public static ServiceHost RegisterLog(this ServiceHost host, Action<ILoggingBuilder> configure)
        {
            host.RegisterService(services => services.AddLogging(configure));
            return host;
        }

        /// <summary>
        /// 注册Log服务
        /// </summary>
        /// <param name="host"></param>
        /// <param name="configure"></param>
        /// <returns></returns>
        public static ServiceHost RegisterLog(this ServiceHost host, IConfiguration configure)
        {
            return host.RegisterLog(builder =>
            {
                builder.ClearProviders();
                builder.SetMinimumLevel(LogLevel.Trace);
                builder.AddNLog(configure);
            });
        }

        /// <summary>
        /// 注册Log服务
        /// </summary>
        /// <param name="host"></param>
        /// <param name="configFile"></param>
        /// <returns></returns>
        public static ServiceHost RegisterLog(this ServiceHost host, string configFile = null)
        {
            return host.RegisterLog(builder =>
            {
                builder.ClearProviders();
                builder.SetMinimumLevel(LogLevel.Trace);
                if (string.IsNullOrEmpty(configFile)) builder.AddNLog();
                else builder.AddNLog(configFile);
            });
        }

        /// <summary>
        /// 获取Log服务
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="provider"></param>
        /// <returns></returns>
        public static ILogger<T> GetLogger<T>(this IServiceProvider provider)
        {
            return provider.GetService<ILogger<T>>();
        }

        /// <summary>
        /// Debug
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="message"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public static async Task LogDebugAsync(this ILogger logger, string message, params object[] args)
        {
            await TaskManager.CreateTask(() => logger.LogDebug(message, args));
        }

        /// <summary>
        /// Trace
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="message"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public static async Task LogTraceAsync(this ILogger logger, string message, params object[] args)
        {
            await TaskManager.CreateTask(() => logger.LogTrace(message, args));
        }

        /// <summary>
        /// Information
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="message"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public static async Task LogInformationAsync(this ILogger logger, string message, params object[] args)
        {
            await TaskManager.CreateTask(() => logger.LogInformation(message, args));
        }

        /// <summary>
        /// Warning
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="message"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public static async Task LogWarningAsync(this ILogger logger, string message, params object[] args)
        {
            await TaskManager.CreateTask(() => logger.LogWarning(message, args));
        }

        /// <summary>
        /// Error
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="message"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public static async Task LogErrorAsync(this ILogger logger, string message, params object[] args)
        {
            await TaskManager.CreateTask(() => logger.LogError(message, args));
        }
    }
}
