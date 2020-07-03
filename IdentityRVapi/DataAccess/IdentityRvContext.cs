using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IdentityRVapi.DataAccess
{
    public class IdentityRvContext : DbContext
    {
        public DbSet<RvNetUsers> RvNetUsers { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder options)
     => options.UseSqlite("Data Source=RvIdentity.db");
    }

    public class RvNetUsers
    {
        public Guid Id { get; set; }
        public DateTime CreationDate { get; set; }
        public bool IsActive { get; set; }
        public string UserName { get; set; }
        public string HashedPassword { get; set; }
        public string SaltValue { get; set; }
        public string Scopes { get; set; }

    }
}
