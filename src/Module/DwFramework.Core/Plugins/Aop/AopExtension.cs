using System;
using Autofac.Builder;
using Autofac.Extras.DynamicProxy;

using DwFramework.Core.Extensions;

namespace DwFramework.Core.Plugins
{
    public static class AopExtension
    {
        /// <summary>
        /// 注册拦截器
        /// </summary>
        /// <param name="host"></param>
        /// <param name="interceptors"></param>
        /// <returns></returns>
        public static ServiceHost RegisterInterceptors(this ServiceHost host, params Type[] interceptors)
        {
            interceptors.ForEach(item =>
            {
                if (item.BaseType == typeof(BaseInterceptor)) host.RegisterType(item);
            });
            return host;
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
