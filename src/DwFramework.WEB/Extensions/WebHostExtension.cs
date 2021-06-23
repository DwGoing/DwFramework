using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using DwFramework.Core;

namespace DwFramework.WEB
{
    public static class WebHostExtension
    {
        /// <summary>
        /// 配置Web主机
        /// </summary>
        /// <param name="host"></param>
        /// <param name="configure"></param>
        /// <returns></returns>
        public static ServiceHost ConfigureWebHost(this ServiceHost host, Action<IWebHostBuilder> configure)
        {
            host.ConfigureHostBuilder(builder => builder.ConfigureWebHost(configure));
            return host;
        }
    }
}
