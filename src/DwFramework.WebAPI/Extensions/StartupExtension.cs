using System;
using Microsoft.Extensions.DependencyInjection;

namespace EthListener
{
    public static class StartupExtension
    {
        /// <summary>
        /// 添加控制器
        /// </summary>
        public static IMvcBuilder AddControllersWithDefaultOptions(this IServiceCollection services)
        {
            return services.AddControllers(options =>
            {
                options.Filters.Add<ResultFilter>();
                options.Filters.Add<ExceptionFilter>();
            }).AddJsonOptions(options =>
            {
                //不使用驼峰样式的key
                options.JsonSerializerOptions.PropertyNamingPolicy = null;
                //不使用驼峰样式的key
                options.JsonSerializerOptions.DictionaryKeyPolicy = null;
            });
        }
    }
}