using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthServer.Core.DTOs
{
    public class TokenDto // esas token bu abi dönülecek olan
    {

        public string AccessToken { get; set; } 
        public DateTime AccessTokenExpiration { get; set; } // accesstoken'ın ömrünü belirliyorum
        public string RefreshToken { get; set; } 
        public string RefreshTokenExpiration { get; set; } // RefresToken'ın ömrünü belirliyorum

    }
}
