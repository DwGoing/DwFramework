using System;
using System.Reflection;
using System.Linq;
using System.Threading;

using Autofac;
using Autofac.Builder;
using Autofac.Core;
using Autofac.Core.Registration;
using Autofac.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

namespace DwFramework.Core
{
    public class ServiceHost
    {
        private readonly AutoResetEvent _autoResetEvent;
        private readonly ContainerBuilder _containerBuilder;
        private readonly ServiceCollection _services;

        public static Environment Environment { get; private set; }
        public static AutofacServiceProvider Provider { get; private set; }
        public event Action<AutofacServiceProvider> OnInitializing;
        public event Action<AutofacServiceProvider> OnInitialized;
        public event Action<AutofacServiceProvider> OnStoping;
        public event Action<AutofacServiceProvider> OnStopped;

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
        /// <param name="fileName"></param>
        /// <param name="onChange"></param>
        public void AddJsonConfig(string fileName, Action onChange = null) => Environment.AddJsonConfig(fileName, onChange);

        /// <summary>
        /// 开启服务
        /// </summary>
        public void Run()
        {
            Environment.Build();
            _containerBuilder.Populate(_services);
            Provider = new AutofacServiceProvider(_containerBuilder.Build());
            OnInitializing?.Invoke(Provider);
            Console.WriteLine("Service is running,Please enter \"Ctrl + C\" to stop!");
            OnInitialized?.Invoke(Provider);
            Console.CancelKeyPress += (sender, args) => Stop();
            _autoResetEvent.WaitOne();
        }

        /// <summary>
        /// 停止服务
        /// </summary>
        public void Stop()
        {
            Console.WriteLine("Service is Stopping!");
            OnStoping?.Invoke(Provider);
            Console.WriteLine("Service is stopped!");
            OnStopped?.Invoke(Provider);
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
        /// <param name="assembly"></param>
        public void RegisterFromAssembly(Assembly assembly)
        {
            var types = assembly.GetTypes();
            foreach (var type in types)
            {
                var attr = type.GetCustomAttribute<RegisterableAttribute>();
                if (attr == null) continue;
                var builder = _containerBuilder.RegisterType(type);
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
                if (attr.IsAutoActivate)
                    builder.AutoActivate();
            }
        }

        /// <summary>
        /// 注册服务
        /// </summary>
        /// <param name="assemblyName"></param>
        /// <returns></returns>
        public void RegisterFromAssembly(string assemblyName)
        {
            var assembly = AppDomain.CurrentDomain.GetAssemblies().Where(item => item.FullName.Split(",").First() == assemblyName).FirstOrDefault();
            if (assembly == null) throw new Exception("未找到该程序集");
            RegisterFromAssembly(assembly);
        }

        /// <summary>
        /// 注册服务
        /// </summary>
        /// <returns></returns>
        public void RegisterFromAssemblies()
        {
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            foreach (var item in assemblies) RegisterFromAssembly(item);
        }

        /// <summary>
        /// 创建生命周期
        /// </summary>
        /// <returns></returns>
        public static ILifetimeScope CreateLifetimeScope()
        {
            return Provider.LifetimeScope.BeginLifetimeScope();
        }

        /// <summary>
        /// 创建生命周期
        /// </summary>
        /// <param name="tag"></param>
        /// <returns></returns>
        public static ILifetimeScope CreateLifetimeScope(object tag)
        {
            return Provider.LifetimeScope.BeginLifetimeScope(tag);
        }
    }
}
