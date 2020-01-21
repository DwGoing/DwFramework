using System;
using System.Net;
using System.Threading.Tasks;

using Hprose.RPC;
using Microsoft.Extensions.Configuration;
using Autofac.Extensions.DependencyInjection;

using DwFramework.Core;
using DwFramework.Core.Extensions;

namespace DwFramework.Rpc
{
    public static class RpcServiceExtension
    {
        /// <summary>
        /// 注册Rpc服务
        /// </summary>
        /// <param name="host"></param>
        public static void RegisterRpcService(this ServiceHost host)
        {
            host.RegisterType<IRpcService, RpcService>().SingleInstance();
        }

        /// <summary>
        /// 初始化Rpc服务
        /// </summary>
        /// <param name="provider"></param>
        public static Task InitRpcServiceAsync(this AutofacServiceProvider provider)
        {
            return provider.GetService<IRpcService, RpcService>().OpenServiceAsync();
        }
    }

    public class RpcService : IRpcService
    {
        public class Config
        {
            public string[] Prefixes { get; set; }
        }

        private readonly IConfiguration _configuration;
        private readonly Config _config;

        public Service Service { get; private set; }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="configuration"></param>
        public RpcService(IConfiguration configuration)
        {
            _configuration = configuration;
            _config = _configuration.GetSection("Rpc").Get<Config>();
        }

        /// <summary>
        /// 开启Rpc服务
        /// </summary>
        /// <returns></returns>
        public Task OpenServiceAsync()
        {
            return Task.Run(() =>
            {
                var listener = new HttpListener();
                foreach (var item in _config.Prefixes)
                {
                    listener.Prefixes.Add(item.EndsWith("/") ? item : $"{item}/");
                }
                listener.Start();
                Service = new Service();
                Service.Bind(listener);
                Console.WriteLine($"Rpc Bind:\n{_config.Prefixes.ToJson()}");
            });
        }
    }
}
