using System;
using System.Net;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using DwFramework.Core;

namespace DwFramework.WEB
{
    public sealed class WebApiService
    {
        private readonly Config _config;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="host"></param>
        /// <param name="config"></param>
        /// <param name="startup"></param>
        public WebApiService(ServiceHost host, Config config, Type startup)
        {
            _config = config;
            if (config.Listens == null || config.Listens.Count <= 0) throw new Exception("缺少Listen配置");
            host.ConfigureHostBuilder(hostBuilder =>
            {
                hostBuilder.ConfigureWebHost(webHostBuilder =>
                {
                    webHostBuilder.UseKestrel(options =>
                    {
                        if (config.Listens.ContainsKey("http"))
                        {
                            var ipAndPort = config.Listens["http"].Split(":");
                            var ip = string.IsNullOrEmpty(ipAndPort[0]) ? IPAddress.Any : IPAddress.Parse(ipAndPort[0]);
                            var port = int.Parse(ipAndPort[1]);
                            options.Listen(ip, port);
                        }
                        if (config.Listens.ContainsKey("https"))
                        {
                            var addrAndCert = config.Listens["https"].Split(";");
                            var ipAndPort = addrAndCert[0].Split(":");
                            var ip = string.IsNullOrEmpty(ipAndPort[0]) ? IPAddress.Any : IPAddress.Parse(ipAndPort[0]);
                            var port = int.Parse(ipAndPort[1]);
                            options.Listen(ip, port, listenOptions =>
                            {
                                var certAndPassword = addrAndCert[1].Split(",");
                                listenOptions.UseHttps(certAndPassword[0], certAndPassword[1]);
                            });
                        }
                    }).UseStartup(startup);
                });
            });
        }
    }
}