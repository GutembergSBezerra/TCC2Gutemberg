using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace PortalArcomix.Data.Entities
{
    [Table("TBL_FORNECEDOR")]
    public class Tbl_Fornecedor
    {
        public int ID { get; set; } // Primary Key
        public string? CNPJ { get; set; } // CHAR(14 BYTE)
        public string? RAZAOSOCIAL { get; set; } // NVARCHAR2(150 CHAR)
        public string? FANTASIA { get; set; } // NVARCHAR2(100 CHAR)
        public string? IE { get; set; } // CHAR(9 BYTE)
        public string? CNAE { get; set; } // CHAR(7 BYTE)
        public string? CEP { get; set; } // CHAR(8 BYTE)
        public string? LOGRADOURO { get; set; } // NVARCHAR2(255 CHAR)
        public string? NUMEROENDERECO { get; set; } // NVARCHAR2(6 CHAR)
        public string? BAIRRO { get; set; } // NVARCHAR2(50 CHAR)
        public string? COMPLEMENTO { get; set; } // NVARCHAR2(100 CHAR)
        public string? UF { get; set; } // CHAR(2 BYTE)
        public string? CIDADE { get; set; } // NVARCHAR2(50 CHAR)
        public string? MICROEMPRESA { get; set; } // CHAR(4 BYTE)
        public string? CONTRIBUIICMS { get; set; } // CHAR(4 BYTE)
        public string? CONTRIBUIIPI { get; set; } // CHAR(4 BYTE)
        public string? TIPOFORNECEDOR { get; set; } // NVARCHAR2(14 CHAR)
        public string? FORNECEDORALIMENTOS { get; set; } // NVARCHAR2(30 CHAR)
        public string? COMPRADORPRINCIPAL { get; set; } // NVARCHAR2(30 CHAR)
        public int? CODIGO { get; set; } // NUMBER
        public string? ESTAGIO { get; set; } // NVARCHAR2(30 CHAR)
        public DateTime? TEMPOESTAGIO { get; set; } // DATE
    }
}