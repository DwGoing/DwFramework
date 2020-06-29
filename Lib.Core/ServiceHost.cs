using System;
using System.Reflection;
using System.Linq;
using System.Collections.Generic;
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
        private readonly List<Action<AutofacServiceProvider>> _initActions;

        public static AutofacServiceProvider Provider;

        /// <summary>
        /// 构造函数
        /// </summary>
        public ServiceHost(EnvironmentType environmentType = EnvironmentType.Develop, string configFilePath = null)
        {
            _autoResetEvent = new AutoResetEvent(false);
            _containerBuilder = new ContainerBuilder();
            _services = new ServiceCollection();
            _initActions = new List<Action<AutofacServiceProvider>>();
            // 环境变量
            RegisterInstance<Environment, IEnvironment>(new Environment(environmentType, configFilePath)).SingleInstance();
        }

        /// <summary>
        /// 开启服务
        /// </summary>
        public void Run()
        {
            _containerBuilder.Populate(_services);
            Provider = new AutofacServiceProvider(_containerBuilder.Build());
            foreach (var item in _initActions) item.Invoke(Provider);
            Console.WriteLine("Services Is Running,Please Enter \"Ctrl + C\" To Stop!");
            _autoResetEvent.WaitOne();
            Console.WriteLine("Services Is Stop!");
        }

        /// <summary>
        /// 停止服务
        /// </summary>
        public void Stop()
        {
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
            return _containerBuilder.Register(func);
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
            return _containerBuilder.Register(func).As<I>();
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
            return _containerBuilder.RegisterType<T>().As<T>();
        }

        /// <summary>
        /// 注册服务
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public IRegistrationBuilder<object, ConcreteReflectionActivatorData, SingleRegistrationStyle> RegisterType(Type type)
        {
            return _containerBuilder.RegisterType(type).As(type);
        }

        /// <summary>
        /// 注册服务
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="I"></typeparam>
        /// <returns></returns>
        public IRegistrationBuilder<T, ConcreteReflectionActivatorData, SingleRegistrationStyle> RegisterType<T, I>() where T : class where I : class
        {
            return _containerBuilder.RegisterType<T>().As<I>();
        }

        /// <summary>
        /// 注册服务
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="instance"></param>
        /// <returns></returns>
        public IRegistrationBuilder<T, SimpleActivatorData, SingleRegistrationStyle> RegisterInstance<T>(T instance) where T : class
        {
            return _containerBuilder.RegisterInstance(instance);
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
            return _containerBuilder.RegisterInstance(instance).As<I>();
        }

        /// <summary>
        /// 注册服务
        /// </summary>
        /// <param name="assemblyName"></param>
        /// <returns></returns>
        public void RegisterFromAssembly(string assemblyName)
        {
            var assembly = AppDomain.CurrentDomain.GetAssemblies().Where(item => item.FullName.Split(",").First() == assemblyName).FirstOrDefault();
            if (assembly == null)
                throw new Exception("未找到该程序集");
            var types = assembly.GetTypes();
            foreach (var type in types)
            {
                var attr = type.GetCustomAttribute<RegisterableAttribute>();
                if (attr == null)
                    continue;
                var builder = _containerBuilder.RegisterType(type);
                if (attr.InterfaceType != null)
                    builder.As(attr.InterfaceType);
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
        /// <returns></returns>
        public void RegisterFromAssemblies()
        {
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            foreach (var assembly in assemblies)
            {
                var types = assembly.GetTypes();
                foreach (var type in types)
                {
                    var attr = type.GetCustomAttribute<RegisterableAttribute>();
                    if (attr == null)
                        continue;
                    var builder = _containerBuilder.RegisterType(type);
                    if (attr.InterfaceType != null)
                        builder.As(attr.InterfaceType);
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
        }

        /// <summary>
        /// 初始化服务
        /// </summary>
        /// <param name="initAction"></param>
        public void InitService(Action<AutofacServiceProvider> initAction)
        {
            _initActions.Add(initAction);
        }
    }
}
