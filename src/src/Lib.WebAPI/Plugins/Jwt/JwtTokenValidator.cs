using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

using Microsoft.IdentityModel.Tokens;

namespace DwFramework.WebAPI.Plugins
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
}
