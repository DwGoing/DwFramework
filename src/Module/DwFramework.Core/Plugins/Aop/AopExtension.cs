using System;
using Autofac;
using Autofac.Builder;
using Autofac.Extras.DynamicProxy;

namespace DwFramework.Core.Plugins
{
    public static class AopExtension
    {
        /// <summary>
        /// 注册拦截器
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="interceptors"></param>
        /// <returns></returns>
        public static ContainerBuilder RegisterInterceptors(this ContainerBuilder builder, params Type[] interceptors)
        {
            foreach (var item in interceptors) if (item.BaseType == typeof(InterceptorBase)) builder.RegisterType(item);
            return builder;
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
