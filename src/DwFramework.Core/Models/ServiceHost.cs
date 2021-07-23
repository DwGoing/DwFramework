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
        private readonly IHostBuilder _hostBuilder;
        private static IHost _host;

        public static bool IsDebug { get; private set; }
        public static string EnvironmentType { get; private set; }
        public static IServiceProvider ServiceProvider => _host.Services;
        public event Action<IServiceProvider> OnHostStarted;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="isDebug"></param>
        /// <param name="environmentType"></param>
        /// <param name="args"></param>
        public ServiceHost(bool isDebug = true, string environmentType = null, params string[] args)
        {
            _hostBuilder = Host.CreateDefaultBuilder(args).UseServiceProviderFactory(new AutofacServiceProviderFactory());
            if (string.IsNullOrEmpty(environmentType)) environmentType = Environment.GetEnvironmentVariable("ENVIRONMENT_TYPE");
            if (string.IsNullOrEmpty(environmentType)) environmentType = "Development";
            _hostBuilder.UseEnvironment(environmentType);
        }

        /// <summary>
        /// 配置主机构造器
        /// </summary>
        /// <param name="configure"></param>
        /// <returns></returns>
        public ServiceHost ConfigureHostBuilder(Action<IHostBuilder> configure)
        {
            configure(_hostBuilder);
            return this;
        }

        /// <summary>
        /// 配置主机
        /// </summary>
        /// <param name="configure"></param>
        /// <returns></returns>
        public ServiceHost ConfigureHostConfiguration(Action<IConfigurationBuilder> configure)
        {
            _hostBuilder.ConfigureHostConfiguration(configure);
            return this;
        }

        /// <summary>
        /// 配置应用
        /// </summary>
        /// <param name="configure"></param>
        /// <returns></returns>
        public ServiceHost ConfigureAppConfiguration(Action<IConfigurationBuilder> configure)
        {
            _hostBuilder.ConfigureAppConfiguration(configure);
            return this;
        }

        /// <summary>
        /// 配置应用
        /// </summary>
        /// <param name="configure"></param>
        /// <returns></returns>
        public ServiceHost ConfigureAppConfiguration(Action<HostBuilderContext, IConfigurationBuilder> configure)
        {
            _hostBuilder.ConfigureAppConfiguration(configure);
            return this;
        }

        /// <summary>
        /// 配置服务
        /// </summary>
        /// <param name="configure"></param>
        /// <returns></returns>
        public ServiceHost ConfigureServices(Action<IServiceCollection> configure)
        {
            _hostBuilder.ConfigureServices(configure);
            return this;
        }

        /// <summary>
        /// 配置服务
        /// </summary>
        /// <param name="configure"></param>
        /// <returns></returns>
        public ServiceHost ConfigureServices(Action<HostBuilderContext, IServiceCollection> configure)
        {
            _hostBuilder.ConfigureServices(configure);
            return this;
        }

        /// <summary>
        /// 配置日志
        /// </summary>
        /// <param name="configure"></param>
        /// <returns></returns>
        public ServiceHost ConfigureLogging(Action<ILoggingBuilder> configure)
        {
            _hostBuilder.ConfigureLogging(configure);
            return this;
        }

        /// <summary>
        /// 配置日志
        /// </summary>
        /// <param name="configure"></param>
        /// <returns></returns>
        public ServiceHost ConfigureLogging(Action<HostBuilderContext, ILoggingBuilder> configure)
        {
            _hostBuilder.ConfigureLogging(configure);
            return this;
        }

        /// <summary>
        /// 配置容器
        /// </summary>
        /// <param name="configure"></param>
        /// <returns></returns>
        public ServiceHost ConfigureContainer(Action<ContainerBuilder> configure)
        {
            _hostBuilder.ConfigureContainer(configure);
            return this;
        }

        /// <summary>
        /// 配置容器
        /// </summary>
        /// <param name="configure"></param>
        /// <returns></returns>
        public ServiceHost ConfigureContainer(Action<HostBuilderContext, ContainerBuilder> configure)
        {
            _hostBuilder.ConfigureContainer(configure);
            return this;
        }

        /// <summary>
        /// 添加配置
        /// </summary>
        /// <param name="configuration"></param>
        /// <returns></returns>
        public ServiceHost AddConfiguration(IConfiguration configuration)
        {
            _hostBuilder.ConfigureHostConfiguration(builder => builder.AddConfiguration(configuration));
            return this;
        }

        /// <summary>
        /// 添加Json配置
        /// </summary>
        /// <param name="path"></param>
        /// <param name="optional"></param>
        /// <param name="reloadOnChange"></param>
        /// <returns></returns>
        public ServiceHost AddJsonConfiguration(string path, bool optional = false, bool reloadOnChange = false)
            => AddConfiguration(new ConfigurationBuilder().AddJsonFile(path, optional, reloadOnChange).Build());

        /// <summary>
        /// 添加Json配置
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        public ServiceHost AddJsonConfiguration(Stream stream)
            => AddConfiguration(new ConfigurationBuilder().AddJsonStream(stream).Build());

        /// <summary>
        /// 添加Xml配置
        /// </summary>
        /// <param name="path"></param>
        /// <param name="optional"></param>
        /// <param name="reloadOnChange"></param>
        /// <returns></returns>         
        public ServiceHost AddXmlConfiguration(string path, bool optional = false, bool reloadOnChange = false)
            => AddConfiguration(new ConfigurationBuilder().AddXmlFile(path, optional, reloadOnChange).Build());

        /// <summary>
        /// 添加Xml配置
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>    
        public ServiceHost AddXmlConfiguration(Stream stream)
            => AddConfiguration(new ConfigurationBuilder().AddXmlStream(stream).Build());

        /// <summary>
        /// 注册服务
        /// </summary>
        /// <param name="assembly"></param>
        public void RegisterFromAssembly(Assembly assembly)
        {
            var t = assembly.GetTypes();
            foreach (var item in assembly.GetTypes())
            {
                var attr = item.GetCustomAttribute<RegisterableAttribute>();
                if (attr == null) continue;
                _hostBuilder.ConfigureContainer<ContainerBuilder>(builder =>
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

        /// <summary>
        /// 运行服务
        /// </summary>
        /// <returns></returns>
        public async Task RunAsync()
        {
            _hostBuilder.UseConsoleLifetime();
            _host = _hostBuilder.Build();
            OnHostStarted?.Invoke(ServiceProvider);
            await _host.RunAsync();
        }

        /// <summary>
        /// 停止服务
        /// </summary>
        /// <returns></returns>
        public async Task StopAsync() => await _host.WaitForShutdownAsync();

        /// <summary>
        /// 获取配置
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static IConfiguration GetConfiguration(string path = null)
            => ServiceProvider.GetConfiguration(path);

        /// <summary>
        /// 解析配置
        /// </summary>
        /// <param name="path"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T ParseConfiguration<T>(string path = null)
            => ServiceProvider.ParseConfiguration<T>(path);
    }
}
