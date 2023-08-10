using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthServer.Core.DTOs
{
    public class ClientTokenDto
    {
        // clientlere döneceğimiz token modeli  (ClientTokenDto,TokenDto)

        public string AccessToken { get; set; }
        public DateTime AccessTokenExpiration { get; set; } // accesstoken'ın ömrünü belirliyorum

    }
}
