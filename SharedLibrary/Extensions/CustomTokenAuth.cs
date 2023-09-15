using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SharedLibrary.Configurations;
using SharedLibrary.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedLibrary.Extensions
{
    public static class CustomTokenAuth  // jwt ile ilgili kodlar olacak
    {
        public static void AddCustomTokenAuth(this IServiceCollection services,CustomTokenOptions tokenOptions)
        {
            services.AddAuthentication(options => // kimlik dığrulama yapacağız
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                // 2 farklı üyelik ssitemi için schema belirtiyoruz. ancak bende bir tane olduğu için sabit kullandım

                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                //  AddAuthentication buradaki schema ile AddJwtBearer schema birbiri ile konuşması lazım
                // alttaki schemayı kullanacağını bilsin


            }).AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, opts => // cokkie mi jwt mi onun için ayar yapacağız
            {
                // requestin headırında jwt arayacak 

                //var tokenOptions = Configuration.GetSection("TokenOption").Get<CustomTokenOptions>();

                opts.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters()
                // validation parametrelerini belirliyoruz
                {
                    ValidIssuer = tokenOptions.Issuer, // tokenı kim dağıttı
                    ValidAudience = tokenOptions.Audience[0], // hangi apiye istek yapacaksa dizi içerisindeki ilk eleman
                    IssuerSigningKey = SignService.GetSymmetricSecurityKey(tokenOptions.SecurityKey),

                    ValidateIssuerSigningKey = true, // mutlaka imza olup doğrulanacak
                    ValidateAudience = true, // audience doğrula
                    ValidateIssuer = true,
                    ValidateLifetime = true, // zamanı, ömrünü yani geçmiş mi hala devam mı ediyor kontol ettim, doğruladım
                    ClockSkew = TimeSpan.Zero  // tokena default olarak 5 dakika daha uzatır. ikiside aynı yerden gelecek o yüzden 0 olsun
                };
            });


        }
    }
}
