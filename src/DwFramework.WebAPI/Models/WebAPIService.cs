using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;

using DwFramework.Core;
using DwFramework.Core.Plugins;

namespace DwFramework.WebAPI
{
    public sealed class WebAPIService : ConfigableServiceBase
    {
        public sealed class Config
        {
            public string ContentRoot { get; init; }
            public Dictionary<string, string> Listen { get; init; }
        }

        private readonly ILogger<WebAPIService> _logger;
        private Config _config;
        private CancellationTokenSource _cancellationTokenSource;
        private event Action<IServiceCollection> _onConfigureServices;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="logger"></param>
        public WebAPIService(ILogger<WebAPIService> logger = null)
        {
            _logger = logger;
        }

        /// <summary>
        /// 读取配置
        /// </summary>
        /// <param name="config"></param>
        public void ReadConfig(Config config)
        {
            try
            {
                _config = config;
                if (_config == null) throw new Exception("未读取到WebAPI配置");
            }
            catch (Exception ex)
            {
                _ = _logger?.LogErrorAsync(ex.Message);
                throw;
            }
        }

        /// <summary>
        /// 读取配置
        /// </summary>
        /// <param name="path"></param>
        /// <param name="key"></param>
        public void ReadConfig(string path = null, string key = null)
        {
            ReadConfig(ReadConfig<Config>(path, key));
        }

        /// <summary>
        /// 添加内部服务
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        public WebAPIService AddInternalService(Action<IServiceCollection> action)
        {
            _onConfigureServices += action;
            return this;
        }

        /// <summary>
        /// 添加外部服务
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public WebAPIService AddExternalService(Type type)
        {
            _onConfigureServices += services => services.AddTransient(type, _ => ServiceHost.Provider.GetService(type));
            return this;
        }

        /// <summary>
        /// 添加外部服务
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public WebAPIService AddExternalService<T>() where T : class
        {
            AddExternalService(typeof(T));
            return this;
        }

        /// <summary>
        /// 运行服务
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public async Task RunAsync<T>() where T : class
        {
            try
            {
                _cancellationTokenSource?.Dispose();
                _cancellationTokenSource = new CancellationTokenSource();
                var builder = Host.CreateDefaultBuilder()
                    .ConfigureWebHostDefaults(builder =>
                    {
                        builder.ConfigureLogging(builder => builder.AddFilter("Microsoft", LogLevel.Warning))
                        // https证书路径
                        .UseContentRoot($"{AppDomain.CurrentDomain.BaseDirectory}{_config.ContentRoot}")
                        .UseKestrel(async options =>
                        {
                            if (_config.Listen == null || _config.Listen.Count <= 0) throw new Exception("缺少Listen配置");
                            var listen = "";
                            // 监听地址及端口
                            if (_config.Listen.ContainsKey("http"))
                            {
                                var ipAndPort = _config.Listen["http"].Split(":");
                                var ip = string.IsNullOrEmpty(ipAndPort[0]) ? IPAddress.Any : IPAddress.Parse(ipAndPort[0]);
                                var port = int.Parse(ipAndPort[1]);
                                options.Listen(ip, port);
                                listen += $"http://{ip}:{port}";
                            }
                            if (_config.Listen.ContainsKey("https"))
                            {
                                var addrAndCert = _config.Listen["https"].Split(";");
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
                            if (_logger != null) await _logger?.LogInformationAsync($"WebAPI服务正在监听:{listen}");
                        })
                        .ConfigureServices(services => _onConfigureServices?.Invoke(services))
                        .UseStartup<T>();
                    });
                await builder.Build().RunAsync(_cancellationTokenSource.Token);
            }
            catch (Exception ex)
            {
                if (_logger != null) await _logger?.LogErrorAsync(ex.Message);
                throw;
            }
        }

        /// <summary>
        /// 停止服务
        /// </summary>
        public void Stop()
        {
            _cancellationTokenSource.Cancel();
        }
    }
}