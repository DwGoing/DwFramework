using System;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Authentication.JwtBearer;


namespace DwFramework.WEB.Plugins
{
    public static class JwtExtension
    {
        /// <summary>
        /// 注入JWT服务
        /// </summary>
        /// <param name="services"></param>
        /// <param name="tokenValidator"></param>
        /// <param name="onSuccess"></param>
        /// <param name="onFail"></param>
        /// <returns></returns>
        public static IServiceCollection AddJwtAuthentication(this IServiceCollection services, ISecurityTokenValidator tokenValidator, Func<TokenValidatedContext, Task> onSuccess, Func<JwtBearerChallengeContext, Task> onFail)
        {
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.SecurityTokenValidators.Clear();
                    options.SecurityTokenValidators.Add(tokenValidator);
                    options.Events = new JwtBearerEvents
                    {
                        OnTokenValidated = onSuccess,
                        OnChallenge = onFail
                    };
                });
            return services;
        }
    }
}