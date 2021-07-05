using System;
using Autofac;

namespace DwFramework.Core
{
    public static class ModuleExtension
    {
        /// <summary>
        /// 导入模块
        /// </summary>
        /// <param name="host"></param>
        /// <param name="modulePath"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static ServiceHost ImportModule<T>(this ServiceHost host, string modulePath)
            => host.ConfigureContainer(builder => builder.Register(_ => ModuleManager.LoadModule<T>(modulePath)));
    }
}