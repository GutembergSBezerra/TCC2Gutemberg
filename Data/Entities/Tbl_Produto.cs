using System.ComponentModel.DataAnnotations.Schema;

namespace PortalArcomix.Data.Entities
{
    [Table("TBL_PRODUTO")]
    public class Tbl_Produto
    {
        public int ID { get; set; } // NUMBER, Primary Key

        public string? GESTORCOMPRAS { get; set; } // NVARCHAR2(30 CHAR), nullable
        public string? MARCA { get; set; } // NVARCHAR2(20 CHAR), nullable
        public bool? IMPORTADO { get; set; } // NUMBER(1,0), nullable (assuming 1/0 for true/false)
        public string? DESCRICAOPRODUTO { get; set; } // NVARCHAR2(30 CHAR), nullable
        public string? NCM { get; set; } // CHAR(8 BYTE), nullable
        public string? CEST { get; set; } // CHAR(7 BYTE), nullable
        public decimal? ICMS { get; set; } // NUMBER(5,2), nullable
        public decimal? IPI { get; set; } // NUMBER(5,2), nullable
        public decimal? PIS { get; set; } // NUMBER(5,2), nullable
        public decimal? COFINS { get; set; } // NUMBER(5,2), nullable
        public decimal? CUSTOUNIDADE { get; set; } // NUMBER(8,2), nullable
        public decimal? CUSTOCAIXA { get; set; } // NUMBER(8,2), nullable
        public string? CODXML { get; set; } // NVARCHAR2(44 CHAR), nullable
        public string? EMBALAGEMFAT { get; set; } // NVARCHAR2(50 CHAR), nullable
        public decimal? VERBACADASTRO { get; set; } // NUMBER(9,2), nullable
        public string? MOTIVOVERBAZERADA { get; set; } // NVARCHAR2(255 CHAR), nullable
        public string? CNPJ { get; set; } // CHAR(14 BYTE), nullable
        public string? DESCRICAOCOMPLETA { get; set; } // NVARCHAR2(100 CHAR), nullable

    }
}
