using System;
using System.Text;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;

using Microsoft.IdentityModel.Tokens;

using DwFramework.Core.Plugins;

namespace DwFramework.Web.Plugins
{
    public abstract class JwtTokenValidator : ISecurityTokenValidator
    {
        private readonly JwtSecurityTokenHandler _tokenHandler;

        public bool CanValidateToken { get => _tokenHandler.CanValidateToken; }
        public int MaximumTokenSizeInBytes { get; set; } = TokenValidationParameters.DefaultMaximumTokenSizeInBytes;

        /// <summary>
        /// 构造函数
        /// </summary>
        public JwtTokenValidator() => _tokenHandler = new JwtSecurityTokenHandler();

        /// <summary>
        /// 是否可读
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public bool CanReadToken(string token) => _tokenHandler.CanReadToken(token);

        /// <summary>
        /// 验证参数处理
        /// </summary>
        /// <param name="validationParameters"></param>
        public abstract void ParametersHandler(TokenValidationParameters validationParameters);

        /// <summary>
        /// 验证Token
        /// </summary>
        /// <param name="securityToken"></param>
        /// <param name="validationParameters"></param>
        /// <param name="validatedToken"></param>
        /// <returns></returns>
        public virtual ClaimsPrincipal ValidateToken(string securityToken, TokenValidationParameters validationParameters, out SecurityToken validatedToken)
        {
            ParametersHandler(validationParameters);
            return _tokenHandler.ValidateToken(securityToken, validationParameters, out validatedToken);
        }
    }

    public static class JwtManager
    {
        /// <summary>
        /// 默认验证器
        /// </summary>
        public class DefaultJwtTokenValidator : JwtTokenValidator
        {
            private string _securityKey;

            public DefaultJwtTokenValidator(string securityKey) => _securityKey = securityKey;

            public override void ParametersHandler(TokenValidationParameters validationParameters)
            {
                validationParameters.ValidateIssuer = false;
                validationParameters.ValidateAudience = false;
                validationParameters.ValidateLifetime = true;
                validationParameters.ClockSkew = TimeSpan.Zero;
                validationParameters.ValidateIssuerSigningKey = true;
                validationParameters.IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_securityKey));
            }
        }

        public static string Tag { get; private set; }

        /// <summary>
        /// 构造函数
        /// </summary>
        static JwtManager()
        {
            Tag = Generater.GenerateUUID();
        }

        /// <summary>
        /// 生成Token
        /// </summary>
        /// <param name="issuer"></param>
        /// <param name="audiences"></param>
        /// <param name="notBefore"></param>
        /// <param name="expires"></param>
        /// <param name="securityKey"></param>
        /// <param name="customFields"></param>
        /// <returns></returns>
        public static string GenerateToken(string issuer, string[] audiences, DateTime notBefore, DateTime expires, string securityKey, Dictionary<string, object> customFields = null)
        {
            if (securityKey.Length < 16)
                throw new Exception("SecuriyKey长度不足");
            JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
            var creds = new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(securityKey)), SecurityAlgorithms.HmacSha256);
            var jwtSecurityToken = new JwtSecurityToken(
                issuer: issuer,
                notBefore: notBefore,
                expires: expires,
                signingCredentials: creds
                );
            foreach (var item in audiences)
            {
                jwtSecurityToken.Audiences.Append(item);
            }
            // 自定义内容
            jwtSecurityToken.Header["tag"] = Tag;
            if (customFields != null)
            {
                foreach (var keyValuePair in customFields)
                    jwtSecurityToken.Payload[keyValuePair.Key] = keyValuePair.Value;
            }
            var token = tokenHandler.WriteToken(jwtSecurityToken);
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
            ValidateSecurityKey(token, securityKey);
            var sourceToken = DecodeToken(token);
            // 生成新Token
            return GenerateToken(sourceToken.Issuer, sourceToken.Audiences.ToArray(), notBefore, expires, securityKey, ReadClaims(token));
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
