using System;
using Microsoft.Extensions.DependencyInjection;
using Autofac;

namespace DwFramework.Core.Generator
{
    public static class GeneratorExtension
    {
        /// <summary>
        /// 配置雪花生成器
        /// </summary>
        /// <param name="host"></param>
        /// <param name="workerId"></param>
        /// <param name="startTime"></param>
        /// <param name="isGlobal"></param>
        /// <returns></returns>
        public static ServiceHost ConfigureSnowflakeGenerator(this ServiceHost host, long workerId, DateTime startTime, bool isGlobal = true)
        {
            return host.ConfigureContainer(builder =>
            {
                var registration = builder.Register(_ => new SnowflakeGenerator(workerId, startTime));
                if (isGlobal) registration.SingleInstance();
            });
        }

        /// <summary>
        /// 获取雪花生成器
        /// </summary>
        /// <param name="provider"></param>
        /// <typeparam name="SnowflakeGenerator"></typeparam>
        /// <returns></returns>
        public static SnowflakeGenerator GetSnowflakeGenerator(this IServiceProvider provider) => provider.GetService<SnowflakeGenerator>();
    }
}