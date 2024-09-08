using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PortalArcomix.Data.Entities
{
    [Table("TBL_USUARIO")]
    public class Tbl_Usuario
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("ID_USUARIO", TypeName = "NUMBER")]
        public int ID_Usuario { get; set; }

        [Required]
        [MaxLength(50)]
        [Column("EMAIL", TypeName = "NVARCHAR2(50 CHAR)")]
        public string Email { get; set; } = string.Empty;

        [Required]
        [MaxLength(20)]
        [Column("SENHA", TypeName = "NVARCHAR2(20 CHAR)")]
        public string Senha { get; set; } = string.Empty;

        [MaxLength(30)]
        [Column("USUARIO", TypeName = "NVARCHAR2(30 CHAR)")]
        public string? Usuario { get; set; }
        public ICollection<Tbl_Empresa> Empresas { get; set; } = new List<Tbl_Empresa>();
        public ICollection<Tbl_Usuario_Empresa> UsuarioEmpresas { get; set; } = new List<Tbl_Usuario_Empresa>();


    }
}
