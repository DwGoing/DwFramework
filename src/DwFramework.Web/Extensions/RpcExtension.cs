using System;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;

namespace DwFramework.Web.Rpc
{
    public static class RpcExtension
    {
        /// <summary>
        /// 添加Rpc服务
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddRpcImplements(this IServiceCollection services, params Type[] rpcImpls)
        {
            WebService.Instance.AddRpcImplements(services, rpcImpls);
            return services;
        }

        /// <summary>
        /// 匹配Rpc路由
        /// </summary>
        /// <param name="endpoints"></param>
        /// <returns></returns>
        public static IEndpointRouteBuilder MapRpcImplements(this IEndpointRouteBuilder endpoints)
        {
            WebService.Instance.MapRpcImplements(endpoints);
            return endpoints;
        }
    }
}