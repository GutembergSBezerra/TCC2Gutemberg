using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using System.ComponentModel.DataAnnotations;
using System.Data.SqlClient;
using System.Threading.Tasks;
using System.Data;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace PortalArcomix.Pages
{
    public class ProdutoVEModel : PageModel
    {
        private readonly IConfiguration _configuration;

        public ProdutoVEModel(IConfiguration configuration)
        {
            _configuration = configuration;
            Especificacoes = new EspecificacoesProduto();
        }

        [BindProperty]
        public EspecificacoesProduto Especificacoes { get; set; }

        [BindProperty]
        public IFormFile? UploadImagemProduto { get; set; }

        public bool IsSubmissionSuccessful { get; set; }

        public void OnGet()
        {
            // Initialization logic, if any
        }

        // Helper method to handle null values
        private static object ToDbValue(object value)
        {
            if (value == null)
            {
                return DBNull.Value;
            }
            else if (value is byte[] byteArray && byteArray.Length == 0)
            {
                return new byte[0];
            }
            else
            {
                return value;
            }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var connectionString = _configuration.GetConnectionString("PortalArcomixDB");

            if (UploadImagemProduto != null)
            {
                using (var memoryStream = new System.IO.MemoryStream())
                {
                    await UploadImagemProduto.CopyToAsync(memoryStream);
                    Especificacoes.ImagemProduto = memoryStream.ToArray();
                }
            }

            // Retrieve CNPJ from user claims
            var cnpjClaim = User.FindFirst("CNPJ");
            if (cnpjClaim != null)
            {
                Especificacoes.CNPJ = cnpjClaim.Value;
            }
            else
            {
                ModelState.AddModelError(string.Empty, "CNPJ not found in user session.");
                return Page();
            }

            using (var conn = new SqlConnection(connectionString))
            {
                var query = @"
                    INSERT INTO Tbl_Produto 
                    (GestorCompras, Marca, NovaMarca, Importado, DescricaoProduto, NCM, CEST, ICMS, IPI, PIS, COFINS, CustoUnidade, CustoCaixa, CodXML, EmbalagemFat, VerbaCadastro, MotivoVerbaZerada, ImagemProduto, CNPJ) 
                    VALUES 
                    (@GestorCompras, @Marca, @NovaMarca, @Importado, @DescricaoProduto, @NCM, @CEST, @ICMS, @IPI, @PIS, @COFINS, @CustoUnidade, @CustoCaixa, @CodXML, @EmbalagemFat, @VerbaCadastro, @MotivoVerbaZerada, @ImagemProduto, @CNPJ)";

                using (var cmd = new SqlCommand(query, conn))
                {
                    // Use the helper method for adding parameters
                    cmd.Parameters.AddWithValue("@GestorCompras", ToDbValue(Especificacoes.GestorCompras!));
                    cmd.Parameters.AddWithValue("@Marca", ToDbValue(Especificacoes.Marca!));
                    cmd.Parameters.AddWithValue("@NovaMarca", ToDbValue(Especificacoes.NovaMarca!));
                    cmd.Parameters.AddWithValue("@Importado", ToDbValue(Especificacoes.Importado!));
                    cmd.Parameters.AddWithValue("@DescricaoProduto", ToDbValue(Especificacoes.DescricaoProduto!));
                    cmd.Parameters.AddWithValue("@NCM", ToDbValue(Especificacoes.NCM!));
                    cmd.Parameters.AddWithValue("@CEST", ToDbValue(Especificacoes.CEST!));
                    cmd.Parameters.AddWithValue("@ICMS", ToDbValue(Especificacoes.ICMS!));
                    cmd.Parameters.AddWithValue("@IPI", ToDbValue(Especificacoes.IPI!));
                    cmd.Parameters.AddWithValue("@PIS", ToDbValue(Especificacoes.PIS!));
                    cmd.Parameters.AddWithValue("@COFINS", ToDbValue(Especificacoes.COFINS!));
                    cmd.Parameters.AddWithValue("@CustoUnidade", ToDbValue(Especificacoes.CustoUnidade!));
                    cmd.Parameters.AddWithValue("@CustoCaixa", ToDbValue(Especificacoes.CustoCaixa!));
                    cmd.Parameters.AddWithValue("@CodXML", ToDbValue(Especificacoes.CodXML!));
                    cmd.Parameters.AddWithValue("@EmbalagemFat", ToDbValue(Especificacoes.EmbalagemFat!));
                    cmd.Parameters.AddWithValue("@VerbaCadastro", ToDbValue(Especificacoes.VerbaCadastro!));
                    cmd.Parameters.AddWithValue("@MotivoVerbaZerada", ToDbValue(Especificacoes.MotivoVerbaZerada!));
                    cmd.Parameters.AddWithValue("@ImagemProduto", ToDbValue(Especificacoes.ImagemProduto));
                    cmd.Parameters.AddWithValue("@CNPJ", ToDbValue(Especificacoes.CNPJ!));

                    conn.Open();
                    await cmd.ExecuteNonQueryAsync();
                }
            }

            IsSubmissionSuccessful = true;

            return Page();
        }

        public class EspecificacoesProduto
        {
            [StringLength(30)]
            public string? GestorCompras { get; set; }

            [StringLength(20)]
            public string? Marca { get; set; }

            [StringLength(20)]
            public string? NovaMarca { get; set; }

            public bool? Importado { get; set; }

            [StringLength(30)]
            public string? DescricaoProduto { get; set; }

            [StringLength(8)]
            public string? NCM { get; set; }

            [StringLength(7)]
            public string? CEST { get; set; }

            public decimal? ICMS { get; set; }

            public decimal? IPI { get; set; }

            public decimal? PIS { get; set; }

            public decimal? COFINS { get; set; }

            public decimal? CustoUnidade { get; set; }

            public decimal? CustoCaixa { get; set; }

            [StringLength(44)]
            public string? CodXML { get; set; }

            [StringLength(50)]
            public string? EmbalagemFat { get; set; }

            public decimal? VerbaCadastro { get; set; }

            [StringLength(255)]
            public string? MotivoVerbaZerada { get; set; }

            public byte[]? ImagemProduto { get; set; }

            [StringLength(14)]
            public string? CNPJ { get; set; }
        }
    }
}
