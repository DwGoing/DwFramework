using System;

using DwFramework.Core;
using DwFramework.Core.Extensions;

namespace DwFramework.DataFlow.Extensions
{
    public static class DataFlowExtension
    {
        /// <summary>
        /// 注册服务
        /// </summary>
        /// <param name="host"></param>
        public static void RegisterDataFlowService(this ServiceHost host)
        {
            host.RegisterType<DataFlowService>().SingleInstance();
        }

        /// <summary>
        /// 获取服务
        /// </summary>
        /// <param name="provider"></param>
        /// <returns></returns>
        public static DataFlowService GetDataFlowService(this IServiceProvider provider)
        {
            return provider.GetService<DataFlowService>();
        }
    }
}
