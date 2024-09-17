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
        public DbSet<Tbl_Empresa> Tbl_Empresa { get; set; }
        public DbSet<Tbl_Usuario_Empresa> Tbl_Usuario_Empresa { get; set; }
        public DbSet<Tbl_FatoRelevante> Tbl_FatoRelevante { get; set; }  // Add this line to manage TBL_FATO_RELEVANTE





        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configurando a chave composta
            modelBuilder.Entity<Tbl_Usuario_Empresa>()
                .HasKey(ue => new { ue.ID_Usuario, ue.ID_Empresa });  // Configura a chave composta

            // Configurando os relacionamentos
            modelBuilder.Entity<Tbl_Usuario_Empresa>()
                .HasOne(ue => ue.Usuario)
                .WithMany(u => u.UsuarioEmpresas)
                .HasForeignKey(ue => ue.ID_Usuario);

            modelBuilder.Entity<Tbl_Usuario_Empresa>()
                .HasOne(ue => ue.Empresa)
                .WithMany(e => e.EmpresaUsuarios)
                .HasForeignKey(ue => ue.ID_Empresa);

            // Configuring relationship between Tbl_FatoRelevante and Tbl_Empresa
            modelBuilder.Entity<Tbl_FatoRelevante>()
                .HasOne(fr => fr.Empresa)
                .WithMany(e => e.FatosRelevantes)  // Assuming Tbl_Empresa has a collection of FatosRelevantes
                .HasForeignKey(fr => fr.ID_Empresa);
        }

    }
}
