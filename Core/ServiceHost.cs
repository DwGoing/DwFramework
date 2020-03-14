using System;
using System.Reflection;
using System.Linq;
using System.IO;

using Autofac;
using Autofac.Builder;
using Autofac.Core;
using Autofac.Core.Registration;
using Autofac.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;

namespace DwFramework.Core
{
    public class ServiceHost
    {
        private readonly ContainerBuilder _containerBuilder;
        private readonly ServiceCollection _services;

        /// <summary>
        /// 构造函数
        /// </summary>
        public ServiceHost(EnvironmentType environmentType = EnvironmentType.Develop, string configFilePath = null)
        {
            _containerBuilder = new ContainerBuilder();
            _services = new ServiceCollection();
            // 读取配置文件
            IConfiguration configuration = null;
            if (configFilePath != null && File.Exists(configFilePath))
                configuration = new ConfigurationBuilder().AddJsonFile(configFilePath).Build();
            // 环境变量
            RegisterInstance<IRunEnvironment, RunEnvironment>(new RunEnvironment(environmentType, configuration)).SingleInstance();
        }

        /// <summary>
        /// 注册服务
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="func"></param>
        /// <returns></returns>
        public IRegistrationBuilder<T, SimpleActivatorData, SingleRegistrationStyle> Register<T>(Func<IComponentContext, T> func)
        {
            return _containerBuilder.Register(func);
        }

        /// <summary>
        /// 注册服务
        /// </summary>
        /// <typeparam name="I"></typeparam>
        /// <typeparam name="T"></typeparam>
        /// <param name="func"></param>
        /// <returns></returns>
        public IRegistrationBuilder<T, SimpleActivatorData, SingleRegistrationStyle> Register<I, T>(Func<IComponentContext, T> func)
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
            return _containerBuilder.RegisterType<T>();
        }

        /// <summary>
        /// 注册服务
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public IRegistrationBuilder<object, ConcreteReflectionActivatorData, SingleRegistrationStyle> RegisterType(Type type)
        {
            return _containerBuilder.RegisterType(type);
        }

        /// <summary>
        /// 注册服务
        /// </summary>
        /// <typeparam name="I"></typeparam>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public IRegistrationBuilder<T, ConcreteReflectionActivatorData, SingleRegistrationStyle> RegisterType<I, T>() where T : class
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
        /// <typeparam name="I"></typeparam>
        /// <typeparam name="T"></typeparam>
        /// <param name="instance"></param>
        /// <returns></returns>
        public IRegistrationBuilder<T, SimpleActivatorData, SingleRegistrationStyle> RegisterInstance<I, T>(T instance) where T : class
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
                var attr = type.GetCustomAttribute<RegisterableAttribute>() as RegisterableAttribute;
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
                    var attr = type.GetCustomAttribute<RegisterableAttribute>() as RegisterableAttribute;
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
        /// 构建服务主机
        /// </summary>
        /// <returns></returns>
        public AutofacServiceProvider Build()
        {
            _containerBuilder.Populate(_services);
            return new AutofacServiceProvider(_containerBuilder.Build());
        }
    }
}
