using System;
using System.Text;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;

using Microsoft.IdentityModel.Tokens;

namespace DwFramework.Core.Plugins
{
    public class JWTPlugin
    {
        private const string CustomPrefix = "Custom-";

        /// <summary>
        /// 生成Token
        /// </summary>
        /// <param name="issuer"></param>
        /// <param name="audience"></param>
        /// <param name="notBefore"></param>
        /// <param name="expires"></param>
        /// <param name="securiyKey"></param>
        /// <param name="customFields"></param>
        /// <returns></returns>
        public static string GenerateToken(string issuer, string audience, DateTime notBefore, DateTime expires, string securiyKey, Dictionary<string, object> customFields = null)
        {
            if (securiyKey.Length < 16)
                throw new Exception("SecuriyKey长度不足");
            JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
            var creds = new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(securiyKey)), SecurityAlgorithms.HmacSha256);
            var jwtSecurityToken = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                notBefore: notBefore,
                expires: expires,
                signingCredentials: creds
                );
            if (customFields != null)
            {
                foreach (var keyValuePair in customFields)
                    jwtSecurityToken.Payload[$"{CustomPrefix}{keyValuePair.Key}"] = keyValuePair.Value;
            }
            return tokenHandler.WriteToken(jwtSecurityToken);
        }

        /// <summary>
        /// 刷新Token
        /// </summary>
        /// <param name="token"></param>
        /// <param name="notBefore"></param>
        /// <param name="expires"></param>
        /// <param name="securiyKey"></param>
        /// <returns></returns>
        public static string RefreshToken(JwtSecurityToken token, DateTime notBefore, DateTime expires, string securiyKey)
        {
            var payload = token.Payload;
            var creds = new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(securiyKey)), SecurityAlgorithms.HmacSha256);
            var jwtSecurityToken = new JwtSecurityToken(
                issuer: payload.Iss,
                audience: payload.Aud.Single(),
                notBefore: notBefore,
                expires: expires,
                signingCredentials: creds
                );
            foreach (var keyValuePair in payload)
            {
                if (keyValuePair.Key.StartsWith(CustomPrefix))
                    jwtSecurityToken.Payload[keyValuePair.Key] = keyValuePair.Value;
            }
            return new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);
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
