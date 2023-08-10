using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthServer.Core.Models
{
    public class UserRefreshToken 
    {
        // Oluşan refresh token'ı bu tabloda tutacağız

        // Ömrünü de tutacağız 

        // Yani Şu usernId'ye sahip Şu refresh token gibi

        public string UserId { get; set; }
        public string Code { get; set; }
        public DateTime Expiration { get; set; }
    }
}
