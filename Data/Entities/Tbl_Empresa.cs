using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PortalArcomix.Data.Entities
{
    [Table("TBL_EMPRESA")]
    public class Tbl_Empresa
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("ID_EMPRESA", TypeName = "NUMBER")]
        public int ID_Empresa { get; set; }

        [Required]
        [MaxLength(100)]
        [Column("NOME", TypeName = "NVARCHAR2(100 CHAR)")]
        public string Nome { get; set; } = string.Empty;

        [Required]
        [MaxLength(20)]
        [Column("TICKER", TypeName = "NVARCHAR2(20 CHAR)")]
        public string Ticker { get; set; } = string.Empty;

        [MaxLength(20)]
        [Column("CNPJ", TypeName = "NVARCHAR2(20 CHAR)")]
        public string? CNPJ { get; set; }

        // Relacionamento com usuários - Cada usuário pode acompanhar várias empresas
        public ICollection<Tbl_Usuario> Usuarios { get; set; } = new List<Tbl_Usuario>();
        public ICollection<Tbl_Usuario_Empresa> EmpresaUsuarios { get; set; } = new List<Tbl_Usuario_Empresa>();

    }
}
