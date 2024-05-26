using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Data.SqlClient;
using System.Security.Claims;
using System.Threading.Tasks;

namespace PortalArcomix.Pages
{
    public class FornecedorNegociacao
    {
        public string DataBaseVencimento { get; set; }
        public int? PrazoPagamentoDias { get; set; }
        public int? PrazoEntregaMedioDias { get; set; }
        public int? PrazoMedioAtrasoDias { get; set; }
        public int? PrazoMedioVisitaDias { get; set; }
        public decimal? VerbaCadastro { get; set; }
        public string MotivoVerbaZerada { get; set; }
        public string JustificativaSemVerba { get; set; }
        public string DivisaoVerba { get; set; }
        public string ContratoFornecedor { get; set; }
        public string ApuracaoContrato { get; set; }
        public string TipoContrato { get; set; }
        public string MotivoSemContrato { get; set; }
        public string JustificativaSemContrato { get; set; }
        public decimal? TotalPercentualVarejo { get; set; }
        public decimal? TotalPercentualAtacado { get; set; }
        public decimal? LogisticoVarejo { get; set; }
        public decimal? DevolucaoVarejo { get; set; }
        public decimal? AniversarioVarejo { get; set; }
        public decimal? ReinauguracaoVarejo { get; set; }
        public decimal? CadastroVarejo { get; set; }
        public decimal? FinanceiroVarejo { get; set; }
        public decimal? MarketingVarejo { get; set; }
        public decimal? LogisticoAtacado { get; set; }
        public decimal? DevolucaoAtacado { get; set; }
        public decimal? AniversarioAtacado { get; set; }
        public decimal? ReinauguracaoAtacado { get; set; }
        public decimal? CadastroAtacado { get; set; }
        public decimal? FinanceiroAtacado { get; set; }
        public decimal? MarketingAtacado { get; set; }
    }

    public class PrivacyModel : PageModel
    {
        private readonly ILogger<PrivacyModel> _logger;
        private readonly IConfiguration _configuration;

        [BindProperty]
        public FornecedorNegociacao FornecedorNegociacao { get; set; }

        public PrivacyModel(ILogger<PrivacyModel> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
        }

        public async Task<IActionResult> OnGet()
        {
            if (User?.Identity?.IsAuthenticated != true)
            {
                return RedirectToPage("/Login");
            }

            var cnpj = User.FindFirst("CNPJ")?.Value;
            if (string.IsNullOrEmpty(cnpj))
            {
                _logger.LogError("CNPJ is null or empty.");
                return Page();
            }

            FornecedorNegociacao = await GetNegociacaoData(cnpj);
            return Page();
        }

        public async Task<IActionResult> OnPost()
        {
            if (User?.Identity?.IsAuthenticated != true)
            {
                return RedirectToPage("/Login");
            }

            var cnpj = User.FindFirst("CNPJ")?.Value;
            if (string.IsNullOrEmpty(cnpj))
            {
                _logger.LogError("CNPJ is null or empty.");
                return Page();
            }

            if (FornecedorNegociacao != null)
            {
                await SaveNegociacaoData(cnpj, FornecedorNegociacao);
            }

            FornecedorNegociacao = await GetNegociacaoData(cnpj);
            return Page();
        }

