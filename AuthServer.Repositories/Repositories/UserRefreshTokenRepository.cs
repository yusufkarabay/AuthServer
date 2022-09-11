using AuthServer.Core.Entities;
using AuthServer.Core.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthServer.Repositories.Repositories
{
    public class UserRefreshTokenRepository : GenericRepository<UserRefreshToken>, IUserRefreshTokenRepoistory
    {
        public UserRefreshTokenRepository(AuthServerDbContext dbContext) : base(dbContext)
        {
        }
    }
}
