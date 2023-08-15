using AuthServer.Core.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthServer.Data
{ 
    //  identity ile ilgili üye tabloları olacak
    // bu tablolar ile beraber ben Userapp ve Product entitylerini de aynı dbde tutmak istiyorum
    public class AppDbContext : IdentityDbContext<UserApp, IdentityRole, string>
    {   // üyelik sistemiyle ilgili tablolar için  IdentityDbContext aldık
        // <Dbde kullanıcı ile ilgili tablo oluşurken Userapp oluşsun, bana rol ver diyor, primary key için tip ver>

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {

        }
        //"Server=DESKTOP-G34LNC7;Database=CoreDemo;Trusted_Connection=true;TrustServerCertificate=True;"
        public DbSet<Product> Products { get; set; }
        public DbSet<UserRefreshToken> UserRefreshTokens { get; set; }

        protected override void OnModelCreating(ModelBuilder builder) // gennel ayarlar mesela entityde required
        {
            builder.ApplyConfigurationsFromAssembly(GetType().Assembly);

            base.OnModelCreating(builder);  
        }

    }
}
