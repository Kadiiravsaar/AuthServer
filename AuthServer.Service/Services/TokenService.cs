using AuthServer.Core.Configuration;
using AuthServer.Core.DTOs;
using AuthServer.Core.Models;
using AuthServer.Core.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
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
    { // bu servisi core katmanındakş authenticationService kullanacak, apilerin haberi olmayacak
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
        private IEnumerable<Claim> GetClaims(UserApp userApp, List<String> audiences)
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


        private IEnumerable<Claim> GetClaimsByClient(Client client) // clientlar için bir claim
        {
            var claims = new List<Claim>(); // claim listesi oluşturup içine atacağız
            claims.AddRange(client.Audiences.Select(x => new Claim(JwtRegisteredClaimNames.Aud, x))); // her bir str ifade için audience oluştursun 
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()); // her token için bir de token ıd olsun
            new Claim(JwtRegisteredClaimNames.Sub, client.Id.ToString()); // bu token kime ait olduğununu burdan çekiyoruz
            // sub => bu token kimin için
            return claims;
        }


        public TokenDto CreateToken(UserApp userApp) // kullanıcı ile işlem yapacağım o yüzden usermanager lazım
        {
            var accessTokenExpiration = DateTime.Now.AddMinutes(_tokenOptions.AccessTokenExpiration); // token ömrünü alacağız peki nerden geliyor (_tokenOptions buradan)
            var refreshTokenExpiration = DateTime.Now.AddMinutes(_tokenOptions.RefreshTokenExpiration); // refresh token ömrünü alacağız

            var securityKey = SignService.GetSymmetricSecurityKey(_tokenOptions.SecurityKey); //  tokenı imzalayacak keyi alıyoruz

            SigningCredentials signingCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256Signature);
            // imza oluşturuyoruz. token isterken burdan istiyor
             
            JwtSecurityToken jwtSecurityToken = new JwtSecurityToken // token oluşturma
                (
                    issuer: _tokenOptions.Issuer, // bu tokenı yayınlayan kim (authserver)
                    expires: accessTokenExpiration, 
                    notBefore: DateTime.Now, // dakikadan önce geçersiz olmasın  ????? 
                    claims: GetClaims(userApp, _tokenOptions.Audience),
                    signingCredentials: signingCredentials
                );


            
            var handler = new JwtSecurityTokenHandler(); // bu arkadaş bir token olşturcak
            
            var token = handler.WriteToken(jwtSecurityToken); // bana bi token ver ve ben tokun oluşturayım
            // peki nasıl => yukarıda (  JwtSecurityToken jwtSecurityToken  ile başlayan ) yerdeki bilgilere göre bana token üretip string token üretiyor 

            var tokenDto = new TokenDto
            {
                AccessToken = token, // tokenın kendisi
                RefreshToken = CreateRefreshToken(),
                AccessTokenExpiration = accessTokenExpiration,
                RefreshTokenExpiration = refreshTokenExpiration

            };

            return tokenDto;


        }

        public ClientTokenDto CreateTokenByClient(Client client)
        {
            var accessTokenExpiration = DateTime.Now.AddMinutes(_tokenOptions.AccessTokenExpiration); // token ömrünü alacağız peki nerden geliyor (_tokenOptions buradan)

            // clientler de refresh token olmayacak

            var securityKey = SignService.GetSymmetricSecurityKey(_tokenOptions.SecurityKey); //  tokenı imzalayacak keyi alıyoruz

            SigningCredentials signingCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256Signature);
            // imza oluşturuyoruz. token isterken burdan istiyor

            JwtSecurityToken jwtSecurityToken = new JwtSecurityToken // token oluşturma
                (
                    issuer: _tokenOptions.Issuer, // bu tokenı yayınlayan kim (authserver)
                    expires: accessTokenExpiration,
                    notBefore: DateTime.Now, // dakikadan önce geçersiz olmasın  ????? 
                    claims: GetClaimsByClient(client),
                    signingCredentials: signingCredentials
                );



            var handler = new JwtSecurityTokenHandler(); // bu arkadaş bir token olşturcak

            var token = handler.WriteToken(jwtSecurityToken); // bana bi token ver ve ben tokun oluşturayım
            // peki nasıl => yukarıda (  JwtSecurityToken jwtSecurityToken  ile başlayan ) yerdeki bilgilere göre bana token üretip string token üretiyor 

            var tokenDto = new ClientTokenDto
            {
                AccessToken = token, // tokenın kendisi
                AccessTokenExpiration = accessTokenExpiration,

            };

            return tokenDto;
        }
    }
}
