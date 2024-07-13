using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PortalArcomix.Data.Entities
{
    [Table("TBL_FORNECEDORNEGOCIACAO")]
    public class Tbl_FornecedorNegociacao
    {
        [Key]
        [Required]
        [StringLength(14, MinimumLength = 14)]
        [RegularExpression(@"\d{14}")]
        [Column("CNPJ")]
        public string CNPJ { get; set; } = string.Empty;


        [Column("DATAVENCIMENTO")]
        public DateTime? DataVencimento { get; set; }

        [Column("PRAZOAPAGAMENTODIAS")]
        public decimal? PrazoPagamentoDias { get; set; }

        [Column("PRAZOENTREGAMEDIADIAS")]
        public decimal? PrazoEntregaMediaDias { get; set; }

        [Column("PRAZOMEDIODIVISITADIAS")]
        public decimal? PrazoMedioDivisitaDias { get; set; }

        [Column("VERBACADASTRO")]
        public decimal? VerbaCadastro { get; set; }

        [MaxLength(30)]
        [Column("MOTIVOVERBAZERADA")]
        public string? MotivoVerbaZerada { get; set; }

        [MaxLength(100)]
        [Column("JUSTIFICATIVASEMVERBA")]
        public string? JustificativaSemVerba { get; set; }

        [MaxLength(20)]
        [Column("DIVISAOVERBA")]
        public string? DivisaoVerba { get; set; }

        [MaxLength(30)]
        [Column("CONTRATOFORNECEDOR")]
        public string? ContratoFornecedor { get; set; }

        [MaxLength(30)]
        [Column("APURACAOCONTRATO")]
        public string? ApuracaoContrato { get; set; }

        [MaxLength(10)]
        [Column("TIPOCONTRATO")]
        public string? TipoContrato { get; set; }

        [MaxLength(30)]
        [Column("MOTIVOSemCONTRATO")]
        public string? MotivoSemContrato { get; set; }

        [MaxLength(100)]
        [Column("JUSTIFICATIVASEMCONTRATO")]
        public string? JustificativaSemContrato { get; set; }

        [Column("TOTALPERCENTUALVAREJO")]
        public decimal? TotalPercentualVarejo { get; set; }

        [Column("TOTALPERCENTUALATACADO")]
        public decimal? TotalPercentualAtacado { get; set; }

        [Column("LOGISTICOVAREJO")]
        public decimal? LogisticoVarejo { get; set; }

        [Column("DEVOLUCAOVAREJO")]
        public decimal? DevolucaoVarejo { get; set; }

        [Column("ANIVERSARIOVAREJO")]
        public decimal? AniversarioVarejo { get; set; }

        [Column("REINAUGURACAOVAREJO")]
        public decimal? ReinauguracaoVarejo { get; set; }

        [Column("CADASTROVAREJO")]
        public decimal? CadastroVarejo { get; set; }

        [Column("FINANCEIROVAREJO")]
        public decimal? FinanceiroVarejo { get; set; }

        [Column("MARKETINGVAREJO")]
        public decimal? MarketingVarejo { get; set; }

        [Column("LOGISTICOATACADO")]
        public decimal? LogisticoAtacado { get; set; }

        [Column("DEVOLUCAOATACADO")]
        public decimal? DevolucaoAtacado { get; set; }

        [Column("ANIVERSARIOATACADO")]
        public decimal? AniversarioAtacado { get; set; }

        [Column("REINAUGURACAOATACADO")]
        public decimal? ReinauguracaoAtacado { get; set; }

        [Column("CADASTROATACADO")]
        public decimal? CadastroAtacado { get; set; }

        [Column("FINANCEIROATACADO")]
        public decimal? FinanceiroAtacado { get; set; }

        [Column("MARKETINGATACADO")]
        public decimal? MarketingAtacado { get; set; }
    }
}

