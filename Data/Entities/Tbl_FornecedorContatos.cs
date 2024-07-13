using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PortalArcomix.Data.Entities
{
    [Table("TBL_FORNECEDORCONTATOS")]
    public class Tbl_FornecedorContatos
    {
        [Key]
        [Required]
        [StringLength(14)]
        [Column("CNPJ")]
        public string CNPJ { get; set; } = string.Empty;

        [MaxLength(30)]
        [Column("CONTATOVENDEDOR")]
        public string? ContatoVendedor { get; set; }

        [MaxLength(2)]
        [Column("DDDVENDEDOR")]
        public string? DDDVendedor { get; set; }

        [MaxLength(9)]
        [Column("TELEFONEVENDEDOR")]
        public string? TelefoneVendedor { get; set; }

        [MaxLength(26)]
        [Column("EMAILVENDEDOR")]
        public string? EmailVendedor { get; set; }

        [MaxLength(30)]
        [Column("CONTATOGERENTE")]
        public string? ContatoGerente { get; set; }

        [MaxLength(2)]
        [Column("DDDGERENTE")]
        public string? DDDGerente { get; set; }

        [MaxLength(9)]
        [Column("TELEFONEGERENTE")]
        public string? TelefoneGerente { get; set; }

        [MaxLength(30)]
        [Column("EMAILGERENTE")]
        public string? EmailGerente { get; set; }

        [MaxLength(30)]
        [Column("RESPONSAVELFINANCEIRO")]
        public string? ResponsavelFinanceiro { get; set; }

        [MaxLength(2)]
        [Column("DDDFRESPFINANCEIRO")]
        public string? DDDFRespFinanceiro { get; set; }

        [MaxLength(9)]
        [Column("TELEFONERESPFINANCEIRO")]
        public string? TelefoneRespFinanceiro { get; set; }

        [MaxLength(30)]
        [Column("EMAILRESPFINANCEIRO")]
        public string? EmailRespFinanceiro { get; set; }

        [MaxLength(2)]
        [Column("DDDFIXOEMPRRESA")]
        public string? DDDFixoEmpresa { get; set; }

        [MaxLength(9)]
        [Column("TELEFONEFIXOEMPRRESA")]
        public string? TelefoneFixoEmpresa { get; set; }
    }
}

