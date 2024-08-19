using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PortalArcomix.Data.Entities
{
    [Table("TBL_PRODUTOSUBEMBALAGEM")]
    public class Tbl_ProdutoSubEmbalagem
    {
        [Key, ForeignKey("Produto")]
        public int ID { get; set; } // PK and FK

        public int? PRODUTOID { get; set; } // NUMBER, nullable (Foreign Key)
        public string? EAN13 { get; set; } // CHAR(13 BYTE), nullable
        public string? REFERENCIA { get; set; } // NVARCHAR2(30 CHAR), nullable
        public decimal? PESOBRUTOKG { get; set; } // NUMBER(8,2), nullable
        public decimal? PESOLIQUIDOKG { get; set; } // NUMBER(8,2), nullable
        public decimal? ALTURACM { get; set; } // NUMBER, nullable
        public decimal? LARGURACM { get; set; } // NUMBER, nullable
        public decimal? PROFUNDIDADECM { get; set; } // NUMBER, nullable
        public string? EMBALAGEM { get; set; } // NVARCHAR2(20 CHAR), nullable
        public int? QUANTIDADEUNIDADES { get; set; } // NUMBER, nullable
    }
}