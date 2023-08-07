using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthServer.Core.DTOs
{
    public class LoginDto // eğer dto görürsek bu dtolar clientlerin göreceği modellerdir
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }
}
