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

        [BindProperty]
        public ProdutoGC ProdutoGCDetails { get; set; } = new ProdutoGC();

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
            SELECT DescricaoProduto, CNPJ, GestorCompras, Importado, CodXML, EmbalagemFat, CustoUnidade, CustoCaixa, VerbaCadastro, Marca
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
                            ProdutoDetails.CodXML = reader["CodXML"].ToString();
                            ProdutoDetails.EmbalagemFat = reader["EmbalagemFat"].ToString();
                            ProdutoDetails.CustoUnidade = reader["CustoUnidade"] != DBNull.Value ? (decimal)reader["CustoUnidade"] : (decimal?)null;
                            ProdutoDetails.CustoCaixa = reader["CustoCaixa"] != DBNull.Value ? (decimal)reader["CustoCaixa"] : (decimal?)null;
                            ProdutoDetails.VerbaCadastro = reader["VerbaCadastro"] != DBNull.Value ? (decimal)reader["VerbaCadastro"] : (decimal?)null;
                            ProdutoDetails.Marca = reader["Marca"].ToString();
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
                    }
                    await conn.CloseAsync();
                }

                var queryGC = @"
            SELECT LJ1Max, LJ1Min, LJ2Max, LJ2Min, LJ3Max, LJ3Min, LJ4Max, LJ4Min, LJ5Max, LJ5Min,
                   LJ6Max, LJ6Min, LJ7Max, LJ7Min, LJ8Max, LJ8Min, LJ9Max, LJ9Min, LJ10Max, LJ10Min,
                   LJ11Max, LJ11Min, LJ12Max, LJ12Min, LJ13Max, LJ13Min, LJ14Max, LJ14Min, LJ15Max, LJ15Min,
                   LJ16Max, LJ16Min, LJ17Max, LJ17Min, LJ18Max, LJ18Min, LJ19Max, LJ19Min, LJ20Max, LJ20Min,
                   LJ21Max, LJ21Min, LJ22Max, LJ22Min
            FROM Tbl_ProdutosGC
            WHERE ProdutoID = @ProdutoID";

                using (var cmdGC = new SqlCommand(queryGC, conn))
                {
                    cmdGC.Parameters.AddWithValue("@ProdutoID", id);

                    await conn.OpenAsync();
                    using (var reader = await cmdGC.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            ProdutoGCDetails = new ProdutoGC
                            {
                                ProdutoID = id.Value,
                                LJ1Max = reader["LJ1Max"] as int?,
                                LJ1Min = reader["LJ1Min"] as int?,
                                LJ2Max = reader["LJ2Max"] as int?,
                                LJ2Min = reader["LJ2Min"] as int?,
                                LJ3Max = reader["LJ3Max"] as int?,
                                LJ3Min = reader["LJ3Min"] as int?,
                                LJ4Max = reader["LJ4Max"] as int?,
                                LJ4Min = reader["LJ4Min"] as int?,
                                LJ5Max = reader["LJ5Max"] as int?,
                                LJ5Min = reader["LJ5Min"] as int?,
                                LJ6Max = reader["LJ6Max"] as int?,
                                LJ6Min = reader["LJ6Min"] as int?,
                                LJ7Max = reader["LJ7Max"] as int?,
                                LJ7Min = reader["LJ7Min"] as int?,
                                LJ8Max = reader["LJ8Max"] as int?,
                                LJ8Min = reader["LJ8Min"] as int?,
                                LJ9Max = reader["LJ9Max"] as int?,
                                LJ9Min = reader["LJ9Min"] as int?,
                                LJ10Max = reader["LJ10Max"] as int?,
                                LJ10Min = reader["LJ10Min"] as int?,
                                LJ11Max = reader["LJ11Max"] as int?,
                                LJ11Min = reader["LJ11Min"] as int?,
                                LJ12Max = reader["LJ12Max"] as int?,
                                LJ12Min = reader["LJ12Min"] as int?,
                                LJ13Max = reader["LJ13Max"] as int?,
                                LJ13Min = reader["LJ13Min"] as int?,
                                LJ14Max = reader["LJ14Max"] as int?,
                                LJ14Min = reader["LJ14Min"] as int?,
                                LJ15Max = reader["LJ15Max"] as int?,
                                LJ15Min = reader["LJ15Min"] as int?,
                                LJ16Max = reader["LJ16Max"] as int?,
                                LJ16Min = reader["LJ16Min"] as int?,
                                LJ17Max = reader["LJ17Max"] as int?,
                                LJ17Min = reader["LJ17Min"] as int?,
                                LJ18Max = reader["LJ18Max"] as int?,
                                LJ18Min = reader["LJ18Min"] as int?,
                                LJ19Max = reader["LJ19Max"] as int?,
                                LJ19Min = reader["LJ19Min"] as int?,
                                LJ20Max = reader["LJ20Max"] as int?,
                                LJ20Min = reader["LJ20Min"] as int?,
                                LJ21Max = reader["LJ21Max"] as int?,
                                LJ21Min = reader["LJ21Min"] as int?,
                                LJ22Max = reader["LJ22Max"] as int?,
                                LJ22Min = reader["LJ22Min"] as int?
                            };
                        }
                    }
                    await conn.CloseAsync();
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
                Importado = @Importado,
                CodXML = @CodXML,
                EmbalagemFat = @EmbalagemFat,
                CustoUnidade = @CustoUnidade,
                CustoCaixa = @CustoCaixa,
                VerbaCadastro = @VerbaCadastro,
                Marca = @Marca
            WHERE ID = @Id";

            using (var cmdProduto = new SqlCommand(queryProduto, conn))
            {
                cmdProduto.Parameters.AddWithValue("@Id", ProdutoDetails.ID);
                cmdProduto.Parameters.AddWithValue("@DescricaoProduto", ProdutoDetails.DescricaoProduto);
                cmdProduto.Parameters.AddWithValue("@GestorCompras", ProdutoDetails.GestorCompras);
                cmdProduto.Parameters.AddWithValue("@Importado", ProdutoDetails.Importado);
                cmdProduto.Parameters.AddWithValue("@CodXML", ProdutoDetails.CodXML);
                cmdProduto.Parameters.AddWithValue("@EmbalagemFat", ProdutoDetails.EmbalagemFat);
                cmdProduto.Parameters.AddWithValue("@CustoUnidade", ProdutoDetails.CustoUnidade);
                cmdProduto.Parameters.AddWithValue("@CustoCaixa", ProdutoDetails.CustoCaixa);
                cmdProduto.Parameters.AddWithValue("@VerbaCadastro", ProdutoDetails.VerbaCadastro);
                cmdProduto.Parameters.AddWithValue("@Marca", ProdutoDetails.Marca);

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

                var queryGC = @"
            IF EXISTS (SELECT 1 FROM Tbl_ProdutosGC WHERE ProdutoID = @ProdutoID)
            BEGIN
                UPDATE Tbl_ProdutosGC
                SET LJ1Max = @LJ1Max, LJ1Min = @LJ1Min,
                    LJ2Max = @LJ2Max, LJ2Min = @LJ2Min,
                    LJ3Max = @LJ3Max, LJ3Min = @LJ3Min,
                    LJ4Max = @LJ4Max, LJ4Min = @LJ4Min,
                    LJ5Max = @LJ5Max, LJ5Min = @LJ5Min,
                    LJ6Max = @LJ6Max, LJ6Min = @LJ6Min,
                    LJ7Max = @LJ7Max, LJ7Min = @LJ7Min,
                    LJ8Max = @LJ8Max, LJ8Min = @LJ8Min,
                    LJ9Max = @LJ9Max, LJ9Min = @LJ9Min,
                    LJ10Max = @LJ10Max, LJ10Min = @LJ10Min,
                    LJ11Max = @LJ11Max, LJ11Min = @LJ11Min,
                    LJ12Max = @LJ12Max, LJ12Min = @LJ12Min,
                    LJ13Max = @LJ13Max, LJ13Min = @LJ13Min,
                    LJ14Max = @LJ14Max, LJ14Min = @LJ14Min,
                    LJ15Max = @LJ15Max, LJ15Min = @LJ15Min,
                    LJ16Max = @LJ16Max, LJ16Min = @LJ16Min,
                    LJ17Max = @LJ17Max, LJ17Min = @LJ17Min,
                    LJ18Max = @LJ18Max, LJ18Min = @LJ18Min,
                    LJ19Max = @LJ19Max, LJ19Min = @LJ19Min,
                    LJ20Max = @LJ20Max, LJ20Min = @LJ20Min,
                    LJ21Max = @LJ21Max, LJ21Min = @LJ21Min,
                    LJ22Max = @LJ22Max, LJ22Min = @LJ22Min
                WHERE ProdutoID = @ProdutoID
            END
            ELSE
            BEGIN
                INSERT INTO Tbl_ProdutosGC (ProdutoID, LJ1Max, LJ1Min, LJ2Max, LJ2Min, LJ3Max, LJ3Min, LJ4Max, LJ4Min, LJ5Max, LJ5Min, LJ6Max, LJ6Min, LJ7Max, LJ7Min, LJ8Max, LJ8Min, LJ9Max, LJ9Min, LJ10Max, LJ10Min, LJ11Max, LJ11Min, LJ12Max, LJ12Min, LJ13Max, LJ13Min, LJ14Max, LJ14Min, LJ15Max, LJ15Min, LJ16Max, LJ16Min, LJ17Max, LJ17Min, LJ18Max, LJ18Min, LJ19Max, LJ19Min, LJ20Max, LJ20Min, LJ21Max, LJ21Min, LJ22Max, LJ22Min)
                VALUES (@ProdutoID, @LJ1Max, @LJ1Min, @LJ2Max, @LJ2Min, @LJ3Max, @LJ3Min, @LJ4Max, @LJ4Min, @LJ5Max, @LJ5Min, @LJ6Max, @LJ6Min, @LJ7Max, @LJ7Min, @LJ8Max, @LJ8Min, @LJ9Max, @LJ9Min, @LJ10Max, @LJ10Min, @LJ11Max, @LJ11Min, @LJ12Max, @LJ12Min, @LJ13Max, @LJ13Min, @LJ14Max, @LJ14Min, @LJ15Max, @LJ15Min, @LJ16Max, @LJ16Min, @LJ17Max, @LJ17Min, @LJ18Max, @LJ18Min, @LJ19Max, @LJ19Min, @LJ20Max, @LJ20Min, @LJ21Max, @LJ21Min, @LJ22Max, @LJ22Min)
            END";

                using (var cmdGC = new SqlCommand(queryGC, conn))
                {
                    cmdGC.Parameters.AddWithValue("@ProdutoID", ProdutoDetails.ID);
                    for (int i = 1; i <= 22; i++)
                    {
                        cmdGC.Parameters.AddWithValue($"@LJ{i}Max", Request.Form[$"LJ{i}Max"]);
                        cmdGC.Parameters.AddWithValue($"@LJ{i}Min", Request.Form[$"LJ{i}Min"]);
                    }

                    await conn.OpenAsync();
                    await cmdGC.ExecuteNonQueryAsync();
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
        public string? GestorCompras { get; set; }
        public bool? Importado { get; set; }
        public string? CodXML { get; set; } // Changed to string
        public string? EmbalagemFat { get; set; } // New property
        public decimal? CustoUnidade { get; set; } // New property
        public decimal? CustoCaixa { get; set; } // New property
        public decimal? VerbaCadastro { get; set; } // New property
        public string? Marca { get; set; } // New property
    }


    public class Fornecedor
    {
        public string? RazaoSocial { get; set; }
        public string? TipoFornecedor { get; set; } // New property
    }

    public class ProdutoGC
    {
        public int ProdutoID { get; set; }
        public int? LJ1Max { get; set; }
        public int? LJ1Min { get; set; }
        public int? LJ2Max { get; set; }
        public int? LJ2Min { get; set; }
        public int? LJ3Max { get; set; }
        public int? LJ3Min { get; set; }
        public int? LJ4Max { get; set; }
        public int? LJ4Min { get; set; }
        public int? LJ5Max { get; set; }
        public int? LJ5Min { get; set; }
        public int? LJ6Max { get; set; }
        public int? LJ6Min { get; set; }
        public int? LJ7Max { get; set; }
        public int? LJ7Min { get; set; }
        public int? LJ8Max { get; set; }
        public int? LJ8Min { get; set; }
        public int? LJ9Max { get; set; }
        public int? LJ9Min { get; set; }
        public int? LJ10Max { get; set; }
        public int? LJ10Min { get; set; }
        public int? LJ11Max { get; set; }
        public int? LJ11Min { get; set; }
        public int? LJ12Max { get; set; }
        public int? LJ12Min { get; set; }
        public int? LJ13Max { get; set; }
        public int? LJ13Min { get; set; }
        public int? LJ14Max { get; set; }
        public int? LJ14Min { get; set; }
        public int? LJ15Max { get; set; }
        public int? LJ15Min { get; set; }
        public int? LJ16Max { get; set; }
        public int? LJ16Min { get; set; }
        public int? LJ17Max { get; set; }
        public int? LJ17Min { get; set; }
        public int? LJ18Max { get; set; }
        public int? LJ18Min { get; set; }
        public int? LJ19Max { get; set; }
        public int? LJ19Min { get; set; }
        public int? LJ20Max { get; set; }
        public int? LJ20Min { get; set; }
        public int? LJ21Max { get; set; }
        public int? LJ21Min { get; set; }
        public int? LJ22Max { get; set; }
        public int? LJ22Min { get; set; }
    }

}
}

