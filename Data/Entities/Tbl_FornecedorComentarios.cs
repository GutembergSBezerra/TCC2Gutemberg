using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PortalArcomix.Data.Entities
{
    [Table("TBL_FORNECEDORCOMENTARIOS")]
    public class Tbl_FornecedorComentarios
    {
        [Key]
        public int ID { get; set; } // Primary key

        [Required]
        [MaxLength(14)]
        public string CNPJ { get; set; } // CNPJ associated with the comment

        public int ID_USUARIO { get; set; } // Foreign key to Tbl_Usuario

        [Required]
        [MaxLength(500)]
        public string COMENTARIO { get; set; } // Comment text

        public DateTime DATACOMENTARIO { get; set; } // Timestamp of the comment
    }
}
