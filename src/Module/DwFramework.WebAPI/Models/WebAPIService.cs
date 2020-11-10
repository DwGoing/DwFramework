using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using DwFramework.Core;
using DwFramework.Core.Plugins;

namespace DwFramework.WebAPI
{
    public sealed class WebAPIService
    {
        public class Config
        {
            public string ContentRoot { get; set; }
            public Dictionary<string, string> Listen { get; set; }
        }

        private readonly Config _config;
        private readonly ILogger<WebAPIService> _logger;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="environment"></param>
        /// <param name="configKey"></param>
        public WebAPIService(Core.Environment environment, string configKey = null)
        {
            var configuration = environment.GetConfiguration(configKey ?? "WebAPI");
            _config = configuration.GetConfig<Config>(configKey);
            if (_config == null) throw new Exception("未读取到WebAPI配置");
            _logger = ServiceHost.Provider.GetLogger<WebAPIService>();
        }

        /// <summary>
        /// 开启服务
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public Task OpenServiceAsync<T>() where T : class
        {
            return Host.CreateDefaultBuilder()
                .ConfigureWebHostDefaults(builder =>
                {
                    builder.ConfigureLogging(builder => builder.AddFilter("Microsoft", LogLevel.Warning))
                    // https证书路径
                    .UseContentRoot($"{AppDomain.CurrentDomain.BaseDirectory}{_config.ContentRoot}")
                    .UseKestrel(options =>
                    {
                        if (_config.Listen == null || _config.Listen.Count <= 0) throw new Exception("缺少Listen配置");
                        string listen = "";
                        // 监听地址及端口
                        if (_config.Listen.ContainsKey("http"))
                        {
                            string[] ipAndPort = _config.Listen["http"].Split(":");
                            var ip = string.IsNullOrEmpty(ipAndPort[0]) ? IPAddress.Any : IPAddress.Parse(ipAndPort[0]);
                            var port = int.Parse(ipAndPort[1]);
                            options.Listen(ip, port);
                            listen += $"http://{ip}:{port}";
                        }
                        if (_config.Listen.ContainsKey("https"))
                        {
                            string[] addrAndCert = _config.Listen["https"].Split(";");
                            string[] ipAndPort = addrAndCert[0].Split(":");
                            var ip = string.IsNullOrEmpty(ipAndPort[0]) ? IPAddress.Any : IPAddress.Parse(ipAndPort[0]);
                            var port = int.Parse(ipAndPort[1]);
                            options.Listen(ip, port, listenOptions =>
                            {
                                string[] certAndPassword = addrAndCert[1].Split(",");
                                listenOptions.UseHttps(certAndPassword[0], certAndPassword[1]);
                            });
                            if (!string.IsNullOrEmpty(listen)) listen += ",";
                            listen += $"https://{ip}:{port}";
                        }
                        _logger.LogDebug($"WebAPI服务已开启 => 监听地址:{listen}");
                    })
                    .UseStartup<T>();
                }).Build().RunAsync();
        }
    }
}
