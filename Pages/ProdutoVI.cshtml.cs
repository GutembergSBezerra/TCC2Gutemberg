using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace PortalArcomix.Pages
{
    public class ProdutoVIModel : PageModel
    {
        private readonly IConfiguration _configuration;

        public ProdutoVIModel(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [BindProperty]
        public Produto ProdutoDetails { get; set; } = new Produto();

        [BindProperty]
        public Fornecedor FornecedorDetails { get; set; } = new Fornecedor();

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var connectionString = _configuration.GetConnectionString("PortalArcomixDB");
            using (var conn = new SqlConnection(connectionString))
            {
                var queryProduto = @"
                    SELECT DescricaoProduto, CNPJ, GestorCompras, Importado
                    FROM Tbl_Produto
                    WHERE ID = @Id";

                using (var cmdProduto = new SqlCommand(queryProduto, conn))
                {
                    cmdProduto.Parameters.AddWithValue("@Id", id);

                    await conn.OpenAsync();
                    using (var reader = await cmdProduto.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            ProdutoDetails.ID = id.Value;
                            ProdutoDetails.DescricaoProduto = reader["DescricaoProduto"].ToString();
                            ProdutoDetails.CNPJ = reader["CNPJ"].ToString();
                            ProdutoDetails.GestorCompras = reader["GestorCompras"].ToString();
                            ProdutoDetails.Importado = reader["Importado"] != DBNull.Value && (bool)reader["Importado"];
                        }
                    }
                    await conn.CloseAsync();
                }

                if (ProdutoDetails.CNPJ != null)
                {
                    var queryFornecedor = @"
                        SELECT RazaoSocial, TipoFornecedor
                        FROM Tbl_Fornecedor
                        WHERE CNPJ = @CNPJ";

                    using (var cmdFornecedor = new SqlCommand(queryFornecedor, conn))
                    {
                        cmdFornecedor.Parameters.AddWithValue("@CNPJ", ProdutoDetails.CNPJ);

                        await conn.OpenAsync();
                        using (var reader = await cmdFornecedor.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync())
                            {
                                FornecedorDetails.RazaoSocial = reader["RazaoSocial"].ToString();
                                FornecedorDetails.TipoFornecedor = reader["TipoFornecedor"].ToString();
                            }
                        }
                        await conn.CloseAsync();
                    }
                }
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var connectionString = _configuration.GetConnectionString("PortalArcomixDB");

            using (var conn = new SqlConnection(connectionString))
            {
                var queryProduto = @"
                    UPDATE Tbl_Produto
                    SET DescricaoProduto = @DescricaoProduto,
                        GestorCompras = @GestorCompras,
                        Importado = @Importado
                    WHERE ID = @Id";

                using (var cmdProduto = new SqlCommand(queryProduto, conn))
                {
                    cmdProduto.Parameters.AddWithValue("@Id", ProdutoDetails.ID);
                    cmdProduto.Parameters.AddWithValue("@DescricaoProduto", ProdutoDetails.DescricaoProduto);
                    cmdProduto.Parameters.AddWithValue("@GestorCompras", ProdutoDetails.GestorCompras);
                    cmdProduto.Parameters.AddWithValue("@Importado", ProdutoDetails.Importado);

                    await conn.OpenAsync();
                    await cmdProduto.ExecuteNonQueryAsync();
                    await conn.CloseAsync();
                }

                if (ProdutoDetails.CNPJ != null)
                {
                    var queryFornecedor = @"
                        UPDATE Tbl_Fornecedor
                        SET RazaoSocial = @RazaoSocial,
                            TipoFornecedor = @TipoFornecedor
                        WHERE CNPJ = @CNPJ";

                    using (var cmdFornecedor = new SqlCommand(queryFornecedor, conn))
                    {
                        cmdFornecedor.Parameters.AddWithValue("@CNPJ", ProdutoDetails.CNPJ);
                        cmdFornecedor.Parameters.AddWithValue("@RazaoSocial", FornecedorDetails.RazaoSocial);
                        cmdFornecedor.Parameters.AddWithValue("@TipoFornecedor", FornecedorDetails.TipoFornecedor);

                        await conn.OpenAsync();
                        await cmdFornecedor.ExecuteNonQueryAsync();
                        await conn.CloseAsync();
                    }
                }
            }

            return RedirectToPage("/SuccessPage"); // Redirect to a success page
        }

        public class Produto
        {
            public int ID { get; set; }
            public string? DescricaoProduto { get; set; }
            public string? CNPJ { get; set; }
            public string? GestorCompras { get; set; } // New property
            public bool? Importado { get; set; } // New property
        }

        public class Fornecedor
        {
            public string? RazaoSocial { get; set; }
            public string? TipoFornecedor { get; set; } // New property
        }
    }
}
