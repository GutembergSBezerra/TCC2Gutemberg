using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PortalArcomix.Data.Entities
{
    [Table("TBL_FATO_RELEVANTE")]
    public class Tbl_FatoRelevante
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("ID_FATO_RELEVANTE", TypeName = "NUMBER")]
        public int ID_FatoRelevante { get; set; }

        [Required]
        [Column("ID_EMPRESA", TypeName = "NUMBER")]
        public int ID_Empresa { get; set; }

        [Required]
        [MaxLength(255)]
        [Column("NOME_ARQUIVO", TypeName = "NVARCHAR2(255 CHAR)")]
        public string NomeArquivo { get; set; } = string.Empty;

        [Required]
        [MaxLength(500)]
        [Column("CAMINHO_ARQUIVO", TypeName = "NVARCHAR2(500 CHAR)")]
        public string CaminhoArquivo { get; set; } = string.Empty;

        [Required]
        [Column("DATA_UPLOAD", TypeName = "DATE")]
        public DateTime DataUpload { get; set; }

        // Relationship with Tbl_Empresa
        [ForeignKey("ID_Empresa")]
        public Tbl_Empresa Empresa { get; set; }
    }
}
