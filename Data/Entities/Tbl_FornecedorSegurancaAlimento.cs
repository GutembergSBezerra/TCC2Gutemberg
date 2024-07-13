using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PortalArcomix.Data.Entities
{
    [Table("TBL_FORNECEDORSEGURANCAALIMENTO")]
    public class Tbl_FornecedorSegurancaAlimento
    {
        [Key]
        [Required]
        [StringLength(14)]
        [Column("CNPJ")]
        public string CNPJ { get; set; } = string.Empty;

        [MaxLength(50)]
        [Column("ATIVIDADEEMPRESA")]
        public string? AtividadeEmpresa { get; set; }

        [MaxLength(50)]
        [Column("ORGAOFISCALIZACAO")]
        public string? OrgaoFiscalizacao { get; set; }

        [MaxLength(50)]
        [Column("NUMEROREGISTROORGAO")]
        public string? NumeroRegistroOrgao { get; set; }

        [MaxLength(30)]
        [Column("RESPONSAVELTECNICO")]
        public string? ResponsavelTecnico { get; set; }

        [MaxLength(2)]
        [Column("DDDTECNICO")]
        public string? DDDTecnico { get; set; }

        [MaxLength(9)]
        [Column("TELEFONETECNICO")]
        public string? TelefoneTecnico { get; set; }

        [MaxLength(30)]
        [Column("EMAILTECNICO")]
        public string? EmailTecnico { get; set; }

        [Column("BPFIMPLANTADOPRATICADO")]
        public bool? BPFImplantadoPraticado { get; set; }

        [Column("SISTEMATRATAMENTORESIDUOS")]
        public bool? SistemaTratamentoResiduos { get; set; }

        [Column("CONTROLESAUDECOLABORADORES")]
        public bool? ControleSaudeColaboradores { get; set; }

        [Column("CIPMIPIMPLEMENTADO")]
        public bool? CIPMIPImplementado { get; set; }

        [Column("SISTEMAIDENTIFICACAORASTREABILIDADE")]
        public bool? SistemaIdentificacaoRastreabilidade { get; set; }

        [Column("PROCEDIMENTOPADRAOFABRICACAO")]
        public bool? ProcedimentoPadraoFabricacao { get; set; }

        [Column("CONTROLEQUALIDADEIMPLEMENTADO")]
        public bool? ControleQualidadeImplementado { get; set; }

        [Column("PROCEDIMENTOPADRAORECEBIMENTO")]
        public bool? ProcedimentoPadraoRecebimento { get; set; }

        [Column("SISTEMATRATAMENTORECLAMACOES")]
        public bool? SistemaTratamentoReclamacoes { get; set; }

        [Column("TRANSPORTEPROPRIO")]
        public bool? TransporteProprio { get; set; }

        [Column("FICHATECNICAPRODUTOS")]
        public bool? FichaTecnicaProdutos { get; set; }

        [Column("CONTROLEPRODUTOSNAOCONFORME")]
        public bool? ControleProdutosNaoConforme { get; set; }

        [Column("EXIGEHIGIENIZACAOCIP")]
        public bool? ExigeHigienizacaoCIP { get; set; }

        [Column("REGISTROSCOMPROVACAOSISTEMAS")]
        public bool? RegistrosComprovacaoSistemas { get; set; }

        [Column("LICENCASPERTINENTES")]
        public bool? LicencasPertinentes { get; set; }

        [Column("ENVIAAMOSTRASCLIENTE")]
        public bool? EnviaAmostrasCliente { get; set; }

        [Column("CONTROLEAGUAABASTECIMENTO")]
        public bool? ControleAguaAbastecimento { get; set; }

        [MaxLength(40)]
        [Column("PARECER")]
        public string? Parecer { get; set; }

        [MaxLength(255)]
        [Column("DESCRICAOAVALIACAO")]
        public string? DescricaoAvaliacao { get; set; }
    }
}