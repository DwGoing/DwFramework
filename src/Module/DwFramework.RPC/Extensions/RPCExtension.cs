﻿using System;
using System.Threading.Tasks;

using DwFramework.Core;
using DwFramework.Core.Extensions;

namespace DwFramework.RPC
{
    public static class RPCExtension
    {
        /// <summary>
        /// 注册服务
        /// </summary>
        /// <param name="host"></param>
        /// <param name="configKey"></param>
        /// <param name="configPath"></param>
        /// <param name="services"></param>
        public static void RegisterRPCService(this ServiceHost host, string configKey = null, string configPath = null, params Type[] services)
        {
            host.Register(c => new RPCService(configKey, configPath)).SingleInstance();
            services.ForEach(item =>
            {
                host.OnInitializing += provider =>
                {
                    var service = provider.GetService(item);
                    if (service == null) return;
                    provider.GetRPCService().AddService(service);
                };
            });
            host.OnInitializing += provider => provider.InitRPCServiceAsync().Wait();
        }

        /// <summary>
        /// 获取服务
        /// </summary>
        /// <param name="provider"></param>
        /// <returns></returns>
        public static RPCService GetRPCService(this IServiceProvider provider) => provider.GetService<RPCService>();

        /// <summary>
        /// 初始化服务
        /// </summary>
        /// <param name="provider"></param>
        /// <returns></returns>
        public static Task InitRPCServiceAsync(this IServiceProvider provider) => provider.GetRPCService().OpenServiceAsync();
    }
}
