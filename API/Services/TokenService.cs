using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using API.Entities;
using API.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace API.Services
{
    public class TokenService : ITokenService
    {
        private readonly SymmetricSecurityKey _key;
        public TokenService(IConfiguration config)
        {
            _key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["TokenKey"]));

        }

        public string CreateToken(AppUser user)
        {
            
            var claims = new List<Claim>
            {
                //używamy NameId aby przechować user.UserName w tokenie
                new Claim(JwtRegisteredClaimNames.NameId, user.UserName)
            };

            //podajemy klucz a następnie wybieramy algorytm szyfrowania
            var creds = new SigningCredentials(_key, SecurityAlgorithms.HmacSha512Signature);

            //opisujemy nasz przyszły token
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                //przechowuje nasze claimsy
                Subject = new ClaimsIdentity(claims),

                //określamy czas przez jaki token będzie ważny
                Expires = DateTime.Now.AddDays(7),

                //przekazujemy zaszyfrowany klucz 
                SigningCredentials = creds
            };

            //tworzymy tokenHandler
            var tokenHandler = new JwtSecurityTokenHandler();

            //przy pomocy tokenHandlera tworzymy TOKEN
            var token = tokenHandler.CreateToken(tokenDescriptor);

            //zwracamy stworzony token
            return tokenHandler.WriteToken(token);
        }
    }
}