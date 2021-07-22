using System;
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
        public static ServiceHost cSnowflakeGenerator(this ServiceHost host, long workerId, DateTime startTime, bool isGlobal = true)
        {
            return host.ConfigureContainer(builder =>
            {
                var registration = builder.Register(_ => new SnowflakeGenerator(workerId, startTime));
                if (isGlobal) registration.SingleInstance();
            });
        }
    }
}