using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PortalArcomix.Data.Entities
{
    [Table("TBL_FORNECEDORCONTATOS")]
    public class Tbl_FornecedorContatos
    {
        [Key]
        public int ID { get; set; } // Primary Key, not nullable

        [MaxLength(14)]
        public string? CNPJ { get; set; } // CHAR(14 BYTE), nullable, unique

        [MaxLength(30)]
        public string? CONTATOVENDEDOR { get; set; } // NVARCHAR2(30 CHAR), nullable

        [MaxLength(2)]
        public string? DDDVENDEDOR { get; set; } // CHAR(2 BYTE), nullable

        [MaxLength(9)]
        public string? TELEFONEVENDEDOR { get; set; } // CHAR(9 BYTE), nullable

        [MaxLength(26)]
        public string? EMAILVENDEDOR { get; set; } // NVARCHAR2(26 CHAR), nullable

        [MaxLength(30)]
        public string? CONTATOGERENTE { get; set; } // NVARCHAR2(30 CHAR), nullable

        [MaxLength(2)]
        public string? DDDGERENTE { get; set; } // CHAR(2 BYTE), nullable

        [MaxLength(9)]
        public string? TELEFONEGERENTE { get; set; } // CHAR(9 BYTE), nullable

        [MaxLength(26)]
        public string? EMAILGERENTE { get; set; } // NVARCHAR2(26 CHAR), nullable

        [MaxLength(30)]
        public string? RESPONSAVELFINANCEIRO { get; set; } // NVARCHAR2(30 CHAR), nullable

        [MaxLength(2)]
        public string? DDDRESPFINANCEIRO { get; set; } // CHAR(2 BYTE), nullable

        [MaxLength(9)]
        public string? TELEFONERESPFINANCEIRO { get; set; } // CHAR(9 BYTE), nullable

        [MaxLength(26)]
        public string? EMAILRESPFINANCEIRO { get; set; } // NVARCHAR2(26 CHAR), nullable

        [MaxLength(2)]
        public string? DDDTELEFONEFIXOEMPRESA { get; set; } // CHAR(2 BYTE), nullable

        [MaxLength(9)]
        public string? TELEFONEFIXOEMPRESA { get; set; } // VARCHAR2(9 BYTE), nullable
    }
}
