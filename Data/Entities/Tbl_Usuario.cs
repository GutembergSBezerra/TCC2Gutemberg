using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PortalArcomix.Data.Entities
{
    [Table("TBL_USUARIO")]
    public class Tbl_Usuario
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("ID_USUARIO")]
        public int ID_Usuario { get; set; }

        [Required]
        [MaxLength(50)]
        [Column("EMAIL")]
        public string Email { get; set; } = string.Empty;

        [Required]
        [MaxLength(20)]
        [Column("SENHA")]
        public string Senha { get; set; } = string.Empty;

        [Required]
        [MaxLength(20)]
        [Column("TIPOUSUARIO")]
        public string TipoUsuario { get; set; } = string.Empty;

        [StringLength(14, MinimumLength = 14)]
        [RegularExpression(@"^\d{14}$", ErrorMessage = "CNPJ INVALIDO")]
        [Column("CNPJ")]
        public string? CNPJ { get; set; }

        [MaxLength(20)]
        [Column("ACCOUNTSTATUS")]
        public string? AccountStatus { get; set; }

        [MaxLength(30)]
        [Column("USUARIO")]
        public string? Usuario { get; set; }
    }
}