        private async Task<FornecedorNegociacao> GetNegociacaoData(string cnpj)
        {
            var connectionString = _configuration.GetConnectionString("PortalArcomixDB");
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                await con.OpenAsync();
                string query = @"
                    SELECT DataBaseVencimento, PrazoPagamentoDias, PrazoEntregaMedioDias, PrazoMedioAtrasoDias, PrazoMedioVisitaDias, VerbaCadastro,
                           MotivoVerbaZerada, JustificativaSemVerba, DivisaoVerba, ContratoFornecedor,
                           ApuracaoContrato, TipoContrato, MotivoSemContrato, JustificativaSemContrato,
                           TotalPercentualVarejo, TotalPercentualAtacado, LogisticoVarejo, DevolucaoVarejo,
                           AniversarioVarejo, ReinauguracaoVarejo, CadastroVarejo, FinanceiroVarejo, MarketingVarejo,
                           LogisticoAtacado, DevolucaoAtacado, AniversarioAtacado, ReinauguracaoAtacado, CadastroAtacado,
                           FinanceiroAtacado, MarketingAtacado
                    FROM Tbl_FornecedorNegociacao
                    WHERE CNPJ = @CNPJ";
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@CNPJ", cnpj);
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            return new FornecedorNegociacao
                            {
                                DataBaseVencimento = reader["DataBaseVencimento"]?.ToString(),
                                PrazoPagamentoDias = reader["PrazoPagamentoDias"] as int?,
                                PrazoEntregaMedioDias = reader["PrazoEntregaMedioDias"] as int?,
                                PrazoMedioAtrasoDias = reader["PrazoMedioAtrasoDias"] as int?,
                                PrazoMedioVisitaDias = reader["PrazoMedioVisitaDias"] as int?,
                                VerbaCadastro = reader["VerbaCadastro"] as decimal?,
                                MotivoVerbaZerada = reader["MotivoVerbaZerada"]?.ToString(),
                                JustificativaSemVerba = reader["JustificativaSemVerba"]?.ToString(),
                                DivisaoVerba = reader["DivisaoVerba"]?.ToString(),
                                ContratoFornecedor = reader["ContratoFornecedor"]?.ToString(),
                                ApuracaoContrato = reader["ApuracaoContrato"]?.ToString(),
                                TipoContrato = reader["TipoContrato"]?.ToString(),
                                MotivoSemContrato = reader["MotivoSemContrato"]?.ToString(),
                                JustificativaSemContrato = reader["JustificativaSemContrato"]?.ToString(),
                                TotalPercentualVarejo = reader["TotalPercentualVarejo"] as decimal?,
                                TotalPercentualAtacado = reader["TotalPercentualAtacado"] as decimal?,
                                LogisticoVarejo = reader["LogisticoVarejo"] as decimal?,
                                DevolucaoVarejo = reader["DevolucaoVarejo"] as decimal?,
                                AniversarioVarejo = reader["AniversarioVarejo"] as decimal?,
                                ReinauguracaoVarejo = reader["ReinauguracaoVarejo"] as decimal?,
                                CadastroVarejo = reader["CadastroVarejo"] as decimal?,
                                FinanceiroVarejo = reader["FinanceiroVarejo"] as decimal?,
                                MarketingVarejo = reader["MarketingVarejo"] as decimal?,
                                LogisticoAtacado = reader["LogisticoAtacado"] as decimal?,
                                DevolucaoAtacado = reader["DevolucaoAtacado"] as decimal?,
                                AniversarioAtacado = reader["AniversarioAtacado"] as decimal?,
                                ReinauguracaoAtacado = reader["ReinauguracaoAtacado"] as decimal?,
                                CadastroAtacado = reader["CadastroAtacado"] as decimal?,
                                FinanceiroAtacado = reader["FinanceiroAtacado"] as decimal?,
                                MarketingAtacado = reader["MarketingAtacado"] as decimal?
                            };
                        }
                    }
                }
            }
            return null;
        }

        private async Task SaveNegociacaoData(string cnpj, FornecedorNegociacao fornecedorNegociacao)
        {
            var connectionString = _configuration.GetConnectionString("PortalArcomixDB");
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                await con.OpenAsync();
                string query = @"
                    UPDATE Tbl_FornecedorNegociacao
                    SET DataBaseVencimento = @DataBaseVencimento,
                        PrazoPagamentoDias = @PrazoPagamentoDias,
                        PrazoEntregaMedioDias = @PrazoEntregaMedioDias,
                        PrazoMedioAtrasoDias = @PrazoMedioAtrasoDias,
                        PrazoMedioVisitaDias = @PrazoMedioVisitaDias,
                        VerbaCadastro = @VerbaCadastro,
                        MotivoVerbaZerada = @MotivoVerbaZerada,
                        JustificativaSemVerba = @JustificativaSemVerba,
                        DivisaoVerba = @DivisaoVerba,
                        ContratoFornecedor = @ContratoFornecedor,
                        ApuracaoContrato = @ApuracaoContrato,
                        TipoContrato = @TipoContrato,
                        MotivoSemContrato = @MotivoSemContrato,
                        JustificativaSemContrato = @JustificativaSemContrato,
                        TotalPercentualVarejo = @TotalPercentualVarejo,
                        TotalPercentualAtacado = @TotalPercentualAtacado,
                        LogisticoVarejo = @LogisticoVarejo,
                        DevolucaoVarejo = @DevolucaoVarejo,
                        AniversarioVarejo = @AniversarioVarejo,
                        ReinauguracaoVarejo = @ReinauguracaoVarejo,
                        CadastroVarejo = @CadastroVarejo,
                        FinanceiroVarejo = @FinanceiroVarejo,
                        MarketingVarejo = @MarketingVarejo,
                        LogisticoAtacado = @LogisticoAtacado,
                        DevolucaoAtacado = @DevolucaoAtacado,
                        AniversarioAtacado = @AniversarioAtacado,
                        ReinauguracaoAtacado = @ReinauguracaoAtacado,
                        CadastroAtacado = @CadastroAtacado,
                        FinanceiroAtacado = @FinanceiroAtacado,
                        MarketingAtacado = @MarketingAtacado
                    WHERE CNPJ = @CNPJ";
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@CNPJ", cnpj);
                    cmd.Parameters.AddWithValue("@DataBaseVencimento", fornecedorNegociacao.DataBaseVencimento ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@PrazoPagamentoDias", fornecedorNegociacao.PrazoPagamentoDias ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@PrazoEntregaMedioDias", fornecedorNegociacao.PrazoEntregaMedioDias ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@PrazoMedioAtrasoDias", fornecedorNegociacao.PrazoMedioAtrasoDias ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@PrazoMedioVisitaDias", fornecedorNegociacao.PrazoMedioVisitaDias ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@VerbaCadastro", fornecedorNegociacao.VerbaCadastro ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@MotivoVerbaZerada", fornecedorNegociacao.MotivoVerbaZerada ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@JustificativaSemVerba", fornecedorNegociacao.JustificativaSemVerba ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@DivisaoVerba", fornecedorNegociacao.DivisaoVerba ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@ContratoFornecedor", fornecedorNegociacao.ContratoFornecedor ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@ApuracaoContrato", fornecedorNegociacao.ApuracaoContrato ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@TipoContrato", fornecedorNegociacao.TipoContrato ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@MotivoSemContrato", fornecedorNegociacao.MotivoSemContrato ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@JustificativaSemContrato", fornecedorNegociacao.JustificativaSemContrato ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@TotalPercentualVarejo", fornecedorNegociacao.TotalPercentualVarejo ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@TotalPercentualAtacado", fornecedorNegociacao.TotalPercentualAtacado ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@LogisticoVarejo", fornecedorNegociacao.LogisticoVarejo ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@DevolucaoVarejo", fornecedorNegociacao.DevolucaoVarejo ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@AniversarioVarejo", fornecedorNegociacao.AniversarioVarejo ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@ReinauguracaoVarejo", fornecedorNegociacao.ReinauguracaoVarejo ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@CadastroVarejo", fornecedorNegociacao.CadastroVarejo ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@FinanceiroVarejo", fornecedorNegociacao.FinanceiroVarejo ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@MarketingVarejo", fornecedorNegociacao.MarketingVarejo ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@LogisticoAtacado", fornecedorNegociacao.LogisticoAtacado ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@DevolucaoAtacado", fornecedorNegociacao.DevolucaoAtacado ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@AniversarioAtacado", fornecedorNegociacao.AniversarioAtacado ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@ReinauguracaoAtacado", fornecedorNegociacao.ReinauguracaoAtacado ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@CadastroAtacado", fornecedorNegociacao.CadastroAtacado ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@FinanceiroAtacado", fornecedorNegociacao.FinanceiroAtacado ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@MarketingAtacado", fornecedorNegociacao.MarketingAtacado ?? (object)DBNull.Value);
                    await cmd.ExecuteNonQueryAsync();
                }
            }
        }
    }
}
