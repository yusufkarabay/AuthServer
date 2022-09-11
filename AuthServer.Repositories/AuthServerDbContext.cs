using AuthServer.Core.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace AuthServer.Repositories
{
    public class AuthServerDbContext : IdentityDbContext<UserApp, IdentityRole, string>

    {
        public AuthServerDbContext(DbContextOptions<AuthServerDbContext> dbContextOptions) : base(dbContextOptions)
        {

        }
        

        public DbSet<UserRefreshToken> UserRefreshTokens { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.ApplyConfigurationsFromAssembly(GetType().Assembly);
            base.OnModelCreating(builder);
        }
    }
}

