using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PortalArcomix.Data.Entities
{
    [Table("TBL_FORNECEDORDADOSBANCARIOS")]
    public class Tbl_FornecedorDadosBancarios
    {
        [Key]
        public int ID { get; set; } // Primary Key, not nullable

        public string? CNPJ { get; set; } // CHAR(14 BYTE), nullable

        public string? BANCO { get; set; } // NVARCHAR2(30 CHAR), nullable

        public string? AGENCIA { get; set; } // VARCHAR2(8 BYTE), nullable

        public string? TIPOCONTA { get; set; } // NVARCHAR2(20 CHAR), nullable

        public string? NUMEROCONTA { get; set; } // VARCHAR2(10 BYTE), nullable

        public string? CNPJCONTATITULAR { get; set; } // CHAR(14 BYTE), nullable
    }
}
