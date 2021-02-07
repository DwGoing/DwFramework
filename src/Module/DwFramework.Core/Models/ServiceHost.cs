using System;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Autofac;
using Autofac.Builder;
using Autofac.Core;
using Autofac.Core.Registration;
using Autofac.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;

using DwFramework.Core.Extensions;
using DwFramework.Core.Plugins;

namespace DwFramework.Core
{
    public sealed class ServiceHost
    {
        private readonly AutoResetEvent _autoResetEvent;
        private readonly ContainerBuilder _containerBuilder;
        private readonly ServiceCollection _services;
        private ILogger<ServiceHost> _logger;

        public static Environment Environment { get; private set; } = null;
        public static AutofacServiceProvider Provider { get; private set; }
        public event Action<IServiceProvider> OnInitializing;
        public event Action<IServiceProvider> OnInitialized;
        public event Action<IServiceProvider> OnStoping;
        public event Action<IServiceProvider> OnStopped;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="environmentType"></param>
        public ServiceHost(EnvironmentType environmentType)
        {
            _autoResetEvent = new AutoResetEvent(false);
            _containerBuilder = new ContainerBuilder();
            _services = new ServiceCollection();
            // 环境变量
            Environment = new Environment(environmentType);
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="environment"></param>
        public ServiceHost(Environment environment = null)
        {
            _autoResetEvent = new AutoResetEvent(false);
            _containerBuilder = new ContainerBuilder();
            _services = new ServiceCollection();
            // 环境变量
            Environment = environment ?? new Environment();
        }

        /// <summary>
        /// 添加配置
        /// </summary>
        /// <param name="configHandler"></param>
        /// <param name="key"></param>
        public void AddConfig(Action<IConfigurationBuilder> configHandler, string key = null)
        {
            Environment.AddConfig(configHandler, key);
        }

        /// <summary>
        /// 添加配置
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="key"></param>
        public void AddJsonConfig(string filePath, string key = null)
        {
            Environment.AddJsonConfig(filePath, key);
        }

        /// <summary>
        /// 添加配置
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="key"></param>
        public void AddJsonConfig(Stream stream, string key = null)
        {
            Environment.AddJsonConfig(stream, key);
        }

        /// <summary>
        /// 运行服务
        /// </summary>
        /// <returns></returns>
        public async Task RunAsync()
        {
            // 注册环境变量
            Environment.Build();
            RegisterInstance(Environment).SingleInstance();
            // 构建容器
            _containerBuilder.Populate(_services);
            Provider = new AutofacServiceProvider(_containerBuilder.Build());
            _logger = Provider.GetLogger<ServiceHost>();
            await _logger?.LogInformationAsync("Service is initializing!");
            OnInitializing?.Invoke(Provider);
            OnInitialized?.Invoke(Provider);
            await _logger?.LogInformationAsync("Service is initialized!");
            Console.CancelKeyPress += async (sender, args) => await StopAsync();
            await _logger?.LogInformationAsync("Service is running,Please enter \"Ctrl + C\" to stop!");
            _autoResetEvent.WaitOne();
        }

        /// <summary>
        /// 停止服务
        /// </summary>
        /// <returns></returns>
        public async Task StopAsync()
        {
            await _logger?.LogInformationAsync("Service is stopping!");
            OnStoping?.Invoke(Provider);
            OnStopped?.Invoke(Provider);
            await _logger?.LogInformationAsync("Service is stopped!");
            _autoResetEvent.Set();
        }

        /// <summary>
        /// 注册服务
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="func"></param>
        /// <returns></returns>
        public IRegistrationBuilder<T, SimpleActivatorData, SingleRegistrationStyle> Register<T>(Func<IComponentContext, T> func) where T : class
        {
            return _containerBuilder.Register(func).AsSelf();
        }

        /// <summary>
        /// 注册服务
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="I"></typeparam>
        /// <param name="func"></param>
        /// <returns></returns>
        public IRegistrationBuilder<T, SimpleActivatorData, SingleRegistrationStyle> Register<T, I>(Func<IComponentContext, T> func) where T : class where I : class
        {
            return Register(func).As<I>();
        }

        /// <summary>
        /// 注册服务
        /// </summary>
        /// <param name="action"></param>
        public void RegisterService(Action<ServiceCollection> action)
        {
            action?.Invoke(_services);
        }

        /// <summary>
        /// 注册服务
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public IModuleRegistrar RegisterModule<T>() where T : class, IModule, new()
        {
            return _containerBuilder.RegisterModule<T>();
        }

        /// <summary>
        /// 注册服务
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public IRegistrationBuilder<T, ConcreteReflectionActivatorData, SingleRegistrationStyle> RegisterType<T>() where T : class
        {
            return _containerBuilder.RegisterType<T>().AsSelf();
        }

        /// <summary>
        /// 注册服务
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public IRegistrationBuilder<object, ConcreteReflectionActivatorData, SingleRegistrationStyle> RegisterType(Type type)
        {
            return _containerBuilder.RegisterType(type).AsSelf();
        }

        /// <summary>
        /// 注册服务
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="I"></typeparam>
        /// <returns></returns>
        public IRegistrationBuilder<T, ConcreteReflectionActivatorData, SingleRegistrationStyle> RegisterType<T, I>() where T : class where I : class
        {
            return RegisterType<T>().As<I>();
        }

        /// <summary>
        /// 注册服务
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="instance"></param>
        /// <returns></returns>
        public IRegistrationBuilder<T, SimpleActivatorData, SingleRegistrationStyle> RegisterInstance<T>(T instance) where T : class
        {
            return _containerBuilder.RegisterInstance(instance).AsSelf();
        }

        /// <summary>
        /// 注册服务
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="I"></typeparam>
        /// <param name="instance"></param>
        /// <returns></returns>
        public IRegistrationBuilder<T, SimpleActivatorData, SingleRegistrationStyle> RegisterInstance<T, I>(T instance) where T : class where I : class
        {
            return RegisterInstance(instance).As<I>();
        }

        /// <summary>
        /// 注册服务
        /// </summary>
        /// <param name="genericType"></param>
        public IRegistrationBuilder<object, ReflectionActivatorData, DynamicRegistrationStyle> RegisterGeneric(Type genericType)
        {
            return _containerBuilder.RegisterGeneric(genericType);
        }

        /// <summary>
        /// 注册服务
        /// </summary>
        /// <param name="assembly"></param>
        public void RegisterFromAssembly(Assembly assembly)
        {
            var types = assembly.GetTypes();
            types.ForEach(item =>
            {
                var attr = item.GetCustomAttribute<RegisterableAttribute>();
                if (attr == null) return;
                var builder = _containerBuilder.RegisterType(item);
                if (attr.InterfaceType != null) builder.As(attr.InterfaceType);
                else builder.AsSelf();
                switch (attr.Lifetime)
                {
                    case Lifetime.Singleton:
                        builder.SingleInstance();
                        break;
                    case Lifetime.InstancePerLifetimeScope:
                        builder.InstancePerLifetimeScope();
                        break;
                }
                if (attr.IsAutoActivate) builder.AutoActivate();
            });
        }

        /// <summary>
        /// 注册服务
        /// </summary>
        /// <returns></returns>
        public void RegisterFromAssemblies()
        {
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            assemblies.ForEach(item => RegisterFromAssembly(item));
        }

        /// <summary>
        /// 创建生命周期
        /// </summary>
        /// <returns></returns>
        public static ILifetimeScope CreateLifetimeScope()
        {
            return Provider?.LifetimeScope.BeginLifetimeScope();
        }

        /// <summary>
        /// 创建生命周期
        /// </summary>
        /// <param name="tag"></param>
        /// <returns></returns>
        public static ILifetimeScope CreateLifetimeScope(object tag)
        {
            return Provider?.LifetimeScope.BeginLifetimeScope(tag);
        }
    }
}
