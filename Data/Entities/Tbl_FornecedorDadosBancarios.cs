using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PortalArcomix.Data.Entities
{
    [Table("TBL_FORNECEDORDADOSBANCARIOS")]
    public class Tbl_FornecedorDadosBancarios
    {
        [Key]
        [Required]
        [StringLength(14)]
        [Column("CNPJ")]
        public string CNPJ { get; set; } = string.Empty;

        [MaxLength(5)]
        [Column("CODIGOBANCO")]
        public string? CodigoBanco { get; set; }

        [MaxLength(5)]
        [Column("CODIGOAGENCIA")]
        public string? CodigoAgencia { get; set; }

        [MaxLength(10)]
        [Column("NUMEROCONTA")]
        public string? NumeroConta { get; set; }

        [MaxLength(1)]
        [Column("DIGITOCONTA")]
        public string? DigitoConta { get; set; }

        [MaxLength(30)]
        [Column("NOMEFAVORECIDO")]
        public string? NomeFavorecido { get; set; }
    }
}