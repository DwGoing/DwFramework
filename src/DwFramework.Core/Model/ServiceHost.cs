using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using Autofac;
using Autofac.Extensions.DependencyInjection;

namespace DwFramework.Core
{
    public sealed class ServiceHost
    {
        private static IHost _host;

        public readonly IHostBuilder HostBuilder;
        public event Action<IServiceProvider> OnHostStarting;
        public event Action<IServiceProvider> OnHostStarted;
        public static IServiceProvider ServiceProvider => _host.Services;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="environmentType"></param>
        /// <param name="args"></param>
        public ServiceHost(EnvironmentType environmentType = EnvironmentType.Development, params string[] args)
        {
            HostBuilder = Host.CreateDefaultBuilder(args).UseServiceProviderFactory(new AutofacServiceProviderFactory());
            if (!Enum.IsDefined<EnvironmentType>(environmentType)) environmentType = EnvironmentType.Development;
            HostBuilder.UseEnvironment(environmentType.ToString());
        }

        /// <summary>
        /// 配置应用
        /// </summary>
        /// <param name="configure"></param>
        /// <returns></returns>
        public ServiceHost ConfigureAppConfiguration(Action<IConfigurationBuilder> configure)
        {
            HostBuilder.ConfigureAppConfiguration(configure);
            return this;
        }

        /// <summary>
        /// 配置应用
        /// </summary>
        /// <param name="configure"></param>
        /// <returns></returns>
        public ServiceHost ConfigureAppConfiguration(Action<HostBuilderContext, IConfigurationBuilder> configure)
        {
            HostBuilder.ConfigureAppConfiguration(configure);
            return this;
        }

        /// <summary>
        /// 配置容器
        /// </summary>
        /// <param name="configure"></param>
        /// <returns></returns>
        public ServiceHost ConfigureContainer(Action<ContainerBuilder> configure)
        {
            HostBuilder.ConfigureContainer(configure);
            return this;
        }

        /// <summary>
        /// 配置容器
        /// </summary>
        /// <param name="configure"></param>
        /// <returns></returns>
        public ServiceHost ConfigureContainer(Action<HostBuilderContext, ContainerBuilder> configure)
        {
            HostBuilder.ConfigureContainer(configure);
            return this;
        }

        /// <summary>
        /// 配置主机
        /// </summary>
        /// <param name="configure"></param>
        /// <returns></returns>
        public ServiceHost ConfigureHostConfiguration(Action<IConfigurationBuilder> configure)
        {
            HostBuilder.ConfigureHostConfiguration(configure);
            return this;
        }

        /// <summary>
        /// 配置日志
        /// </summary>
        /// <param name="configure"></param>
        /// <returns></returns>
        public ServiceHost ConfigureLogging(Action<ILoggingBuilder> configure)
        {
            HostBuilder.ConfigureLogging(configure);
            return this;
        }

        /// <summary>
        /// 配置日志
        /// </summary>
        /// <param name="configure"></param>
        /// <returns></returns>
        public ServiceHost ConfigureLogging(Action<HostBuilderContext, ILoggingBuilder> configure)
        {
            HostBuilder.ConfigureLogging(configure);
            return this;
        }

        /// <summary>
        /// 配置服务
        /// </summary>
        /// <param name="configure"></param>
        /// <returns></returns>
        public ServiceHost ConfigureServices(Action<IServiceCollection> configure)
        {
            HostBuilder.ConfigureServices(configure);
            return this;
        }

        /// <summary>
        /// 配置服务
        /// </summary>
        /// <param name="configure"></param>
        /// <returns></returns>
        public ServiceHost ConfigureServices(Action<HostBuilderContext, IServiceCollection> configure)
        {
            HostBuilder.ConfigureServices(configure);
            return this;
        }

        /// <summary>
        /// 添加Json配置
        /// </summary>
        /// <param name="path"></param>
        /// <param name="optional"></param>
        /// <param name="reloadOnChange"></param>
        /// <returns></returns>
        public ServiceHost AddJsonConfig(string path, bool optional = false, bool reloadOnChange = false)
        {
            HostBuilder.ConfigureHostConfiguration(builder => builder.AddJsonFile(path, optional, reloadOnChange));
            return this;
        }

        /// <summary>
        /// 添加Json配置
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        public ServiceHost AddJsonConfig(Stream stream)
        {
            HostBuilder.ConfigureHostConfiguration(builder => builder.AddJsonStream(stream));
            return this;
        }

        /// <summary>
        /// 添加Xml配置
        /// </summary>
        /// <param name="path"></param>
        /// <param name="optional"></param>
        /// <param name="reloadOnChange"></param>
        /// <returns></returns>         
        public ServiceHost AddXmlConfig(string path, bool optional = false, bool reloadOnChange = false)
        {
            HostBuilder.ConfigureHostConfiguration(builder => builder.AddXmlFile(path, optional, reloadOnChange));
            return this;
        }

        /// <summary>
        /// 添加Xml配置
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>    
        public ServiceHost AddXmlConfig(Stream stream)
        {
            HostBuilder.ConfigureHostConfiguration(builder => builder.AddXmlStream(stream));
            return this;
        }

        /// <summary>
        /// 运行服务
        /// </summary>
        /// <returns></returns>
        public async Task RunAsync()
        {
            HostBuilder.UseConsoleLifetime();
            OnHostStarting?.Invoke(ServiceProvider);
            _host = HostBuilder.Build();
            OnHostStarted?.Invoke(ServiceProvider);
            await _host.RunAsync();
        }

        /// <summary>
        /// 停止服务
        /// </summary>
        /// <returns></returns>
        public async Task StopAsync() => await _host.WaitForShutdownAsync();

        /// <summary>
        /// 注册服务
        /// </summary>
        /// <param name="assembly"></param>
        public void RegisterFromAssembly(Assembly assembly)
        {
            foreach (var item in assembly.GetTypes())
            {
                var attr = item.GetCustomAttribute<RegisterableAttribute>();
                if (attr == null) return;
                HostBuilder.ConfigureContainer<ContainerBuilder>(builder =>
                {
                    var registration = builder.RegisterType(item);
                    registration = attr.InterfaceType != null ? registration.As(attr.InterfaceType) : registration.AsSelf();
                    registration = attr.Lifetime switch
                    {
                        Lifetime.Singleton => registration.SingleInstance(),
                        Lifetime.InstancePerLifetimeScope => registration.InstancePerLifetimeScope(),
                        _ => registration
                    };
                    if (attr.IsAutoActivate) registration.AutoActivate();
                });
            };
        }

        /// <summary>
        /// 注册服务
        /// </summary>
        public void RegisterFromAssemblies()
        {
            foreach (var item in AppDomain.CurrentDomain.GetAssemblies()) RegisterFromAssembly(item);
        }
    }
}
