using System;

using DwFramework.Core;
using DwFramework.Core.Extensions;

namespace DwFramework.MachineLearning.Extensions
{
    public static class MachineLearningExtension
    {
        /// <summary>
        /// 注册服务
        /// </summary>
        /// <param name="host"></param>
        public static void RegisterMachineLearningService(this ServiceHost host)
        {
            host.RegisterType<MachineLearningService>().SingleInstance();
        }

        /// <summary>
        /// 获取服务
        /// </summary>
        /// <param name="provider"></param>
        /// <returns></returns>
        public static MachineLearningService GetMachineLearningService(this IServiceProvider provider)
        {
            return provider.GetService<MachineLearningService>();
        }
    }
}
