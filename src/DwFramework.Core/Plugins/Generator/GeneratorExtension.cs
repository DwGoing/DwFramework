using System;
using Autofac;

namespace DwFramework.Core.Generator
{
    public static class GeneratorExtension
    {
        /// <summary>
        /// 配置全局雪花生成器
        /// </summary>
        /// <param name="host"></param>
        /// <param name="workerId"></param>
        /// <param name="startTime"></param>
        /// <returns></returns>
        public static ServiceHost cSnowflakeGenerator(this ServiceHost host, long workerId, DateTime startTime)
            => host.ConfigureContainer(builder => builder.Register(_ => new SnowflakeGenerator(workerId, startTime)).SingleInstance());
    }
}