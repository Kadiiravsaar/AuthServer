﻿using AuthServer.Core.DTOs;
using SharedLibrary.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthServer.Core.Services
{
    public interface IAuthentService // kimlik doğrulama işlemi burda gerçekleşecek
    {

        Task<Response<TokenDto>> CreateTokenAsync(LoginDto loginDto);
        // kullanıcı bilgilerinin doğruluğuna göre token oluşturucak
        Task<Response<TokenDto>> CreateTokenByRefreshToken(string refreshToken); // refresh token ile yeniden bir token oluşturma
        Task<Response<NoDataDto>> RevokeRefreshToken(string refreshToken); // refresh token sonlandırabilirim
        // kullanıcı logout yapmak istediğined null'a set ederim
        Response<ClientTokenDto> CreateTokenByClient(ClientLoginDto clientLoginDto);
        // üyelik sistemi olmadan client bi token alsın diye

    }
}
