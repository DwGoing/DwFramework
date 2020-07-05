using System;
using System.Linq;
using System.Net;
using System.Reflection;

using Hprose.RPC;

using DwFramework.Core;
using DwFramework.Core.Extensions;

namespace DwFramework.Rpc
{
    public class RpcService : BaseService
    {
        public class Config
        {
            public string[] Prefixes { get; set; } = new[] { "http://*:10100" };
        }

        private readonly Config _config;

        public Service Service { get; private set; }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="provider"></param>
        /// <param name="environment"></param>
        public RpcService(IServiceProvider provider, IEnvironment environment) : base(provider, environment)
        {
            _config = _environment.GetConfiguration().GetConfig<Config>("Rpc");
            var listener = new HttpListener();
            foreach (var item in _config.Prefixes)
            {
                listener.Prefixes.Add(item.EndsWith("/") ? item : $"{item}/");
            }
            listener.Start();
            Service = new Service();
            Service.Bind(listener);
        }

        /// <summary>
        /// 从实例中注册Rpc函数
        /// </summary>
        /// <param name="instance"></param>
        public void RegisterFuncFromInstance(object instance)
        {
            var methods = instance.GetType().GetMethods();
            foreach (var item in methods)
            {
                var attr = item.GetCustomAttribute<RpcAttribute>();
                if (attr != null)
                {
                    Service.Add(item, attr.CallName ?? "", instance);
                }
            }
        }

        /// <summary>
        /// 从服务中注册Rpc函数
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public void RegisterFuncFromService<T>() where T : class
        {
            T service = _provider.GetService<T>();
            var methods = service.GetType().GetMethods();
            foreach (var item in methods)
            {
                var attr = item.GetCustomAttribute<RpcAttribute>();
                if (attr != null)
                {
                    Service.Add(item, attr.CallName ?? "", service);
                }
            }
        }

        /// <summary>
        /// 从服务中注册Rpc函数
        /// </summary>
        /// <typeparam name="I"></typeparam>
        public void RegisterFuncFromServices<I>() where I : class
        {
            var services = _provider.GetServices<I>();
            foreach (var service in services)
            {
                var methods = service.GetType().GetMethods();
                foreach (var item in methods)
                {
                    var attr = item.GetCustomAttribute<RpcAttribute>();
                    if (attr != null)
                    {
                        Service.Add(item, attr.CallName ?? "", service);
                    }
                }
            }
        }

        /// <summary>
        /// 从程序集中注册Rpc函数
        /// 仅支持从程序集中注册的服务
        /// </summary>
        /// <param name="assemblyName"></param>
        public void RegisterFuncFromAssembly(string assemblyName)
        {
            var assembly = AppDomain.CurrentDomain.GetAssemblies().Where(item => item.FullName.Split(",").First() == assemblyName).FirstOrDefault();
            if (assembly == null)
                throw new Exception("未找到该程序集");
            var types = assembly.GetTypes();
            foreach (var type in types)
            {
                var typeAttr = type.GetCustomAttribute<RegisterableAttribute>();
                if (typeAttr == null)
                    continue;
                var methods = type.GetMethods();
                foreach (var item in methods)
                {
                    var methodAttr = item.GetCustomAttribute<RpcAttribute>();
                    if (methodAttr != null)
                    {
                        Service.Add(item, methodAttr.CallName ?? "", _provider.GetService(typeAttr.InterfaceType));
                    }
                }
            }
        }

        /// <summary>
        /// 从程序集中注册Rpc函数
        /// 仅支持从程序集中注册的服务
        /// </summary>
        public void RegisterFuncFromAssemblies()
        {
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            foreach (var assembly in assemblies)
            {
                var types = assembly.GetTypes();
                foreach (var type in types)
                {
                    var typeAttr = type.GetCustomAttribute<RegisterableAttribute>();
                    if (typeAttr == null)
                        continue;
                    var methods = type.GetMethods();
                    foreach (var item in methods)
                    {
                        var methodAttr = item.GetCustomAttribute<RpcAttribute>();
                        if (methodAttr != null)
                        {
                            Service.Add(item, methodAttr.CallName ?? "", _provider.GetService(typeAttr.InterfaceType));
                        }
                    }
                }
            }
        }
    }
}