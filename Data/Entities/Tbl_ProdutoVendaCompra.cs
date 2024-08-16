using System.ComponentModel.DataAnnotations.Schema;

namespace PortalArcomix.Data.Entities
{
    [Table("TBL_PRODUTOVENDACOMPRA")]
    public class Tbl_ProdutoVendaCompra
    {
        public int ID { get; set; } // NUMBER, Primary Key

        public int? PRODUTOID { get; set; } // NUMBER, nullable (Foreign Key)
        public string? EAN13 { get; set; } // CHAR(13 BYTE), nullable
        public string? REFERENCIA { get; set; } // NVARCHAR2(30 CHAR), nullable
        public decimal? PESOBRUTOKG { get; set; } // NUMBER(8,2), nullable
        public decimal? PESOLIQUIDOKG { get; set; } // NUMBER(8,2), nullable
        public decimal? ALTURACM { get; set; } // NUMBER, nullable
        public decimal? LARGURACM { get; set; } // NUMBER, nullable
        public decimal? PROFUNDIDADECM { get; set; } // NUMBER, nullable
        public string? DUN14 { get; set; } // CHAR(14 BYTE), nullable
        public string? REFERENCIADUN14 { get; set; } // NVARCHAR2(50 CHAR), nullable
        public decimal? PESOBRUTODUN14KG { get; set; } // NUMBER(8,2), nullable
        public decimal? PESOLIQUIDODUN14KG { get; set; } // NUMBER(10,2), nullable
        public decimal? ALTURADUN14CM { get; set; } // NUMBER, nullable
        public decimal? LARGURADUN14CM { get; set; } // NUMBER, nullable
        public decimal? PROFUNDIDADEDUN14CM { get; set; } // NUMBER, nullable
        public string? EMBALAGEM { get; set; } // NVARCHAR2(20 CHAR), nullable
        public int? QUANTIDADEUNIDADES { get; set; } // NUMBER, nullable
        public int? MESACAIXAS { get; set; } // NUMBER, nullable
        public decimal? ALTURACAIXAS { get; set; } // NUMBER, nullable
        public int? SHELFLIFEDIAS { get; set; } // NUMBER, nullable
    }
}
