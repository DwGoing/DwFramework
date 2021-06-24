using System;
using Microsoft.Extensions.DependencyInjection;

namespace DwFramework.Web
{
    public static class UdpExtension
    {
        /// <summary>
        /// 获取Udp服务
        /// </summary>
        /// <param name="provider"></param>
        /// <typeparam name="UdpService"></typeparam>
        /// <returns></returns>
        public static UdpService GetUdp(this IServiceProvider provider) => provider.GetService<UdpService>();
    }
}