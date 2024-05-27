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
            UnidadeVendaCompraData = new UnidadeVendaCompra();
        }

        [BindProperty]
        public EspecificacoesProduto Especificacoes { get; set; }

        [BindProperty]
        public UnidadeVendaCompra UnidadeVendaCompraData { get; set; }

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
            return value ?? DBNull.Value;
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
                await conn.OpenAsync();
                using (var transaction = conn.BeginTransaction())
                {
                    try
                    {
                        var queryProduto = @"
                            INSERT INTO Tbl_Produto 
                            (GestorCompras, Marca, NovaMarca, Importado, DescricaoProduto, NCM, CEST, ICMS, IPI, PIS, COFINS, CustoUnidade, CustoCaixa, CodXML, EmbalagemFat, VerbaCadastro, MotivoVerbaZerada, ImagemProduto, CNPJ) 
                            OUTPUT INSERTED.ID
                            VALUES 
                            (@GestorCompras, @Marca, @NovaMarca, @Importado, @DescricaoProduto, @NCM, @CEST, @ICMS, @IPI, @PIS, @COFINS, @CustoUnidade, @CustoCaixa, @CodXML, @EmbalagemFat, @VerbaCadastro, @MotivoVerbaZerada, @ImagemProduto, @CNPJ)";

                        using (var cmdProduto = new SqlCommand(queryProduto, conn, transaction))
                        {
                            // Use the helper method for adding parameters
                            cmdProduto.Parameters.AddWithValue("@GestorCompras", ToDbValue(Especificacoes.GestorCompras!));
                            cmdProduto.Parameters.AddWithValue("@Marca", ToDbValue(Especificacoes.Marca!));
                            cmdProduto.Parameters.AddWithValue("@NovaMarca", ToDbValue(Especificacoes.NovaMarca!));
                            cmdProduto.Parameters.AddWithValue("@Importado", ToDbValue(Especificacoes.Importado!));
                            cmdProduto.Parameters.AddWithValue("@DescricaoProduto", ToDbValue(Especificacoes.DescricaoProduto!));
                            cmdProduto.Parameters.AddWithValue("@NCM", ToDbValue(Especificacoes.NCM!));
                            cmdProduto.Parameters.AddWithValue("@CEST", ToDbValue(Especificacoes.CEST!));
                            cmdProduto.Parameters.AddWithValue("@ICMS", ToDbValue(Especificacoes.ICMS!));
                            cmdProduto.Parameters.AddWithValue("@IPI", ToDbValue(Especificacoes.IPI!));
                            cmdProduto.Parameters.AddWithValue("@PIS", ToDbValue(Especificacoes.PIS!));
                            cmdProduto.Parameters.AddWithValue("@COFINS", ToDbValue(Especificacoes.COFINS!));
                            cmdProduto.Parameters.AddWithValue("@CustoUnidade", ToDbValue(Especificacoes.CustoUnidade!));
                            cmdProduto.Parameters.AddWithValue("@CustoCaixa", ToDbValue(Especificacoes.CustoCaixa!));
                            cmdProduto.Parameters.AddWithValue("@CodXML", ToDbValue(Especificacoes.CodXML!));
                            cmdProduto.Parameters.AddWithValue("@EmbalagemFat", ToDbValue(Especificacoes.EmbalagemFat!));
                            cmdProduto.Parameters.AddWithValue("@VerbaCadastro", ToDbValue(Especificacoes.VerbaCadastro!));
                            cmdProduto.Parameters.AddWithValue("@MotivoVerbaZerada", ToDbValue(Especificacoes.MotivoVerbaZerada!));
                            cmdProduto.Parameters.AddWithValue("@ImagemProduto", Especificacoes.ImagemProduto != null ? (object)Especificacoes.ImagemProduto : DBNull.Value).SqlDbType = SqlDbType.VarBinary;
                            cmdProduto.Parameters.AddWithValue("@CNPJ", ToDbValue(Especificacoes.CNPJ!));

                            var produtoId = (int)await cmdProduto.ExecuteScalarAsync();

                            var queryUnidadeVendaCompra = @"
                                INSERT INTO Tbl_ProdutoVendaCompra 
                                (ProdutoID, EAN13, Referencia, PesoBrutoKg, PesoLiquidoKg, AlturaCm, LarguraCm, ProfundidadeCm, DUN14, ReferenciaDUN14, PesoBrutoDUN14Kg, PesoLiquidoDUN14Kg, AlturaDUN14Cm, LarguraDUN14Cm, ProfundidadeDUN14Cm, Embalagem, QuantidadeUnidades, MesaCaixas, AlturaCaixas, ShelfLifeDias) 
                                VALUES 
                                (@ProdutoID, @EAN13, @Referencia, @PesoBrutoKg, @PesoLiquidoKg, @AlturaCm, @LarguraCm, @ProfundidadeCm, @DUN14, @ReferenciaDUN14, @PesoBrutoDUN14Kg, @PesoLiquidoDUN14Kg, @AlturaDUN14Cm, @LarguraDUN14Cm, @ProfundidadeDUN14Cm, @Embalagem, @QuantidadeUnidades, @MesaCaixas, @AlturaCaixas, @ShelfLifeDias)";

                            using (var cmdUnidadeVendaCompra = new SqlCommand(queryUnidadeVendaCompra, conn, transaction))
                            {
                                cmdUnidadeVendaCompra.Parameters.AddWithValue("@ProdutoID", produtoId);
                                cmdUnidadeVendaCompra.Parameters.AddWithValue("@EAN13", ToDbValue(UnidadeVendaCompraData.EAN13!));
                                cmdUnidadeVendaCompra.Parameters.AddWithValue("@Referencia", ToDbValue(UnidadeVendaCompraData.Referencia!));
                                cmdUnidadeVendaCompra.Parameters.AddWithValue("@PesoBrutoKg", ToDbValue(UnidadeVendaCompraData.PesoBrutoKg!));
                                cmdUnidadeVendaCompra.Parameters.AddWithValue("@PesoLiquidoKg", ToDbValue(UnidadeVendaCompraData.PesoLiquidoKg!));
                                cmdUnidadeVendaCompra.Parameters.AddWithValue("@AlturaCm", ToDbValue(UnidadeVendaCompraData.AlturaCm!));
                                cmdUnidadeVendaCompra.Parameters.AddWithValue("@LarguraCm", ToDbValue(UnidadeVendaCompraData.LarguraCm!));
                                cmdUnidadeVendaCompra.Parameters.AddWithValue("@ProfundidadeCm", ToDbValue(UnidadeVendaCompraData.ProfundidadeCm!));
                                cmdUnidadeVendaCompra.Parameters.AddWithValue("@DUN14", ToDbValue(UnidadeVendaCompraData.DUN14!));
                                cmdUnidadeVendaCompra.Parameters.AddWithValue("@ReferenciaDUN14", ToDbValue(UnidadeVendaCompraData.ReferenciaDUN14!));
                                cmdUnidadeVendaCompra.Parameters.AddWithValue("@PesoBrutoDUN14Kg", ToDbValue(UnidadeVendaCompraData.PesoBrutoDUN14Kg!));
                                cmdUnidadeVendaCompra.Parameters.AddWithValue("@PesoLiquidoDUN14Kg", ToDbValue(UnidadeVendaCompraData.PesoLiquidoDUN14Kg!));
                                cmdUnidadeVendaCompra.Parameters.AddWithValue("@AlturaDUN14Cm", ToDbValue(UnidadeVendaCompraData.AlturaDUN14Cm!));
                                cmdUnidadeVendaCompra.Parameters.AddWithValue("@LarguraDUN14Cm", ToDbValue(UnidadeVendaCompraData.LarguraDUN14Cm!));
                                cmdUnidadeVendaCompra.Parameters.AddWithValue("@ProfundidadeDUN14Cm", ToDbValue(UnidadeVendaCompraData.ProfundidadeDUN14Cm!));
                                cmdUnidadeVendaCompra.Parameters.AddWithValue("@Embalagem", ToDbValue(UnidadeVendaCompraData.Embalagem!));
                                cmdUnidadeVendaCompra.Parameters.AddWithValue("@QuantidadeUnidades", ToDbValue(UnidadeVendaCompraData.QuantidadeUnidades!));
                                cmdUnidadeVendaCompra.Parameters.AddWithValue("@MesaCaixas", ToDbValue(UnidadeVendaCompraData.MesaCaixas!));
                                cmdUnidadeVendaCompra.Parameters.AddWithValue("@AlturaCaixas", ToDbValue(UnidadeVendaCompraData.AlturaCaixas!));
                                cmdUnidadeVendaCompra.Parameters.AddWithValue("@ShelfLifeDias", ToDbValue(UnidadeVendaCompraData.ShelfLifeDias!));

                                await cmdUnidadeVendaCompra.ExecuteNonQueryAsync();
                            }

                            transaction.Commit();
                        }
                    }
                    catch
                    {
                        transaction.Rollback();
                        throw;
                    }
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

        public class UnidadeVendaCompra
        {
            public string? EAN13 { get; set; }

            public string? Referencia { get; set; }

            public decimal? PesoBrutoKg { get; set; }

            public decimal? PesoLiquidoKg { get; set; }

            public int? AlturaCm { get; set; }

            public int? LarguraCm { get; set; }

            public int? ProfundidadeCm { get; set; }

            public string? DUN14 { get; set; }

            public string? ReferenciaDUN14 { get; set; }

            public decimal? PesoBrutoDUN14Kg { get; set; }

            public decimal? PesoLiquidoDUN14Kg { get; set; }

            public int? AlturaDUN14Cm { get; set; }

            public int? LarguraDUN14Cm { get; set; }

            public int? ProfundidadeDUN14Cm { get; set; }

            public string? Embalagem { get; set; }

            public int? QuantidadeUnidades { get; set; }

            public int? MesaCaixas { get; set; }

            public int? AlturaCaixas { get; set; }

            public int? ShelfLifeDias { get; set; }
        }
    }
}

