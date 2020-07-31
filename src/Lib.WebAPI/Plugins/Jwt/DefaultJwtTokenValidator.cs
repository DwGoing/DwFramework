using System;
using System.Text;

using Microsoft.IdentityModel.Tokens;

namespace DwFramework.WebAPI.Plugins
{
    /// <summary>
    /// 默认验证器
    /// </summary>
    public class DefaultJwtTokenValidator : JwtTokenValidator
    {
        private readonly string _securityKey;

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
}
