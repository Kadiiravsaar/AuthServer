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
            // scopped => Tek bir istekde bir tane nesne �rne�i olu�tur, ayn� istekde ctorda birden fazla interface kar��la��rsa ayn� nesne �rne�i kullan 
            // addtransient => her interface ile kar��la�t���nda yeni bir nesne �rne�i
            // singelton => uygulama boyunca tek bir nesne �rne�i �zerinden 


            // DI Register
            services.AddScoped<IAuthentService, AuthentService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<ITokenService, TokenService>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
            services.AddScoped(typeof(IServiceGeneric<,>), typeof(ServiceGeneric<,>));  // virg�n koymam�n sebebi i�erisinde 2 tane T al�yor


            services.AddDbContext<AppDbContext>(options =>
            {
                options.UseSqlServer(Configuration.GetConnectionString("SqlCon"), sqloptions =>
                {
                    sqloptions.MigrationsAssembly("AuthServer.Data");// migration burda olaca�� i�in burda olu�sun
                });
            });

            services.AddIdentity<UserApp, IdentityRole>(opt =>
            {
                opt.User.RequireUniqueEmail = true;
                opt.Password.RequireNonAlphanumeric = false; // non alfanum * ? gibi ifadeler bunlar zorunlu olmas�n

            }).AddEntityFrameworkStores<AppDbContext>().AddDefaultTokenProviders(); // �ifre s�f�rlama i�in tokenprovider yapt�k


            // CustomTokenOptions ve appsettingin haberle�mesini sa�layaca��z
            services.Configure<CustomTokenOptions>(Configuration.GetSection("TokenOption"));

            // generic olarak CustomTokenOptions al. bu CustomTokenOptions'� nerden al git appseetings i�inden getsetting ile tokenoption al
            services.Configure<List<Client>>(Configuration.GetSection("Clients"));



            // bi token geldi�inde onu do�rulamak ile ilgili
            services.AddAuthentication(options => // kimlik d��rulama yapaca��z
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                // 2 farkl� �yelik ssitemi i�in schema belirtiyoruz. ancak bende bir tane oldu�u i�in sabit kulland�m

                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                //  AddAuthentication buradaki schema ile AddJwtBearer schema birbiri ile konu�mas� laz�m
                // alttaki schemay� kullanaca��n� bilsin


            }).AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, opts => // cokkie mi jwt mi onun i�in ayar yapaca��z
            {
                // requestin head�r�nda jwt arayacak 

                var tokenOptions = Configuration.GetSection("TokenOption").Get<CustomTokenOptions>();

                opts.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters()
                // validation parametrelerini belirliyoruz
                {
                    ValidIssuer = tokenOptions.Issuer, // token� kim da��tt�
                    ValidAudience = tokenOptions.Audience[0], // hangi apiye istek yapacaksa dizi i�erisindeki ilk eleman
                    IssuerSigningKey = SignService.GetSymmetricSecurityKey(tokenOptions.SecurityKey),

                    ValidateIssuerSigningKey = true, // mutlaka imza olup do�rulanacak
                    ValidateAudience = true, // audience do�rula
                    ValidateIssuer = true,
                    ValidateLifetime = true, // zaman�, �mr�n� yani ge�mi� mi hala devam m� ediyor kontol ettim, do�rulad�m
                    ClockSkew = TimeSpan.Zero  // tokena default olarak 5 dakika daha uzat�r. ikiside ayn� yerden gelecek o y�zden 0 olsun
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
