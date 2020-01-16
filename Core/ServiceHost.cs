using System;
using System.Reflection;
using System.Linq;

using Autofac;
using Autofac.Core;
using Autofac.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

using DwFramework.Core.Models;

namespace DwFramework.Core
{
    public class ServiceHost
    {
        public static AutofacServiceProvider ServiceProvider { get; private set; }

        private ContainerBuilder _containerBuilder;
        private ServiceCollection _services;

        /// <summary>
        /// 构造函数
        /// </summary>
        public ServiceHost()
        {
            _containerBuilder = new ContainerBuilder();
            _services = new ServiceCollection();
        }

        /// <summary>
        /// 注册服务
        /// </summary>
        /// <param name="action"></param>
        public ServiceHost RegisterService(Action<ServiceCollection> action)
        {
            action?.Invoke(_services);
            return this;
        }

        /// <summary>
        /// 注册服务
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public ServiceHost RegisterModule<T>() where T : class, IModule, new()
        {
            _containerBuilder.RegisterModule<T>();
            return this;
        }

        /// <summary>
        /// 注册服务
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public ServiceHost RegisterType<T>() where T : class
        {
            _containerBuilder.RegisterType<T>();
            return this;
        }

        /// <summary>
        /// 注册服务
        /// </summary>
        /// <typeparam name="I"></typeparam>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public ServiceHost RegisterType<I, T>() where T : class
        {
            _containerBuilder.RegisterType<T>().As<I>();
            return this;
        }

        /// <summary>
        /// 注册服务
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="instance"></param>
        /// <returns></returns>
        public ServiceHost RegisterInstance<T>(T instance) where T : class
        {
            _containerBuilder.RegisterInstance(instance);
            return this;
        }

        /// <summary>
        /// 注册服务
        /// </summary>
        /// <typeparam name="I"></typeparam>
        /// <typeparam name="T"></typeparam>
        /// <param name="instance"></param>
        /// <returns></returns>
        public ServiceHost RegisterInstance<I, T>(T instance) where T : class
        {
            _containerBuilder.RegisterInstance(instance).As<I>();
            return this;
        }

        /// <summary>
        /// 注册服务
        /// </summary>
        /// <param name="assemblyName"></param>
        /// <returns></returns>
        public ServiceHost RegisterFromAssembly(string assemblyName)
        {
            var assembly = AppDomain.CurrentDomain.GetAssemblies().Where(item => item.FullName.Split(",").First() == assemblyName).FirstOrDefault();
            if (assembly == null)
                throw new Exception("未找到该程序集");
            var types = assembly.GetTypes();
            foreach (var item in types)
            {
                var attr = item.GetCustomAttribute(typeof(RegisterableAttribute)) as RegisterableAttribute;
                if (attr != null)
                {
                    var tmp = _containerBuilder.RegisterType(item).As(attr.InterfaceType);
                    switch (attr.Lifetime)
                    {
                        case Lifetime.Singleton:
                            tmp.SingleInstance();
                            break;
                        case Lifetime.InstancePerLifetimeScope:
                            tmp.InstancePerLifetimeScope();
                            break;
                    }
                    if (attr.IsAutoActivate)
                        tmp.AutoActivate();
                }
            }
            return this;
        }

        /// <summary>
        /// 构建服务主机
        /// </summary>
        /// <returns></returns>
        public AutofacServiceProvider Build()
        {
            _containerBuilder.Populate(_services);
            ServiceProvider = new AutofacServiceProvider(_containerBuilder.Build());
            return ServiceProvider;
        }
    }
}
