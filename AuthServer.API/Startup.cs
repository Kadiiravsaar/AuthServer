using AuthServer.Core.Configuration;
using AuthServer.Core.Models;
using AuthServer.Core.Repositories;
using AuthServer.Core.Services;
using AuthServer.Core.UniwOfWork;
using AuthServer.Data;
using AuthServer.Data.Repositories;
using AuthServer.Service.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using SharedLibrary.Configurations;
using SharedLibrary.Services;
using System;
using System.Collections.Generic;

namespace AuthServer.API
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // scopped => Tek bir istekde bir tane nesne örneði oluþtur, ayný istekde ctorda birden fazla interface karþýlaþýrsa ayný nesne örneði kullan 
            // addtransient => her interface ile karþýlaþtýðýnda yeni bir nesne örneði
            // singelton => uygulama boyunca tek bir nesne örneði üzerinden 


            // DI Register
            services.AddScoped<IAuthentService, AuthentService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<ITokenService, TokenService>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
            services.AddScoped(typeof(IServiceGeneric<,>), typeof(ServiceGeneric<,>));  // virgün koymamýn sebebi içerisinde 2 tane T alýyor


            services.AddDbContext<AppDbContext>(options =>
            {
                options.UseSqlServer(Configuration.GetConnectionString("SqlCon"), sqloptions =>
                {
                    sqloptions.MigrationsAssembly("AuthServer.Data");// migration burda olacaðý için burda oluþsun
                });
            });

            services.AddIdentity<UserApp, IdentityRole>(opt =>
            {
                opt.User.RequireUniqueEmail = true;
                opt.Password.RequireNonAlphanumeric = false; // non alfanum * ? gibi ifadeler bunlar zorunlu olmasýn

            }).AddEntityFrameworkStores<AppDbContext>().AddDefaultTokenProviders(); // þifre sýfýrlama için tokenprovider yaptýk


            // CustomTokenOptions ve appsettingin haberleþmesini saðlayacaðýz
            services.Configure<CustomTokenOptions>(Configuration.GetSection("TokenOption"));

            // generic olarak CustomTokenOptions al. bu CustomTokenOptions'ý nerden al git appseetings içinden getsetting ile tokenoption al
            services.Configure<List<Client>>(Configuration.GetSection("Clients"));



            // bi token geldiðinde onu doðrulamak ile ilgili
            services.AddAuthentication(options => // kimlik dýðrulama yapacaðýz
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                // 2 farklý üyelik ssitemi için schema belirtiyoruz. ancak bende bir tane olduðu için sabit kullandým

                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                //  AddAuthentication buradaki schema ile AddJwtBearer schema birbiri ile konuþmasý lazým
                // alttaki schemayý kullanacaðýný bilsin


            }).AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, opts => // cokkie mi jwt mi onun için ayar yapacaðýz
            {
                // requestin headýrýnda jwt arayacak 

                var tokenOptions = Configuration.GetSection("TokenOption").Get<CustomTokenOptions>();

                opts.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters()
                // validation parametrelerini belirliyoruz
                {
                    ValidIssuer = tokenOptions.Issuer, // tokený kim daðýttý
                    ValidAudience = tokenOptions.Audience[0], // hangi apiye istek yapacaksa dizi içerisindeki ilk eleman
                    IssuerSigningKey = SignService.GetSymmetricSecurityKey(tokenOptions.SecurityKey),

                    ValidateIssuerSigningKey = true, // mutlaka imza olup doðrulanacak
                    ValidateAudience = true, // audience doðrula
                    ValidateIssuer = true,
                    ValidateLifetime = true, // zamaný, ömrünü yani geçmiþ mi hala devam mý ediyor kontol ettim, doðruladým
                    ClockSkew = TimeSpan.Zero  // tokena default olarak 5 dakika daha uzatýr. ikiside ayný yerden gelecek o yüzden 0 olsun
                };
            });



            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "AuthServer.API", Version = "v1" });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "AuthServer.API v1"));
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthentication();
            
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
