using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Hosting;

using DwFramework.Core;
using DwFramework.Core.Extensions;

namespace DwFramework.Http
{
    public static class HttpServiceExtension
    {
        /// <summary>
        /// 注册Http服务
        /// </summary>
        /// <param name="host"></param>
        public static void RegisterHttpService(this ServiceHost host)
        {
            host.RegisterType<IHttpService, HttpService>().SingleInstance();
        }

        /// <summary>
        /// 初始化Http服务
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="provider"></param>
        public static Task InitHttpServiceAsync<T>(this IServiceProvider provider) where T : class, IHttpStartup
        {
            return provider.GetService<IHttpService, HttpService>().OpenServiceAsync<T>();
        }
    }

    public class HttpService : IHttpService
    {
        public class Config
        {
            public string ContentRoot { get; set; }
            public Dictionary<string, string> Listen { get; set; }
        }

        private readonly IServiceProvider _provider;
        private readonly IRunEnvironment _environment;
        private readonly Config _config;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="provider"></param>
        /// <param name="environment"></param>
        public HttpService(IServiceProvider provider, IRunEnvironment environment)
        {
            _provider = provider;
            _environment = environment;
            _config = _environment.GetConfiguration().GetSection<Config>("Http");
        }

        /// <summary>
        /// 开启Http服务
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public Task OpenServiceAsync<T>() where T : class, IHttpStartup
        {
            return Task.Run(() =>
            {
                var builder = new WebHostBuilder()
                    .UseDwServiceProvider(_provider)
                    // https证书路径
                    .UseContentRoot($"{AppDomain.CurrentDomain.BaseDirectory}{_config.ContentRoot}")
                    .UseKestrel(options =>
                    {
                        // 监听地址及端口
                        if (_config.Listen == null || _config.Listen.Count <= 0)
                            options.Listen(IPAddress.Parse("0.0.0.0"), 5080);
                        else
                        {
                            if (_config.Listen.ContainsKey("http"))
                            {
                                string[] ipAndPort = _config.Listen["http"].Split(":");
                                options.Listen(IPAddress.Parse(ipAndPort[0]), int.Parse(ipAndPort[1]));
                            }
                            if (_config.Listen.ContainsKey("https"))
                            {
                                string[] addrAndCert = _config.Listen["https"].Split(";");
                                string[] ipAndPort = addrAndCert[0].Split(":");
                                options.Listen(IPAddress.Parse(ipAndPort[0]), int.Parse(ipAndPort[1]), listenOptions =>
                                {
                                    string[] certAndPassword = addrAndCert[1].Split(",");
                                    listenOptions.UseHttps(certAndPassword[0], certAndPassword[1]);
                                });
                            }
                        }
                    })
                    .UseStartup<T>()
                    .Build();
                builder.Run();
            });
        }
    }
}
