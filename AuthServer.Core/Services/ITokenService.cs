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
    public interface ITokenService // kendi iç yapımda kullanacağım
    {
        // api ile direk haberleşmediğim için direk response dönmüyorum    

        TokenDto CreateToken(UserApp userApp); // sadece normal token üretecek, kullanıcı için oluşacak
        // kendi içinde kullanacağı için response dönmüyorum

        ClientTokenDto CreateTokenByClient(Client client); // refresh token olmayan client token dönecek
    }
}
