using System;
using Microsoft.Extensions.DependencyInjection;

namespace DwFramework.Web
{
    public static class TcpExtension
    {
        /// <summary>
        /// 获取Tcp服务
        /// </summary>
        /// <param name="provider"></param>
        /// <typeparam name="TcpService"></typeparam>
        /// <returns></returns>
        public static TcpService GetTcp(this IServiceProvider provider) => provider.GetService<TcpService>();
    }
}