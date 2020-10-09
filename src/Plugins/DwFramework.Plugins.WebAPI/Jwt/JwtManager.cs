using System;
using System.Text;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Threading.Tasks;
using System.Linq;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Authentication.JwtBearer;

using DwFramework.Core.Plugins;

namespace DwFramework.WebAPI.Jwt
{
    public static class JwtManager
    {
        public static string Tag { get; private set; }

        /// <summary>
        /// 构造函数
        /// </summary>
        static JwtManager()
        {
            Tag = Generater.GenerateUUID();
        }

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

        /// <summary>
        /// 生成Token
        /// </summary>
        /// <param name="issuer"></param>
        /// <param name="securityKey"></param>
        /// <param name="audiences"></param>
        /// <param name="notBefore"></param>
        /// <param name="expires"></param>
        /// <param name="customFields"></param>
        /// <returns></returns>
        public static string GenerateToken(string issuer, string securityKey, string[] audiences = null, DateTime? notBefore = null, DateTime? expires = null, Dictionary<string, object> customFields = null)
        {
            if (securityKey.Length < 16) throw new Exception("SecuriyKey长度不足");
            JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
            var claims = new List<Claim>();
            if (audiences != null) foreach (var item in audiences) claims.Add(new Claim("aud", item));
            var creds = new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(securityKey)), SecurityAlgorithms.HmacSha256);
            var jwtSecurityToken = new JwtSecurityToken(
                issuer: issuer,
                claims: claims,
                notBefore: notBefore,
                expires: expires,
                signingCredentials: creds
                );
            // 自定义内容
            jwtSecurityToken.Header["tag"] = Tag;
            if (customFields != null) foreach (var keyValuePair in customFields) jwtSecurityToken.Payload[keyValuePair.Key] = keyValuePair.Value;
            var token = tokenHandler.WriteToken(jwtSecurityToken);
            return token;
        }

        /// <summary>
        /// 刷新Token
        /// </summary>
        /// <param name="token"></param>
        /// <param name="securityKey"></param>
        /// <param name="notBefore"></param>
        /// <param name="expires"></param>
        /// <returns></returns>
        public static string RefreshToken(string token, string securityKey, DateTime? notBefore = null, DateTime? expires = null)
        {
            // 验证
            ValidateSecurityKey(token, securityKey);
            var sourceToken = DecodeToken(token);
            // 生成新Token
            return GenerateToken(sourceToken.Issuer, securityKey, sourceToken.Audiences.ToArray(), notBefore, expires, ReadClaims(token));
        }

        /// <summary>
        /// 验证SecurityKey及Lifetime
        /// </summary>
        /// <param name="token"></param>
        /// <param name="securityKey"></param>
        /// <param name="validateLifetime"></param>
        private static void ValidateSecurityKey(string token, string securityKey, bool validateLifetime = false)
        {
            var parameters = new TokenValidationParameters()
            {
                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateLifetime = validateLifetime,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(securityKey))
            };
            new JwtSecurityTokenHandler().ValidateToken(token, parameters, out _);
        }

        /// <summary>
        /// 解析Token
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public static JwtSecurityToken DecodeToken(string token)
        {
            JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
            if (string.IsNullOrEmpty(token) || !tokenHandler.CanReadToken(token))
                throw new Exception("无效的Token");
            return tokenHandler.ReadJwtToken(token);
        }

        /// <summary>
        /// 读取Claims信息
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public static Dictionary<string, object> ReadClaims(string token)
        {
            var jwt = DecodeToken(token);
            return jwt.Payload.ToDictionary(item => item.Key, item => item.Value);
        }

        /// <summary>
        /// 读取Claims信息
        /// </summary>
        /// <param name="token"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static object ReadClaim(string token, string key)
        {
            var jwt = DecodeToken(token);
            return jwt.Payload[key];
        }
    }
}
