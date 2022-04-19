using Crud11API.Models;
using Crud11API.ViewModels;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using System.Web.Helpers;

namespace Crud11API.Repository
{
    public class JWTManagerRepository : IJWTManagerRepository
    {
        Dictionary<string, string> UsersRecords = new Dictionary<string, string>
        {
            { "user1","password1"},
            { "user2","password2"},
            { "user3","password3"},
        };
        private readonly IConfiguration iconfiguration;
        public JWTManagerRepository(IConfiguration iconfiguration)
        {
            this.iconfiguration = iconfiguration;
        }
        public Tokens Authenticate(User user)
        {
            /*
            if (!UsersRecords.Any(x => x.Key == users.Name && x.Value == users.Password))
            {
                return null;
            }
            */
            // generate JSON Web Token
            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenKey = Encoding.UTF8.GetBytes(iconfiguration["JWT:Key"]);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.NameIdentifier, user.UserID+""),
                    new Claim(ClaimTypes.Name, user.Name)
                }),
                Expires = DateTime.UtcNow.AddMinutes(10),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(tokenKey), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);



            return new Tokens { Token = tokenHandler.WriteToken(token) };
        }
        public Tokens VerifyToken(String token)
        {
            // generate JSON Web Token
            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenKey = Encoding.UTF8.GetBytes(iconfiguration["JWT:Key"]);
            
            var parameters = new TokenValidationParameters()
            {
                IssuerSigningKey = new SymmetricSecurityKey(tokenKey),
                ValidateLifetime = true,
                ValidateAudience = false,
                ValidateIssuer = false,
                ValidateIssuerSigningKey = true
            };
            try
            {
                Debug.WriteLine("Validacion token");
                var token2 = new JwtSecurityTokenHandler().ReadJwtToken(token);
                var id = token2.Claims.First(c => c.Type == "nameid").Value;
                var name = token2.Claims.First(c => c.Type == "unique_name").Value;
                Debug.WriteLine(id);
                Debug.WriteLine(name);

                return new Tokens { Token = id , RefreshToken = true};
            }
            catch (Microsoft.IdentityModel.Tokens.SecurityTokenExpiredException)
            {

                return new Tokens { Token = "TokenExpired" };
            }
            catch (Exception )
            {

                return new Tokens { Token = "invalid" };
            }

        }
    }
}
