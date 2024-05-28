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
            SubEmbalagemData = new SubEmbalagem();
        }

        [BindProperty]
        public EspecificacoesProduto Especificacoes { get; set; }

        [BindProperty]
        public UnidadeVendaCompra UnidadeVendaCompraData { get; set; }

        [BindProperty]
        public SubEmbalagem SubEmbalagemData { get; set; }

        [BindProperty]
        public IFormFile? UploadImagemProduto { get; set; }

        public bool IsSubmissionSuccessful { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id.HasValue)
            {
                var connectionString = _configuration.GetConnectionString("PortalArcomixDB");
                using (var conn = new SqlConnection(connectionString))
                {
                    var query = @"
                        SELECT p.GestorCompras, p.Marca, p.NovaMarca, p.Importado, p.DescricaoProduto, p.NCM, p.CEST, p.ICMS, p.IPI, p.PIS, p.COFINS, p.CustoUnidade, p.CustoCaixa, p.CodXML, p.EmbalagemFat, p.VerbaCadastro, p.MotivoVerbaZerada, p.ImagemProduto, p.CNPJ,
                               pvc.EAN13, pvc.Referencia, pvc.PesoBrutoKg, pvc.PesoLiquidoKg, pvc.AlturaCm, pvc.LarguraCm, pvc.ProfundidadeCm, pvc.DUN14, pvc.ReferenciaDUN14, pvc.PesoBrutoDUN14Kg, pvc.PesoLiquidoDUN14Kg, pvc.AlturaDUN14Cm, pvc.LarguraDUN14Cm, pvc.ProfundidadeDUN14Cm, pvc.Embalagem, pvc.QuantidadeUnidades, pvc.MesaCaixas, pvc.AlturaCaixas, pvc.ShelfLifeDias,
                               pse.EAN13, pse.Referencia, pse.PesoBrutoKg, pse.PesoLiquidoKg, pse.AlturaCm, pse.LarguraCm, pse.ProfundidadeCm, pse.Embalagem, pse.QuantidadeUnidades
                        FROM Tbl_Produto p
                        LEFT JOIN Tbl_ProdutoVendaCompra pvc ON p.ID = pvc.ProdutoID
                        LEFT JOIN Tbl_ProdutoSubEmbalagem pse ON p.ID = pse.ProdutoID
                        WHERE p.ID = @Id";

                    using (var cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@Id", id.Value);
                        await conn.OpenAsync();
                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync())
                            {
                                Especificacoes = new EspecificacoesProduto
                                {
                                    GestorCompras = reader["GestorCompras"].ToString(),
                                    Marca = reader["Marca"].ToString(),
                                    NovaMarca = reader["NovaMarca"].ToString(),
                                    Importado = reader["Importado"] as bool?,
                                    DescricaoProduto = reader["DescricaoProduto"].ToString(),
                                    NCM = reader["NCM"].ToString(),
                                    CEST = reader["CEST"].ToString(),
                                    ICMS = reader["ICMS"] as decimal?,
                                    IPI = reader["IPI"] as decimal?,
                                    PIS = reader["PIS"] as decimal?,
                                    COFINS = reader["COFINS"] as decimal?,
                                    CustoUnidade = reader["CustoUnidade"] as decimal?,
                                    CustoCaixa = reader["CustoCaixa"] as decimal?,
                                    CodXML = reader["CodXML"].ToString(),
                                    EmbalagemFat = reader["EmbalagemFat"].ToString(),
                                    VerbaCadastro = reader["VerbaCadastro"] as decimal?,
                                    MotivoVerbaZerada = reader["MotivoVerbaZerada"].ToString(),
                                    ImagemProduto = reader["ImagemProduto"] as byte[],
                                    CNPJ = reader["CNPJ"].ToString()
                                };

                                UnidadeVendaCompraData = new UnidadeVendaCompra
                                {
                                    EAN13 = reader["EAN13"].ToString(),
                                    Referencia = reader["Referencia"].ToString(),
                                    PesoBrutoKg = reader["PesoBrutoKg"] as decimal?,
                                    PesoLiquidoKg = reader["PesoLiquidoKg"] as decimal?,
                                    AlturaCm = reader["AlturaCm"] as int?,
                                    LarguraCm = reader["LarguraCm"] as int?,
                                    ProfundidadeCm = reader["ProfundidadeCm"] as int?,
                                    DUN14 = reader["DUN14"].ToString(),
                                    ReferenciaDUN14 = reader["ReferenciaDUN14"].ToString(),
                                    PesoBrutoDUN14Kg = reader["PesoBrutoDUN14Kg"] as decimal?,
                                    PesoLiquidoDUN14Kg = reader["PesoLiquidoDUN14Kg"] as decimal?,
                                    AlturaDUN14Cm = reader["AlturaDUN14Cm"] as int?,
                                    LarguraDUN14Cm = reader["LarguraDUN14Cm"] as int?,
                                    ProfundidadeDUN14Cm = reader["ProfundidadeDUN14Cm"] as int?,
                                    Embalagem = reader["Embalagem"].ToString(),
                                    QuantidadeUnidades = reader["QuantidadeUnidades"] as int?,
                                    MesaCaixas = reader["MesaCaixas"] as int?,
                                    AlturaCaixas = reader["AlturaCaixas"] as int?,
                                    ShelfLifeDias = reader["ShelfLifeDias"] as int?
                                };

                                SubEmbalagemData = new SubEmbalagem
                                {
                                    EAN13 = reader["EAN13"].ToString(),
                                    Referencia = reader["Referencia"].ToString(),
                                    PesoBrutoKg = reader["PesoBrutoKg"] as decimal?,
                                    PesoLiquidoKg = reader["PesoLiquidoKg"] as decimal?,
                                    AlturaCm = reader["AlturaCm"] as int?,
                                    LarguraCm = reader["LarguraCm"] as int?,
                                    ProfundidadeCm = reader["ProfundidadeCm"] as int?,
                                    Embalagem = reader["Embalagem"].ToString(),
                                    QuantidadeUnidades = reader["QuantidadeUnidades"] as int?
                                };
                            }
                        }
                    }
                }
            }

            return Page();
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

                            var querySubEmbalagem = @"
                                INSERT INTO Tbl_ProdutoSubEmbalagem
                                (ProdutoID, EAN13, Referencia, PesoBrutoKg, PesoLiquidoKg, AlturaCm, LarguraCm, ProfundidadeCm, Embalagem, QuantidadeUnidades)
                                VALUES
                                (@ProdutoID, @EAN13, @Referencia, @PesoBrutoKg, @PesoLiquidoKg, @AlturaCm, @LarguraCm, @ProfundidadeCm, @Embalagem, @QuantidadeUnidades)";

                            using (var cmdSubEmbalagem = new SqlCommand(querySubEmbalagem, conn, transaction))
                            {
                                cmdSubEmbalagem.Parameters.AddWithValue("@ProdutoID", produtoId);
                                cmdSubEmbalagem.Parameters.AddWithValue("@EAN13", ToDbValue(SubEmbalagemData.EAN13!));
                                cmdSubEmbalagem.Parameters.AddWithValue("@Referencia", ToDbValue(SubEmbalagemData.Referencia!));
                                cmdSubEmbalagem.Parameters.AddWithValue("@PesoBrutoKg", ToDbValue(SubEmbalagemData.PesoBrutoKg!));
                                cmdSubEmbalagem.Parameters.AddWithValue("@PesoLiquidoKg", ToDbValue(SubEmbalagemData.PesoLiquidoKg!));
                                cmdSubEmbalagem.Parameters.AddWithValue("@AlturaCm", ToDbValue(SubEmbalagemData.AlturaCm!));
                                cmdSubEmbalagem.Parameters.AddWithValue("@LarguraCm", ToDbValue(SubEmbalagemData.LarguraCm!));
                                cmdSubEmbalagem.Parameters.AddWithValue("@ProfundidadeCm", ToDbValue(SubEmbalagemData.ProfundidadeCm!));
                                cmdSubEmbalagem.Parameters.AddWithValue("@Embalagem", ToDbValue(SubEmbalagemData.Embalagem!));
                                cmdSubEmbalagem.Parameters.AddWithValue("@QuantidadeUnidades", ToDbValue(SubEmbalagemData.QuantidadeUnidades!));

                                await cmdSubEmbalagem.ExecuteNonQueryAsync();
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

        public class SubEmbalagem
        {
            public string? EAN13 { get; set; }

            public string? Referencia { get; set; }

            public decimal? PesoBrutoKg { get; set; }

            public decimal? PesoLiquidoKg { get; set; }

            public int? AlturaCm { get; set; }

            public int? LarguraCm { get; set; }

            public int? ProfundidadeCm { get; set; }

            public string? Embalagem { get; set; }

            public int? QuantidadeUnidades { get; set; }
        }
    }
}
