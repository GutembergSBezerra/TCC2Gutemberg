using Microsoft.EntityFrameworkCore;
using PortalArcomix.Data.Entities;

namespace PortalArcomix.Data
{
    public class OracleDbContext : DbContext
    {
        public OracleDbContext(DbContextOptions<OracleDbContext> options)
            : base(options)
        {
        }

        // DbSet for Fornecedor
        public DbSet<Tbl_Usuario> Tbl_Usuario { get; set; }
     



        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

    

        }
    }
}
