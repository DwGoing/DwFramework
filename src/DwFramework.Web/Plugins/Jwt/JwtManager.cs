using System;
using System.Text;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;

namespace DwFramework.Web.JWT
{
    public static class JwtManager
    {
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
        public static string Generate(string issuer, string securityKey, string[] audiences = null, DateTime? notBefore = null, DateTime? expires = null, Dictionary<string, object> customFields = null)
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
            if (customFields != null) foreach (var keyValuePair in customFields) jwtSecurityToken.Payload[keyValuePair.Key] = keyValuePair.Value;
            var token = tokenHandler.WriteToken(jwtSecurityToken);
            return token;
        }

        /// <summary>
        /// 验证Token
        /// </summary>
        /// <param name="token"></param>
        /// <param name="validationParameters"></param>
        /// <returns></returns>
        public static SecurityToken Verify(string token, TokenValidationParameters validationParameters)
        {
            new JwtSecurityTokenHandler().ValidateToken(token, validationParameters, out var securityToken);
            return securityToken;
        }

        /// <summary>
        /// 解析Token
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public static JwtSecurityToken Decode(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            if (string.IsNullOrEmpty(token) || !tokenHandler.CanReadToken(token)) throw new Exception("无效的Token");
            return tokenHandler.ReadJwtToken(token);
        }

        /// <summary>
        /// 读取Claims信息
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public static Dictionary<string, object> ReadClaims(string token) => Decode(token).Payload.ToDictionary(item => item.Key, item => item.Value);

        /// <summary>
        /// 读取Claims信息
        /// </summary>
        /// <param name="token"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static object ReadClaim(string token, string key) => Decode(token).Payload[key];
    }
}
