using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthServer.Service.Services
{
    public static class SignService // key'imizi imzalama işlemi yapacağız geriye symetrik key döneceğiz
    {
        public static SecurityKey GetSymmetricSecurityKey(string securityKey)
        {
            return new SymmetricSecurityKey(Encoding.UTF8.GetBytes(securityKey));
        }


    };
}
// symetrik => ben bu tokenı oluştururken hangi string ifade ile imzalıyorsam,
// aynı string ile doğrulama işlemi gerçekleştirirsem buna symetrik key denir


// asimetrik key => public key ve private key var. Public ile imzalıyorum, public ile doğruluyorum 
