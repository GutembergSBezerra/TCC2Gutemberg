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

        public DbSet<Tbl_Usuario> Tbl_Usuario { get; set; }
        public DbSet<Tbl_Fornecedor> Tbl_Fornecedor { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

        }
    }
}


//public DbSet<Tbl_FornecedorContatos> Tbl_FornecedorContatos { get; set; }
//public DbSet<Tbl_FornecedorDadosBancarios> Tbl_FornecedorDadosBancarios { get; set; }
//public DbSet<Tbl_FornecedorNegociacao> Tbl_FornecedorNegociacao { get; set; }
//public DbSet<Tbl_FornecedorSegurancaAlimento> Tbl_FornecedorSegurancaAlimento { get; set; }