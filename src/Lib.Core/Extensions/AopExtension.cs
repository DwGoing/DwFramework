using System;

using Autofac.Builder;
using Autofac.Extras.DynamicProxy;

using DwFramework.Core.Plugins;

namespace DwFramework.Core.Extensions
{
    public static class AopExtension
    {
        /// <summary>
        /// 注册拦截器
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="host"></param>
        public static void RegisterInterceptor<T>(this ServiceHost host) where T : BaseInterceptor
        {
            host.RegisterType<T>();
            Console.WriteLine(0);
        }

        /// <summary>
        /// 注册拦截器
        /// </summary>
        /// <param name="host"></param>
        /// <param name="interceptors"></param>
        public static void RegisterInterceptors(this ServiceHost host, params Type[] interceptors)
        {
            foreach (var item in interceptors)
            {
                if (item.BaseType == typeof(BaseInterceptor))
                    host.RegisterType(item);
            }
        }

        /// <summary>
        /// 添加类拦截器
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="builder"></param>
        /// <param name="interceptors"></param>
        public static void AddClassInterceptors<T>(this IRegistrationBuilder<T, ConcreteReflectionActivatorData, SingleRegistrationStyle> builder, params Type[] interceptors)
        {
            builder.InterceptedBy(interceptors).EnableClassInterceptors();
        }

        /// <summary>
        /// 添加类拦截器
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="interceptors"></param>
        public static void AddClassInterceptors(this IRegistrationBuilder<object, ConcreteReflectionActivatorData, SingleRegistrationStyle> builder, params Type[] interceptors)
        {
            builder.InterceptedBy(interceptors).EnableClassInterceptors();
        }

        /// <summary>
        /// 添加接口拦截器
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="builder"></param>
        /// <param name="interceptors"></param>
        public static void AddInterfaceInterceptors<T>(this IRegistrationBuilder<T, ConcreteReflectionActivatorData, SingleRegistrationStyle> builder, params Type[] interceptors)
        {
            builder.InterceptedBy(interceptors).EnableInterfaceInterceptors();
        }

        /// <summary>
        /// 添加接口拦截器
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="builder"></param>
        /// <param name="interceptors"></param>
        public static void AddInterfaceInterceptors<T>(this IRegistrationBuilder<T, SimpleActivatorData, SingleRegistrationStyle> builder, params Type[] interceptors)
        {
            builder.InterceptedBy(interceptors).EnableInterfaceInterceptors();
        }
    }
}
