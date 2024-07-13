using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PortalArcomix.Data.Entities
{
    [Table("TBL_FORNECEDOR")]
    public class Tbl_Fornecedor
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("ID")]
        public int ID { get; set; }

        [Required]
        [MaxLength(14)]
        [Column("CNPJ")]
        public string CNPJ { get; set; } = string.Empty;

        [MaxLength(150)]
        [Column("RAZAOSOCIAL")]
        public string? RazaoSocial { get; set; }

        [MaxLength(100)]
        [Column("FANTASIA")]
        public string? Fantasia { get; set; }

        [MaxLength(9)]
        [Column("IE")]
        public string? IE { get; set; }

        [MaxLength(7)]
        [Column("CNAE")]
        public string? CNAE { get; set; }

        [MaxLength(8)]
        [Column("CEP")]
        public string? CEP { get; set; }

        [MaxLength(255)]
        [Column("LOGRADOURO")]
        public string? Logradouro { get; set; }

        [MaxLength(6)]
        [Column("NUMEROENDERECO")]
        public string? NumeroEndereco { get; set; }

        [MaxLength(50)]
        [Column("BAIRRO")]
        public string? Bairro { get; set; }

        [MaxLength(100)]
        [Column("COMPLEMENTO")]
        public string? Complemento { get; set; }

        [MaxLength(2)]
        [Column("UF")]
        public string? UF { get; set; }

        [MaxLength(50)]
        [Column("CIDADE")]
        public string? Cidade { get; set; }

        [MaxLength(3)]
        [Column("MICROEMPRESA")]
        public string? MicroEmpresa { get; set; }

        [MaxLength(3)]
        [Column("CONTRIBUITUICMS")]
        public string? ContribuiICMS { get; set; }

        [MaxLength(3)]
        [Column("CONTRIBUITUPI")]
        public string? ContribuiPI { get; set; }

        [MaxLength(14)]
        [Column("TIPOFORNECEDOR")]
        public string? TipoFornecedor { get; set; }

        [MaxLength(30)]
        [Column("FORNECEDORALIMENTOS")]
        public string? FornecedorAlimentos { get; set; }

        [MaxLength(30)]
        [Column("COMPRADORPRINCIPAL")]
        public string? CompradorPrincipal { get; set; }

        [MaxLength(30)]
        [Column("ESTAGIO")]
        public string? Estagio { get; set; }

        [Column("TEMPOESTAGIO")]
        public DateTime? TempoEstagio { get; set; }
    }
}
