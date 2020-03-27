using System;
using System.Text;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Claims;

using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;

using DwFramework.Core.Plugins;

namespace DwFramework.Http.Plugins
{
    public abstract class CustomSecurityTokenValidator : ISecurityTokenValidator
    {
        private readonly JwtSecurityTokenHandler _tokenHandler;

        public bool CanValidateToken { get => _tokenHandler.CanValidateToken; }
        public int MaximumTokenSizeInBytes { get; set; } = TokenValidationParameters.DefaultMaximumTokenSizeInBytes;

        public string CurrentIssuer { get; private set; }
        public string CurrentAudience { get; private set; }
        public string CurrentSecurityKey { get; private set; }

        /// <summary>
        /// 构造函数
        /// </summary>
        public CustomSecurityTokenValidator() => _tokenHandler = new JwtSecurityTokenHandler();

        /// <summary>
        /// 是否可读
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public bool CanReadToken(string token) => _tokenHandler.CanReadToken(token);

        #region 验证内容
        public void SetIssuerValidation(string issuer) => CurrentIssuer = issuer;
        public void ClearIssuerValidation() => CurrentIssuer = null;
        public void SetAudienceValidation(string audience) => CurrentAudience = audience;
        public void ClearAudienceValidation() => CurrentAudience = null;
        public void SetSecurityKeyValidation(string securityKey) => CurrentSecurityKey = securityKey;
        public void ClearSecurityKeyValidation() => CurrentSecurityKey = null;
        #endregion

        /// <summary>
        /// 验证Token
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public abstract bool ValidateToken(JwtSecurityToken token);

        /// <summary>
        /// 验证Token
        /// </summary>
        /// <param name="securityToken"></param>
        /// <param name="validationParameters"></param>
        /// <param name="validatedToken"></param>
        /// <returns></returns>
        public virtual ClaimsPrincipal ValidateToken(string securityToken, TokenValidationParameters validationParameters, out SecurityToken validatedToken)
        {
            if (string.IsNullOrEmpty(CurrentSecurityKey))
                throw new Exception("未设置SecurityKey");
            if (!JwtManager.IsInvalid(securityToken) || !ValidateToken(JwtManager.DecodeToken(securityToken)))
                throw new Exception("验证失败");

            validationParameters.IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(CurrentSecurityKey));
            if (string.IsNullOrEmpty(CurrentIssuer)) validationParameters.ValidateIssuer = false; else validationParameters.ValidIssuer = CurrentIssuer;
            if (string.IsNullOrEmpty(CurrentAudience)) validationParameters.ValidateAudience = false; else validationParameters.ValidAudience = CurrentAudience;

            return _tokenHandler.ValidateToken(securityToken, validationParameters, out validatedToken);
        }
    }

    public static class JwtManager
    {
        /// <summary>
        /// 注入JWT服务
        /// </summary>
        /// <param name="services"></param>
        /// <param name="tokenValidator"></param>
        /// <param name="onSuccess"></param>
        /// <param name="onFail"></param>
        /// <returns></returns>
        public static IServiceCollection AddJWTAuthentication(this IServiceCollection services, ISecurityTokenValidator tokenValidator, Func<TokenValidatedContext, Task> onSuccess, Func<JwtBearerChallengeContext, Task> onFail)
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
        /// 使用JWT服务
        /// </summary>
        /// <param name="app"></param>
        /// <returns></returns>
        public static IApplicationBuilder UseJWTAuthentication(this IApplicationBuilder app)
        {
            app.UseAuthentication();
            app.UseAuthorization();
            return app;
        }

        private const string CustomPrefix = "Custom-";
        public static string Tag { get; private set; }
        private static readonly Dictionary<string, string> _tokens;

        /// <summary>
        /// 构造函数
        /// </summary>
        static JwtManager()
        {
            Tag = Generater.GenerateRandomString(16);
            _tokens = new Dictionary<string, string>();
        }

        /// <summary>
        /// 生成Token
        /// </summary>
        /// <param name="issuer"></param>
        /// <param name="audience"></param>
        /// <param name="notBefore"></param>
        /// <param name="expires"></param>
        /// <param name="securityKey"></param>
        /// <param name="customFields"></param>
        /// <returns></returns>
        public static string GenerateToken(string issuer, string audience, DateTime notBefore, DateTime expires, string securityKey, Dictionary<string, object> customFields = null, string nonce = null)
        {
            if (securityKey.Length < 16)
                throw new Exception("SecuriyKey长度不足");
            JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
            var creds = new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(securityKey)), SecurityAlgorithms.HmacSha256);
            var jwtSecurityToken = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                notBefore: notBefore,
                expires: expires,
                signingCredentials: creds
                );
            // 自定义内容
            nonce ??= Generater.GenerateRandomString(16);
            jwtSecurityToken.Header["tag"] = Tag;
            jwtSecurityToken.Header["nonce"] = nonce;
            if (customFields != null)
            {
                foreach (var keyValuePair in customFields)
                    jwtSecurityToken.Payload[$"{CustomPrefix}{keyValuePair.Key}"] = keyValuePair.Value;
            }
            var token = tokenHandler.WriteToken(jwtSecurityToken);
            _tokens[nonce] = token;
            return token;
        }

        /// <summary>
        /// 刷新Token
        /// </summary>
        /// <param name="token"></param>
        /// <param name="notBefore"></param>
        /// <param name="expires"></param>
        /// <param name="securityKey"></param>
        /// <returns></returns>
        public static string RefreshToken(string token, DateTime notBefore, DateTime expires, string securityKey)
        {
            // 验证
            var parameters = new TokenValidationParameters()
            {
                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateLifetime = false,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(securityKey))
            };
            new JwtSecurityTokenHandler().ValidateToken(token, parameters, out SecurityToken validatedToken);
            var sourceToken = DecodeToken(token);
            var header = sourceToken.Header;
            var payload = sourceToken.Payload;
            if (!header.TryGetValue("nonce", out object nonce) || string.IsNullOrEmpty(nonce.ToString()))
                throw new Exception("无效的Token");
            // 注销Token
            RemoveToken(token);
            // 生成新Token
            return GenerateToken(sourceToken.Issuer, sourceToken.Audiences.Single(), notBefore, expires, securityKey, ReadCustomFields(token), header["nonce"].ToString());
        }

        /// <summary>
        /// 注销Token
        /// </summary>
        /// <param name="token"></param>
        public static void RemoveToken(string token)
        {
            var sourceToken = DecodeToken(token);
            if (!sourceToken.Header.TryGetValue("nonce", out object nonce) || string.IsNullOrEmpty(nonce.ToString()))
                throw new Exception("无效的Token");
            if (_tokens.ContainsKey(nonce.ToString())) _tokens.Remove(nonce.ToString());
        }

        /// <summary>
        /// 检查Token是否有效
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public static bool IsInvalid(string token)
        {
            var sourceToken = DecodeToken(token);
            if (!sourceToken.Header.TryGetValue("nonce", out object nonce) || string.IsNullOrEmpty(nonce.ToString()))
                throw new Exception("无效的Token");
            return _tokens.ContainsKey(nonce.ToString());
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
        /// 读取自定义信息
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public static Dictionary<string, object> ReadCustomFields(string token)
        {
            var jwt = DecodeToken(token);
            Dictionary<string, object> customFields = new Dictionary<string, object>();
            foreach (var keyValuePair in jwt.Payload)
            {
                if (keyValuePair.Key.StartsWith(CustomPrefix))
                    customFields[keyValuePair.Key.Replace(CustomPrefix, "")] = keyValuePair.Value;
            }
            return customFields;
        }
    }
}
