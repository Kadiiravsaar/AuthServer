using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthServer.Core.Configuration
{
    public class Client // authServer apiye istek yapacak uygulamalara karşılık gelecek (web de mobil de)
    {
        public string Id { get; set; } 
        public string Secret { get; set; } 


        public List<string> Audiences { get; set; } 
        // kendi iç mekanizmamda  benim apilerimden hangilerine erişecek
        // göndereceğim token da hangi apilere erişeceğim bilgisini tutacağım
    }
}
