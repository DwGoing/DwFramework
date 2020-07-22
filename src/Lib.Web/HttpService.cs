using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

using DwFramework.Core;
using DwFramework.Core.Extensions;

namespace DwFramework.Web
{
    public class HttpService : BaseService
    {
        public class Config
        {
            public string ContentRoot { get; set; }
            public Dictionary<string, string> Listen { get; set; }
        }

        private readonly Config _config;

        /// <summary>
        /// 构造函数
        /// </summary>
        public HttpService()
        {
            _config = ServiceHost.Environment.GetConfiguration().GetConfig<Config>("Web:Http");
        }

        /// <summary>
        /// 开启服务
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public Task OpenServiceAsync<T>() where T : class
        {
            var builder = new WebHostBuilder()
                .SuppressStatusMessages(true)
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
                        options.Listen(string.IsNullOrEmpty(ipAndPort[0]) ? IPAddress.Any : IPAddress.Parse(ipAndPort[0]), int.Parse(ipAndPort[1]));
                        listen += $"http://{_config.Listen["http"]}";
                    }
                    if (_config.Listen.ContainsKey("https"))
                    {
                        string[] addrAndCert = _config.Listen["https"].Split(";");
                        string[] ipAndPort = addrAndCert[0].Split(":");
                        options.Listen(string.IsNullOrEmpty(ipAndPort[0]) ? IPAddress.Any : IPAddress.Parse(ipAndPort[0]), int.Parse(ipAndPort[1]), listenOptions =>
                        {
                            string[] certAndPassword = addrAndCert[1].Split(",");
                            listenOptions.UseHttps(certAndPassword[0], certAndPassword[1]);
                        });
                        if (!string.IsNullOrEmpty(listen)) listen += ",";
                        listen += $"https://{_config.Listen["https"]}";
                    }
                    Console.WriteLine($"Http服务已开启 => 监听地址:{listen}");
                })
                .UseStartup<T>();
            return builder.Build().RunAsync();
        }
    }
}
