using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedLibrary.Configurations
{
    public class CustomTokenOptions // appsettingsde ki satıra karşılık gelecek (audience.....)
    {
        public List<string> Audience { get; set; }
        public string Issuer { get; set; }
        public int AccessTokenExpiration { get; set; }
        public int RefreshTokenExpiration { get; set; }
        // Refresh Token
        // Access token’ın expire süresi sona ermeye yaklaştığında veya
        // sona erdiğinde yeni bir access token üretebilmek için kullanılan token değeridir
        public string SecurityKey { get; set; }

    }

}
