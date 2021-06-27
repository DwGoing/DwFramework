using System;
using System.Net;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using DwFramework.Core;

namespace DwFramework.Web.WebApi
{
    public sealed class WebApiService
    {
        private readonly Config.Web _config;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="host"></param>
        /// <param name="config"></param>
        /// <param name="configureWebHostBuilder"></param>
        public WebApiService(ServiceHost host, Config.Web config, Action<IWebHostBuilder> configureWebHostBuilder)
        {
            _config = config;
            host.ConfigureHostBuilder(hostBuilder =>
            {
                hostBuilder.ConfigureWebHost(webHostBuilder =>
                {
                    webHostBuilder.UseContentRoot($"{AppDomain.CurrentDomain.BaseDirectory}{_config.ContentRoot}")
                    .UseKestrel(options =>
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
                    });
                    configureWebHostBuilder?.Invoke(webHostBuilder);
                });
            });
        }
    }
}