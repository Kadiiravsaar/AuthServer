using AuthServer.Core.Configuration;
using AuthServer.Core.DTOs;
using AuthServer.Core.Models;
using AuthServer.Core.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using SharedLibrary.Configurations;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace AuthServer.Service.Services
{
    public class TokenService : ITokenService
    {
        private readonly UserManager<UserApp> _userManager;
        private readonly CustomTokenOptions _tokenOptions;
        public TokenService(UserManager<UserApp> userManager, IOptions<CustomTokenOptions> options)
        {
            _userManager = userManager;
            _tokenOptions = options.Value;
        }

        private string CreateRefreshToken() // bir tana refresh token üretecek 
        {
            var numberByte = new byte[32];
            using var rnd = RandomNumberGenerator.Create();

            rnd.GetBytes(numberByte);
            return Convert.ToBase64String(numberByte);
        }

        // Claim => Bir token içerisinde token'ın payloadında kullanıcı hakkında tutmuş olduğum nesneler birer claimdir
        // Claim => hangi apilere istek yapacağı gibi ömrü gibi alanlara da claim denir
        private IEnumerable<Claim> GetClaim(UserApp userApp, List<String> audiences)
        {
            var userList = new List<Claim> {
                new Claim(ClaimTypes.NameIdentifier,userApp.Id), // kullanıcı ıd'sini payload da görmek istiyorum
                new Claim(JwtRegisteredClaimNames.Email, userApp.Email), // payload da email görmek istiyorum
                new Claim(ClaimTypes.Name,userApp.UserName), // payload da username görmek istiyorum
                new Claim(JwtRegisteredClaimNames.Jti,Guid.NewGuid().ToString()), // her token için bir de token ıd olsun

            };
            
            userList.AddRange(audiences.Select(x => new Claim(JwtRegisteredClaimNames.Aud, x)));
            return userList;
        }

        public TokenDto CreateToken(UserApp userApp) // kullanıcı ile işlem yapacağım o yüzden usermanager lazım
        {
            throw new NotImplementedException();
        }

        public ClientTokenDto CreateTokenByClient(Client client)
        {
            throw new NotImplementedException();
        }
    }
}
