using System;
using System.Collections.Generic;
using System.Net;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
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
            host.HostBuilder.ConfigureWebHost(configure);
            return host;
        }

        private sealed class WebAPIConfig
        {
            public string ContentRoot { get; init; }
            public Dictionary<string, string> Listen { get; init; }
        }

        /// <summary>
        /// 配置Web主机
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="host"></param>
        /// <returns></returns>
        public static ServiceHost ConfigureWebAPI<T>(this ServiceHost host) where T : class
        {
            host.ConfigureAppConfiguration((context, app) =>
            {
                var config = context.Configuration.GetConfig<WebAPIConfig>();
                if (config == null) throw new Exception("未读取到WebAPI配置");
                if (config.Listen.Count <= 0) throw new Exception("缺少Listen配置");
                host.HostBuilder.ConfigureWebHost(builder =>
                {
                    builder.UseKestrel(options =>
                    {
                        var listen = "";
                        if (config.Listen.ContainsKey("http"))
                        {
                            var ipAndPort = config.Listen["http"].Split(":");
                            var ip = string.IsNullOrEmpty(ipAndPort[0]) ? IPAddress.Any : IPAddress.Parse(ipAndPort[0]);
                            var port = int.Parse(ipAndPort[1]);
                            options.Listen(ip, port);
                            listen += $"http://{ip}:{port}";
                        }
                        if (config.Listen.ContainsKey("https"))
                        {
                            var addrAndCert = config.Listen["https"].Split(";");
                            var ipAndPort = addrAndCert[0].Split(":");
                            var ip = string.IsNullOrEmpty(ipAndPort[0]) ? IPAddress.Any : IPAddress.Parse(ipAndPort[0]);
                            var port = int.Parse(ipAndPort[1]);
                            options.Listen(ip, port, listenOptions =>
                            {
                                var certAndPassword = addrAndCert[1].Split(",");
                                listenOptions.UseHttps(certAndPassword[0], certAndPassword[1]);
                            });
                            if (!string.IsNullOrEmpty(listen)) listen += ",";
                            listen += $"https://{ip}:{port}";
                        }
                    });
                });
            });
            return host;
        }
    }
}
