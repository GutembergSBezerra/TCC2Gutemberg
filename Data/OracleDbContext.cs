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
        public DbSet<Tbl_Fornecedor> Tbl_Fornecedor { get; set; }
        public DbSet<Tbl_FornecedorDadosBancarios> Tbl_FornecedorDadosBancarios { get; set; }
        public DbSet<Tbl_FornecedorContatos> Tbl_FornecedorContatos { get; set; }
        public DbSet<Tbl_FornecedorNegociacao> Tbl_FornecedorNegociacao { get; set; }
        public DbSet<Tbl_FornecedorSegurancaAlimentos> Tbl_FornecedorSegurancaAlimentos { get; set; }
        public DbSet<Tbl_FornecedorDocumentos> Tbl_FornecedorDocumentos { get; set; }


        // DbSet for Produto
        public DbSet<Tbl_Produto> Tbl_Produto { get; set; }
        public DbSet<Tbl_ProdutoVendaCompra> Tbl_ProdutoVendaCompra { get; set; }
        public DbSet<Tbl_ProdutoSubEmbalagem> Tbl_ProdutoSubEmbalagem { get; set; } 



        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure the unique constraint on CNPJ for Tbl_FornecedorDadosBancarios
            modelBuilder.Entity<Tbl_FornecedorDadosBancarios>()
                .HasIndex(b => b.CNPJ)
                .IsUnique();

            // Configure the unique constraint on CNPJ for Tbl_FornecedorContatos
            modelBuilder.Entity<Tbl_FornecedorContatos>()
                .HasIndex(c => c.CNPJ)
                .IsUnique();

            // Configure the unique constraint on CNPJ for Tbl_FornecedorNegociacao
            modelBuilder.Entity<Tbl_FornecedorNegociacao>()
                .HasIndex(n => n.CNPJ)
                .IsUnique();

            // Configure the unique constraint on CNPJ for Tbl_FornecedorSegurancaAlimentos
            modelBuilder.Entity<Tbl_FornecedorSegurancaAlimentos>()
                .HasIndex(sa => sa.CNPJ)
                .IsUnique();

            // Map Tbl_FornecedorDocumentos to the database table
            modelBuilder.Entity<Tbl_FornecedorDocumentos>().ToTable("TBL_FORNECEDORDOCUMENTOS");



        }
    }
}
