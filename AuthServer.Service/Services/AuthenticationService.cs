using AuthServer.Core.Configuration;
using AuthServer.Core.DTOs;
using AuthServer.Core.Models;
using AuthServer.Core.Repositories;
using AuthServer.Core.Services;
using AuthServer.Core.UniwOfWork;
using AuthServer.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using SharedLibrary.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthServer.Service.Services
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly List<Client> _clients; // cilentleri alacağım
        private readonly ITokenService _tokenService; // token oluşturmak için
        private readonly UserManager<UserApp> _userManager;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IGenericRepository<UserRefreshToken> _userRefreshToken;

        public AuthenticationService(IOptions<List<Client>> optionsClients, ITokenService tokenService, UserManager<UserApp> userManager,
            IUnitOfWork unitOfWork, IGenericRepository<UserRefreshToken> userRefreshToken)
        {
            _clients = optionsClients.Value; // startup da okuyacak
            _tokenService = tokenService;
            _userManager = userManager;
            _unitOfWork = unitOfWork;
            _userRefreshToken = userRefreshToken;
        }

        public async Task<Response<TokenDto>> CreateTokenAsync(LoginDto loginDto)
        {

            if (loginDto == null) throw new ArgumentNullException(nameof(loginDto)); // loginDto null mu kontrol

            var user = await _userManager.FindByEmailAsync(loginDto.Email); // userı bulmaya çalışalım usermanager içindeki metot FindByEmailAsync 

            if (user == null) return Response<TokenDto>.Fail("Password or email Wrong", 400, true); // user yoksa

            if (!await _userManager.CheckPasswordAsync(user, loginDto.Password)) // userın password kontrolu
            {
                return Response<TokenDto>.Fail("Password or email Wrong", 400, true);

            }
            // artık kullanıcı var token üretebiliriz

            var token = _tokenService.CreateToken(user); // token oluştur/üret

            var userRefreshToken = await _userRefreshToken.Where(x => x.UserId == user.Id).SingleOrDefaultAsync(); // refresh token kaydedilecek ama acaba var mı 

            if (userRefreshToken == null) // eğer yoksa 
            {
                await _userRefreshToken.Add(new UserRefreshToken // userrefreshtoken'a yeni bi userrefreshtoken ekle
                {
                    UserId = user.Id,
                    Code = token.RefreshToken, // token üzerinden gelecek refreshtokenı code eşitle
                    Expiration = token.RefreshTokenExpiration
                });
            }
            else // eğer varsa 
            {
                userRefreshToken.Code = token.RefreshToken; // bilgileri güncelle
                userRefreshToken.Expiration = token.RefreshTokenExpiration;
            }

            await _unitOfWork.CommitAsync();
            return Response<TokenDto>.Success(token, 200);

        }

        public Response<ClientTokenDto> CreateTokenByClient(ClientLoginDto clientLoginDto)
        {
            var clients = _clients.SingleOrDefault(x => x.Id == clientLoginDto.ClientId && x.Secret == clientLoginDto.ClientSecret);
            if (clients == null)
            {
                return Response<ClientTokenDto>.Fail("ClientId or ClientSecret Wrong", 404, true);
            }
            var tokenClient = _tokenService.CreateTokenByClient(clients);

            return Response<ClientTokenDto>.Success(tokenClient, 200);
        }

        public async Task<Response<TokenDto>> CreateTokenByRefreshToken(string refreshToken)
        {
            var existRefresToken = await _userRefreshToken.Where(x => x.Code == refreshToken).SingleOrDefaultAsync();
            if (existRefresToken == null)
            {
                return Response<TokenDto>.Fail("Refresh Token Not found", 404, true);

            }

            var user = await _userManager.FindByEmailAsync(existRefresToken.UserId);
            if (user == null)
            {
                return Response<TokenDto>.Fail("UserId Not found", 404, true);

            }

            var tokenDto = _tokenService.CreateToken(user);
            existRefresToken.Code = tokenDto.RefreshToken;
            existRefresToken.Expiration = tokenDto.RefreshTokenExpiration;

            await _unitOfWork.CommitAsync();

            return Response<TokenDto>.Success(tokenDto, 200);

        }

        public async Task<Response<NoDataDto>> RevokeRefreshToken(string refreshToken)
        {
            var existRefreshToken = await _userRefreshToken.Where(x => x.Code == refreshToken).SingleOrDefaultAsync();
            if (existRefreshToken == null)
            {
                return Response<NoDataDto>.Fail("Refresh Token Not found", 404, true);

            }
            _userRefreshToken.Remove(existRefreshToken);

            await _unitOfWork.CommitAsync();

            return Response<NoDataDto>.Success(200);


        }
    }
}
