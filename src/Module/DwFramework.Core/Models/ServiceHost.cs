using System;
using System.Reflection;
using System.Threading;
using Autofac;
using Autofac.Builder;
using Autofac.Core;
using Autofac.Core.Registration;
using Autofac.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

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
        /// <param name="configFilePath"></param>
        public ServiceHost(EnvironmentType environmentType = EnvironmentType.Develop, string configFilePath = null)
        {
            _autoResetEvent = new AutoResetEvent(false);
            _containerBuilder = new ContainerBuilder();
            _services = new ServiceCollection();
            // 环境变量
            Environment = new Environment(environmentType, configFilePath);
        }

        /// <summary>
        /// 添加配置
        /// </summary>
        /// <param name="configFilePath"></param>
        /// <param name="key"></param>
        /// <param name="onChange"></param>
        public static void AddJsonConfig(string configFilePath, string key = null, Action onChange = null) => Environment?.AddJsonConfig(configFilePath, key, onChange);

        /// <summary>
        /// 开启服务
        /// </summary>
        public void Run()
        {
            // 注册环境变量
            Environment.Build();
            RegisterInstance(Environment).SingleInstance();
            // 构建容器
            _containerBuilder.Populate(_services);
            Provider = new AutofacServiceProvider(_containerBuilder.Build());
            _logger = Provider.GetLogger<ServiceHost>();
            OnInitializing?.Invoke(Provider);
            _logger?.LogInformationAsync("Service is running,Please enter \"Ctrl + C\" to stop!");
            OnInitialized?.Invoke(Provider);
            Console.CancelKeyPress += (sender, args) => Stop();
            _autoResetEvent.WaitOne();
        }

        /// <summary>
        /// 停止服务
        /// </summary>
        public void Stop()
        {
            _logger?.LogInformationAsync("Service is Stopping!");
            OnStoping?.Invoke(Provider);
            _logger?.LogInformationAsync("Service is stopped!");
            OnStopped?.Invoke(Provider);
            _autoResetEvent.Set();
        }

        /// <summary>
        /// 注册服务
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="func"></param>
        /// <returns></returns>
        public IRegistrationBuilder<T, SimpleActivatorData, SingleRegistrationStyle> Register<T>(Func<IComponentContext, T> func) where T : class => _containerBuilder.Register(func).AsSelf();

        /// <summary>
        /// 注册服务
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="I"></typeparam>
        /// <param name="func"></param>
        /// <returns></returns>
        public IRegistrationBuilder<T, SimpleActivatorData, SingleRegistrationStyle> Register<T, I>(Func<IComponentContext, T> func) where T : class where I : class => Register(func).As<I>();

        /// <summary>
        /// 注册服务
        /// </summary>
        /// <param name="action"></param>
        public void RegisterService(Action<ServiceCollection> action) => action?.Invoke(_services);

        /// <summary>
        /// 注册服务
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public IModuleRegistrar RegisterModule<T>() where T : class, IModule, new() => _containerBuilder.RegisterModule<T>();

        /// <summary>
        /// 注册服务
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public IRegistrationBuilder<T, ConcreteReflectionActivatorData, SingleRegistrationStyle> RegisterType<T>() where T : class => _containerBuilder.RegisterType<T>().AsSelf();

        /// <summary>
        /// 注册服务
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public IRegistrationBuilder<object, ConcreteReflectionActivatorData, SingleRegistrationStyle> RegisterType(Type type) => _containerBuilder.RegisterType(type).AsSelf();

        /// <summary>
        /// 注册服务
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="I"></typeparam>
        /// <returns></returns>
        public IRegistrationBuilder<T, ConcreteReflectionActivatorData, SingleRegistrationStyle> RegisterType<T, I>() where T : class where I : class => RegisterType<T>().As<I>();

        /// <summary>
        /// 注册服务
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="instance"></param>
        /// <returns></returns>
        public IRegistrationBuilder<T, SimpleActivatorData, SingleRegistrationStyle> RegisterInstance<T>(T instance) where T : class => _containerBuilder.RegisterInstance(instance).AsSelf();

        /// <summary>
        /// 注册服务
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="I"></typeparam>
        /// <param name="instance"></param>
        /// <returns></returns>
        public IRegistrationBuilder<T, SimpleActivatorData, SingleRegistrationStyle> RegisterInstance<T, I>(T instance) where T : class where I : class => RegisterInstance(instance).As<I>();

        /// <summary>
        /// 注册服务
        /// </summary>
        /// <param name="genericType"></param>
        public IRegistrationBuilder<object, ReflectionActivatorData, DynamicRegistrationStyle> RegisterGeneric(Type genericType) => _containerBuilder.RegisterGeneric(genericType);

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
        public static ILifetimeScope CreateLifetimeScope() => Provider.LifetimeScope.BeginLifetimeScope();

        /// <summary>
        /// 创建生命周期
        /// </summary>
        /// <param name="tag"></param>
        /// <returns></returns>
        public static ILifetimeScope CreateLifetimeScope(object tag) => Provider.LifetimeScope.BeginLifetimeScope(tag);
    }
}
