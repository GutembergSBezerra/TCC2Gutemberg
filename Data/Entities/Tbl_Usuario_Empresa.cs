using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PortalArcomix.Data.Entities
{
    [Table("TBL_USUARIO_EMPRESA")]
    public class Tbl_Usuario_Empresa
    {
        [Column("ID_USUARIO", TypeName = "NUMBER")]
        public int ID_Usuario { get; set; }

        [Column("ID_EMPRESA", TypeName = "NUMBER")]
        public int ID_Empresa { get; set; }

        // Propriedades de navegação
        public Tbl_Usuario Usuario { get; set; }
        public Tbl_Empresa Empresa { get; set; }
    }
}
