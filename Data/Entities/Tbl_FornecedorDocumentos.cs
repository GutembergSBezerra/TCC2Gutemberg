namespace PortalArcomix.Data.Entities
{
    public class Tbl_FornecedorDocumentos
    {
        public int ID { get; set; }
        public string CNPJ { get; set; } = string.Empty;
        public string? NOMEARQUIVO { get; set; }
        public string? CAMINHOARQUIVO { get; set; }
        public string? TIPODOCUMENTO { get; set; }
        public DateTimeOffset HORARIOUPLOAD { get; set; }
    }
}
