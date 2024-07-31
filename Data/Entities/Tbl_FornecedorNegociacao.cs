using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PortalArcomix.Data.Entities
{
    [Table("TBL_FORNECEDORNEGOCIACAO")]
    public class Tbl_FornecedorNegociacao
    {
        [Key]
        public int ID { get; set; } // Primary Key, not nullable

        [MaxLength(14)]
        public string CNPJ { get; set; } = string.Empty; // CHAR(14 BYTE), not nullable, unique

        [MaxLength(20)]
        public string? DATABASEVENCIMENTO { get; set; } // NVARCHAR2(20 CHAR), nullable

        public int? PRAZOPAGAMENTODIAS { get; set; } // NUMBER(38,0), nullable
        public int? PRAZOENTREGAMEDIODIAS { get; set; } // NUMBER(38,0), nullable
        public int? PRAZOMEDIOATRASODIAS { get; set; } // NUMBER(38,0), nullable
        public int? PRAZOMEDIOVISITADIAS { get; set; } // NUMBER(38,0), nullable

        [Column(TypeName = "NUMBER(9,2)")]
        public decimal? VERBACADASTRO { get; set; } // NUMBER(9,2), nullable

        [MaxLength(30)]
        public string? MOTIVOVERBAZERADA { get; set; } // NVARCHAR2(30 CHAR), nullable

        [MaxLength(100)]
        public string? JUSTIFICATIVASEMVERBA { get; set; } // NVARCHAR2(100 CHAR), nullable

        [MaxLength(20)]
        public string? DIVISAOVERBA { get; set; } // NVARCHAR2(20 CHAR), nullable

        [MaxLength(30)]
        public string? CONTRATOFORNECEDOR { get; set; } // NVARCHAR2(30 CHAR), nullable

        [MaxLength(30)]
        public string? APURACAOCONTRATO { get; set; } // NVARCHAR2(30 CHAR), nullable

        [MaxLength(10)]
        public string? TIPOCONTRATO { get; set; } // NVARCHAR2(10 CHAR), nullable

        [MaxLength(30)]
        public string? MOTIVOSEMCONTRATO { get; set; } // NVARCHAR2(30 CHAR), nullable

        [MaxLength(100)]
        public string? JUSTIFICATIVASEMCONTRATO { get; set; } // NVARCHAR2(100 CHAR), nullable

        [Column(TypeName = "NUMBER(5,2)")]
        public decimal? TOTALPERCENTUALVAREJO { get; set; } // NUMBER(5,2), nullable

        [Column(TypeName = "NUMBER(5,2)")]
        public decimal? TOTALPERCENTUALATACADO { get; set; } // NUMBER(5,2), nullable

        [Column(TypeName = "NUMBER(5,2)")]
        public decimal? LOGISTICOVAREJO { get; set; } // NUMBER(5,2), nullable

        [Column(TypeName = "NUMBER(5,2)")]
        public decimal? DEVOLUCAOVAREJO { get; set; } // NUMBER(5,2), nullable

        [Column(TypeName = "NUMBER(5,2)")]
        public decimal? ANIVERSARIOVAREJO { get; set; } // NUMBER(5,2), nullable

        [Column(TypeName = "NUMBER(5,2)")]
        public decimal? REINAUGURACAOVAREJO { get; set; } // NUMBER(5,2), nullable

        [Column(TypeName = "NUMBER(5,2)")]
        public decimal? CADASTROVAREJO { get; set; } // NUMBER(5,2), nullable

        [Column(TypeName = "NUMBER(5,2)")]
        public decimal? FINANCEIROVAREJO { get; set; } // NUMBER(5,2), nullable

        [Column(TypeName = "NUMBER(5,2)")]
        public decimal? MARKETINGVAREJO { get; set; } // NUMBER(5,2), nullable

        [Column(TypeName = "NUMBER(5,2)")]
        public decimal? LOGISTICOATACADO { get; set; } // NUMBER(5,2), nullable

        [Column(TypeName = "NUMBER(5,2)")]
        public decimal? DEVOLUCAOATACADO { get; set; } // NUMBER(5,2), nullable

        [Column(TypeName = "NUMBER(5,2)")]
        public decimal? ANIVERSARIOATACADO { get; set; } // NUMBER(5,2), nullable

        [Column(TypeName = "NUMBER(5,2)")]
        public decimal? REINAUGURACAOATACADO { get; set; } // NUMBER(5,2), nullable

        [Column(TypeName = "NUMBER(5,2)")]
        public decimal? CADASTROATACADO { get; set; } // NUMBER(5,2), nullable

        [Column(TypeName = "NUMBER(5,2)")]
        public decimal? FINANCEIROATACADO { get; set; } // NUMBER(5,2), nullable

        [Column(TypeName = "NUMBER(5,2)")]
        public decimal? MARKETINGATACADO { get; set; } // NUMBER(5,2), nullable
    }
}
