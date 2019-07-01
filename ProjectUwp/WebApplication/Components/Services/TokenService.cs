using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using WebApplication.Components.Services.Interfaces;

namespace WebApplication.Components.Services
{
    public class TokenService : ITokenService
    {
        private readonly SigningCredentials _cert;
        private readonly SymmetricSecurityKey _key;
        public TokenService(byte[] key)
        {
            _key = new SymmetricSecurityKey(key);
            _cert = new SigningCredentials(_key, SecurityAlgorithms.HmacSha256);
        }
        public string GenerateToken(IEnumerable<Claim> claims, int time)
        {
            var token = new JwtSecurityToken(
                    issuer: "localhost",
                    audience: "localhost",
                    claims: claims,
                    expires: DateTime.Now.AddDays(time),
                    signingCredentials: _cert
                    );
            var rettoken = new JwtSecurityTokenHandler().WriteToken(token);
            return rettoken;
        }

        public JwtSecurityToken DecodeToken(string stream)
        {
            var handler = new JwtSecurityTokenHandler();
            var jsonToken = handler.ReadJwtToken(stream);

            return jsonToken;
        }

        public JwtSecurityToken DecodeAndValidateToken(string stream)
        {
            var handler = new JwtSecurityTokenHandler();
            var param = new TokenValidationParameters()
            {
                IssuerSigningKey = _key,
                ValidAudience = "localhost",
                ValidIssuer = "localhost",
                ValidateLifetime = true,
                ValidateAudience = true,
                ValidateIssuer = true,
                ValidateIssuerSigningKey = true
            };
            SecurityToken token;
            handler.ValidateToken(stream, param, out token);

            return (JwtSecurityToken)token;
        }
    }
}
