using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PortalArcomix.Data.Entities
{
    [Table("TBL_PRODUTOCOMENTARIOS")]
    public class Tbl_ProdutoComentarios
    {
        [Key]
        public int ID { get; set; } // Primary key

        public int PRODUTOID { get; set; } // Foreign key to Tbl_Produto

        public int ID_USUARIO { get; set; } // Foreign key to Tbl_Usuario

        [Required]
        [MaxLength(500)]
        public string COMENTARIO { get; set; } // Comment text

        public DateTime DATACOMENTARIO { get; set; } // Timestamp of the comment

        //// Navigation properties
        //[ForeignKey("PRODUTOID")]
        //public virtual Tbl_Produto Produto { get; set; } // Navigation property to Tbl_Produto

        //[ForeignKey("ID_USUARIO")]
        //public virtual Tbl_Usuario Usuario { get; set; } // Navigation property to Tbl_Usuario
    }
}