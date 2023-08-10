using AuthServer.Core.Configuration;
using AuthServer.Core.DTOs;
using AuthServer.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthServer.Core.Services
{
    public interface ITokenService
    {
        TokenDto CreateToken(UserApp userApp); // normal token üretecek, kullanıcı için oluşacak
        // kendi içinde kullanacağı için response dönmüyorum

        ClientTokenDto CreateTokenByClient(Client client); // refresh token olmayan client token dönecek
    }
}
