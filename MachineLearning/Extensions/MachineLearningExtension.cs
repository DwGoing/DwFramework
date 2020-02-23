using System;

using DwFramework.Core;
using DwFramework.Core.Extensions;

namespace DwFramework.MachineLearning.Extensions
{
    public static class MachineLearningExtension
    {
        /// <summary>
        /// 注册MachineLearning服务
        /// </summary>
        /// <param name="host"></param>
        public static void RegisterMachineLearningService(this ServiceHost host)
        {
            host.RegisterType<IMachineLearningService, MachineLearningService>().SingleInstance();
        }
    }
}
