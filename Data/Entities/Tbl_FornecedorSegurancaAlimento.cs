using System.ComponentModel.DataAnnotations.Schema;

namespace PortalArcomix.Data.Entities
{
    [Table("TBL_FORNECEDORSEGURANCAALIMENTOS")]
    public class Tbl_FornecedorSegurancaAlimentos
    {
        public int ID { get; set; } // Primary Key
        public int? ID_SEGURANCAALIMENTO { get; set; }
        public string CNPJ { get; set; } = string.Empty; // CHAR(14 BYTE), not nullable, unique
        public string? PARECER { get; set; } // NVARCHAR2(40 CHAR)
        public string? DESCRICAOAVALIACAO { get; set; } // NVARCHAR2(255 CHAR)
        public string? ATIVIDADEEMPRESA { get; set; } // NVARCHAR2(50 CHAR)
        public string? ORGAOFISCALIZACAO { get; set; } // NVARCHAR2(50 CHAR)
        public string? NUMEROREGISTROORGAO { get; set; } // VARCHAR2(50 BYTE)
        public string? RESPONSAVELTECNICO { get; set; } // NVARCHAR2(30 CHAR)
        public string? DDDTECNICO { get; set; } // CHAR(2 BYTE)
        public string? TELEFONETECNICO { get; set; } // CHAR(9 BYTE)
        public string? EMAILTECNICO { get; set; } // NVARCHAR2(30 CHAR)
        public bool BPFIMPLANTADOPRATICADO { get; set; } // NUMBER(1,0)
        public bool SISTEMATRATAMENTORESIDUOS { get; set; } // NUMBER(1,0)
        public bool CONTROLESAUDECOLABORADORES { get; set; } // NUMBER(1,0)
        public bool CIPMIPIMPLEMENTADO { get; set; } // NUMBER(1,0)
        public bool SISTEMAIDENTIFICACAORASTREABILIDADE { get; set; } // NUMBER(1,0)
        public bool PROCEDIMENTOPADRAOFABRICACAO { get; set; } // NUMBER(1,0)
        public bool CONTROLEQUALIDADEIMPLEMENTADO { get; set; } // NUMBER(1,0)
        public bool PROCEDIMENTOPADRAORECEBIMENTO { get; set; } // NUMBER(1,0)
        public bool SISTEMATRATAMENTORECLAMACOES { get; set; } // NUMBER(1,0)
        public bool TRANSPORTEPROPRIO { get; set; } // NUMBER(1,0)
        public bool FICHATECNICAPRODUTOS { get; set; } // NUMBER(1,0)
        public bool CONTROLEPRODUTOSNAOCONFORME { get; set; } // NUMBER(1,0)
        public bool EXIGEHIGIENIZACAOCIP { get; set; } // NUMBER(1,0)
        public bool REGISTROSCOMPROVACAOSISTEMAS { get; set; } // NUMBER(1,0)
        public bool LICENCASPERTINENTES { get; set; } // NUMBER(1,0)
        public bool ENVIAAMOSTRASCLIENTE { get; set; } // NUMBER(1,0)
        public bool CONTROLEAGUAABASTECIMENTO { get; set; } // NUMBER(1,0)
        public string? REGRESPTECNICO { get; set; } // NVARCHAR2(30 CHAR)


    }
}
