using AtConnect.BLL.Options;
using AtConnect.Core.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace AtConnect.BLL.Helper
{
    public static class TokenHelper
    {

        public static string CreateJWT(IdentityUser user, IConfiguration configuration, JwtOptions JwtOptions)
        {
            var Claims = new Claim[]
            {
                new(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new(ClaimTypes.Name, user.UserName)
            };
            var JWTHandler = new JwtSecurityTokenHandler();
            var SecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(JwtOptions.SigningKey ?? ""));
            var SigningCredentials = new SigningCredentials(SecurityKey, SecurityAlgorithms.HmacSha256);
            var TokenDescriptor = new SecurityTokenDescriptor
            {
                Issuer = JwtOptions.Issuer,
                Audience = JwtOptions.Audience,
                Subject = new ClaimsIdentity(Claims),
                SigningCredentials = SigningCredentials,
                Expires = DateTime.UtcNow.Add(JwtOptions.LifeTime)
            };
            var Token = JWTHandler.CreateToken(TokenDescriptor);
            var Key = JWTHandler.WriteToken(Token);
            return Key;
        }
    }
}
