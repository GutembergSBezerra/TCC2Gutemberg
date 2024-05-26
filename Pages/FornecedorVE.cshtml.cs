using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using System.Data.SqlClient;
using System.Security.Claims;
using System.Threading.Tasks;

namespace PortalArcomix.Pages
{
    public class FornecedorVEModel : PageModel
    {
        private readonly IConfiguration _configuration;

        public FornecedorVEModel(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [BindProperty]
        public FornecedorData Fornecedor { get; set; }
        [BindProperty]
        public DadosBancariosData DadosBancarios { get; set; }
        [BindProperty]
        public ContatosData Contatos { get; set; }
        [BindProperty]
        public FornecedorNegociacao FornecedorNegociacao { get; set; } // Add this line

        public bool IsSubmissionSuccessful { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            var cnpj = User.FindFirst("CNPJ")?.Value;
            if (string.IsNullOrEmpty(cnpj))
            {
                return RedirectToPage("/Login");
            }

            Fornecedor = await GetFornecedorData(cnpj);
            DadosBancarios = await GetDadosBancariosData(cnpj);
            Contatos = await GetContatosData(cnpj);
            FornecedorNegociacao = await GetFornecedorNegociacaoData(cnpj); // Add this line

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var cnpj = User.FindFirst("CNPJ")?.Value;
            if (string.IsNullOrEmpty(cnpj))
            {
                return RedirectToPage("/Login");
            }

            Fornecedor.CNPJ = cnpj; // Ensure the CNPJ remains the same
            DadosBancarios.CNPJ = cnpj;
            Contatos.CNPJ = cnpj;
            FornecedorNegociacao.CNPJ = cnpj; // Add this line

            await UpdateFornecedorData(Fornecedor);
            await UpdateDadosBancariosData(DadosBancarios);
            await UpdateContatosData(Contatos);
            await UpdateFornecedorNegociacaoData(FornecedorNegociacao); // Add this line

            // Refresh the data
            Fornecedor = await GetFornecedorData(cnpj);
            DadosBancarios = await GetDadosBancariosData(cnpj);
            Contatos = await GetContatosData(cnpj);
            FornecedorNegociacao = await GetFornecedorNegociacaoData(cnpj); // Add this line

            IsSubmissionSuccessful = true; // Set the flag to true after successful update

            return Page();
        }

        private async Task<FornecedorData> GetFornecedorData(string cnpj)
        {
            var connectionString = _configuration.GetConnectionString("PortalArcomixDB");
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                await con.OpenAsync();
                string query = @"
                    SELECT CNPJ, RazaoSocial, Fantasia, IE, CNAE, CEP, Logradouro, NumeroEndereco, Bairro, Complemento, UF, Cidade, 
                           MicroEmpresa, ContribuiICMS, ContribuiIPI, TipoFornecedor, FornecedorAlimentos, CompradorPrincipal
                    FROM Tbl_Fornecedor
                    WHERE CNPJ = @CNPJ";
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@CNPJ", cnpj);
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            return new FornecedorData
                            {
                                CNPJ = reader["CNPJ"].ToString(),
                                RazaoSocial = reader["RazaoSocial"].ToString(),
                                Fantasia = reader["Fantasia"].ToString(),
                                IE = reader["IE"].ToString(),
                                CNAE = reader["CNAE"].ToString(),
                                CEP = reader["CEP"].ToString(),
                                Logradouro = reader["Logradouro"].ToString(),
                                NumeroEndereco = reader["NumeroEndereco"].ToString(),
                                Bairro = reader["Bairro"].ToString(),
                                Complemento = reader["Complemento"].ToString(),
                                UF = reader["UF"].ToString(),
                                Cidade = reader["Cidade"].ToString(),
                                MicroEmpresa = reader["MicroEmpresa"].ToString(),
                                ContribuiICMS = reader["ContribuiICMS"].ToString(),
                                ContribuiIPI = reader["ContribuiIPI"].ToString(),
                                TipoFornecedor = reader["TipoFornecedor"].ToString(),
                                FornecedorAlimentos = reader["FornecedorAlimentos"].ToString(),
                                CompradorPrincipal = reader["CompradorPrincipal"].ToString()
                            };
                        }
                    }
                }
            }
            return null;
        }

        private async Task<DadosBancariosData> GetDadosBancariosData(string cnpj)
        {
            var connectionString = _configuration.GetConnectionString("PortalArcomixDB");
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                await con.OpenAsync();
                string query = @"
                    SELECT Banco, Agencia, TipoConta, NumeroConta, CNPJContaTitular
                    FROM Tbl_DadosBancariosFornecedor
                    WHERE CNPJ = @CNPJ";
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@CNPJ", cnpj);
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            return new DadosBancariosData
                            {
                                Banco = reader["Banco"].ToString(),
                                Agencia = reader["Agencia"].ToString(),
                                TipoConta = reader["TipoConta"].ToString(),
                                NumeroConta = reader["NumeroConta"].ToString(),
                                CNPJContaTitular = reader["CNPJContaTitular"].ToString()
                            };
                        }
                    }
                }
            }
            return null;
        }

        private async Task<ContatosData> GetContatosData(string cnpj)
        {
            var connectionString = _configuration.GetConnectionString("PortalArcomixDB");
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                await con.OpenAsync();
                string query = @"
                    SELECT ContatoVendedor, DDDVendedor, TelefoneVendedor, EmailVendedor, 
                           ContatoGerente, DDDGerente, TelefoneGerente, EmailGerente, 
                           ResponsavelFinanceiro, DDDRespFinanceiro, TelefoneRespFinanceiro, EmailRespFinanceiro, 
                           DDDTelefoneFixoEmpresa, TelefoneFixoEmpresa
                    FROM Tbl_ContatosFornecedor
                    WHERE CNPJ = @CNPJ";
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@CNPJ", cnpj);
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            return new ContatosData
                            {
                                ContatoVendedor = reader["ContatoVendedor"].ToString(),
                                DDDVendedor = reader["DDDVendedor"].ToString(),
                                TelefoneVendedor = reader["TelefoneVendedor"].ToString(),
                                EmailVendedor = reader["EmailVendedor"].ToString(),
                                ContatoGerente = reader["ContatoGerente"].ToString(),
                                DDDGerente = reader["DDDGerente"].ToString(),
                                TelefoneGerente = reader["TelefoneGerente"].ToString(),
                                EmailGerente = reader["EmailGerente"].ToString(),
                                ResponsavelFinanceiro = reader["ResponsavelFinanceiro"].ToString(),
                                DDDRespFinanceiro = reader["DDDRespFinanceiro"].ToString(),
                                TelefoneRespFinanceiro = reader["TelefoneRespFinanceiro"].ToString(),
                                EmailRespFinanceiro = reader["EmailRespFinanceiro"].ToString(),
                                DDDTelefoneFixoEmpresa = reader["DDDTelefoneFixoEmpresa"].ToString(),
                                TelefoneFixoEmpresa = reader["TelefoneFixoEmpresa"].ToString()
                            };
                        }
                    }
                }
            }
            return null;
        }

        private async Task<FornecedorNegociacao> GetFornecedorNegociacaoData(string cnpj)
        {
            var connectionString = _configuration.GetConnectionString("PortalArcomixDB");
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                await con.OpenAsync();
                string query = @"
                    SELECT DataBaseVencimento, PrazoPagamentoDias, PrazoEntregaMedioDias, PrazoMedioAtrasoDias, PrazoMedioVisitaDias, VerbaCadastro,
                           MotivoVerbaZerada, JustificativaSemVerba, DivisaoVerba, ContratoFornecedor, ApuracaoContrato, TipoContrato, MotivoSemContrato,
                           JustificativaSemContrato, TotalPercentualVarejo, TotalPercentualAtacado, LogisticoVarejo, DevolucaoVarejo, AniversarioVarejo,
                           ReinauguracaoVarejo, CadastroVarejo, FinanceiroVarejo, MarketingVarejo, LogisticoAtacado, DevolucaoAtacado, AniversarioAtacado,
                           ReinauguracaoAtacado, CadastroAtacado, FinanceiroAtacado, MarketingAtacado
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
                                DataBaseVencimento = reader["DataBaseVencimento"].ToString(),
                                PrazoPagamentoDias = reader["PrazoPagamentoDias"] != DBNull.Value ? Convert.ToInt32(reader["PrazoPagamentoDias"]) : 0,
                                PrazoEntregaMedioDias = reader["PrazoEntregaMedioDias"] != DBNull.Value ? Convert.ToInt32(reader["PrazoEntregaMedioDias"]) : 0,
                                PrazoMedioAtrasoDias = reader["PrazoMedioAtrasoDias"] != DBNull.Value ? Convert.ToInt32(reader["PrazoMedioAtrasoDias"]) : 0,
                                PrazoMedioVisitaDias = reader["PrazoMedioVisitaDias"] != DBNull.Value ? Convert.ToInt32(reader["PrazoMedioVisitaDias"]) : 0,
                                VerbaCadastro = reader["VerbaCadastro"] != DBNull.Value ? Convert.ToDecimal(reader["VerbaCadastro"]) : 0,
                                MotivoVerbaZerada = reader["MotivoVerbaZerada"].ToString(),
                                JustificativaSemVerba = reader["JustificativaSemVerba"].ToString(),
                                DivisaoVerba = reader["DivisaoVerba"].ToString(),
                                ContratoFornecedor = reader["ContratoFornecedor"].ToString(),
                                ApuracaoContrato = reader["ApuracaoContrato"].ToString(),
                                TipoContrato = reader["TipoContrato"].ToString(),
                                MotivoSemContrato = reader["MotivoSemContrato"].ToString(),
                                JustificativaSemContrato = reader["JustificativaSemContrato"].ToString(),
                                TotalPercentualVarejo = reader["TotalPercentualVarejo"] != DBNull.Value ? Convert.ToDecimal(reader["TotalPercentualVarejo"]) : 0,
                                TotalPercentualAtacado = reader["TotalPercentualAtacado"] != DBNull.Value ? Convert.ToDecimal(reader["TotalPercentualAtacado"]) : 0,
                                LogisticoVarejo = reader["LogisticoVarejo"] != DBNull.Value ? Convert.ToDecimal(reader["LogisticoVarejo"]) : 0,
                                DevolucaoVarejo = reader["DevolucaoVarejo"] != DBNull.Value ? Convert.ToDecimal(reader["DevolucaoVarejo"]) : 0,
                                AniversarioVarejo = reader["AniversarioVarejo"] != DBNull.Value ? Convert.ToDecimal(reader["AniversarioVarejo"]) : 0,
                                ReinauguracaoVarejo = reader["ReinauguracaoVarejo"] != DBNull.Value ? Convert.ToDecimal(reader["ReinauguracaoVarejo"]) : 0,
                                CadastroVarejo = reader["CadastroVarejo"] != DBNull.Value ? Convert.ToDecimal(reader["CadastroVarejo"]) : 0,
                                FinanceiroVarejo = reader["FinanceiroVarejo"] != DBNull.Value ? Convert.ToDecimal(reader["FinanceiroVarejo"]) : 0,
                                MarketingVarejo = reader["MarketingVarejo"] != DBNull.Value ? Convert.ToDecimal(reader["MarketingVarejo"]) : 0,
                                LogisticoAtacado = reader["LogisticoAtacado"] != DBNull.Value ? Convert.ToDecimal(reader["LogisticoAtacado"]) : 0,
                                DevolucaoAtacado = reader["DevolucaoAtacado"] != DBNull.Value ? Convert.ToDecimal(reader["DevolucaoAtacado"]) : 0,
                                AniversarioAtacado = reader["AniversarioAtacado"] != DBNull.Value ? Convert.ToDecimal(reader["AniversarioAtacado"]) : 0,
                                ReinauguracaoAtacado = reader["ReinauguracaoAtacado"] != DBNull.Value ? Convert.ToDecimal(reader["ReinauguracaoAtacado"]) : 0,
                                CadastroAtacado = reader["CadastroAtacado"] != DBNull.Value ? Convert.ToDecimal(reader["CadastroAtacado"]) : 0,
                                FinanceiroAtacado = reader["FinanceiroAtacado"] != DBNull.Value ? Convert.ToDecimal(reader["FinanceiroAtacado"]) : 0,
                                MarketingAtacado = reader["MarketingAtacado"] != DBNull.Value ? Convert.ToDecimal(reader["MarketingAtacado"]) : 0,
                            };
                        }
                    }
                }
            }
            return null;
        }

        private async Task UpdateFornecedorData(FornecedorData fornecedor)
        {
            var connectionString = _configuration.GetConnectionString("PortalArcomixDB");
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                await con.OpenAsync();
                string query = @"
                    UPDATE Tbl_Fornecedor
                    SET RazaoSocial = @RazaoSocial,
                        Fantasia = @Fantasia,
                        IE = @IE,
                        CNAE = @CNAE,
                        CEP = @CEP,
                        Logradouro = @Logradouro,
                        NumeroEndereco = @NumeroEndereco,
                        Bairro = @Bairro,
                        Complemento = @Complemento,
                        UF = @UF,
                        Cidade = @Cidade,
                        MicroEmpresa = @MicroEmpresa,
                        ContribuiICMS = @ContribuiICMS,
                        ContribuiIPI = @ContribuiIPI,
                        TipoFornecedor = @TipoFornecedor,
                        FornecedorAlimentos = @FornecedorAlimentos,
                        CompradorPrincipal = @CompradorPrincipal
                    WHERE CNPJ = @CNPJ";
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@CNPJ", fornecedor.CNPJ);
                    cmd.Parameters.AddWithValue("@RazaoSocial", fornecedor.RazaoSocial);
                    cmd.Parameters.AddWithValue("@Fantasia", fornecedor.Fantasia);
                    cmd.Parameters.AddWithValue("@IE", fornecedor.IE);
                    cmd.Parameters.AddWithValue("@CNAE", fornecedor.CNAE);
                    cmd.Parameters.AddWithValue("@CEP", fornecedor.CEP);
                    cmd.Parameters.AddWithValue("@Logradouro", fornecedor.Logradouro);
                    cmd.Parameters.AddWithValue("@NumeroEndereco", fornecedor.NumeroEndereco);
                    cmd.Parameters.AddWithValue("@Bairro", fornecedor.Bairro);
                    cmd.Parameters.AddWithValue("@Complemento", fornecedor.Complemento);
                    cmd.Parameters.AddWithValue("@UF", fornecedor.UF);
                    cmd.Parameters.AddWithValue("@Cidade", fornecedor.Cidade);
                    cmd.Parameters.AddWithValue("@MicroEmpresa", fornecedor.MicroEmpresa);
                    cmd.Parameters.AddWithValue("@ContribuiICMS", fornecedor.ContribuiICMS);
                    cmd.Parameters.AddWithValue("@ContribuiIPI", fornecedor.ContribuiIPI);
                    cmd.Parameters.AddWithValue("@TipoFornecedor", fornecedor.TipoFornecedor);
                    cmd.Parameters.AddWithValue("@FornecedorAlimentos", fornecedor.FornecedorAlimentos);
                    cmd.Parameters.AddWithValue("@CompradorPrincipal", fornecedor.CompradorPrincipal);

                    await cmd.ExecuteNonQueryAsync();
                }
            }
        }

        private async Task UpdateDadosBancariosData(DadosBancariosData dadosBancarios)
        {
            var connectionString = _configuration.GetConnectionString("PortalArcomixDB");
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                await con.OpenAsync();
                string query = @"
                    UPDATE Tbl_DadosBancariosFornecedor
                    SET Banco = @Banco,
                        Agencia = @Agencia,
                        TipoConta = @TipoConta,
                        NumeroConta = @NumeroConta,
                        CNPJContaTitular = @CNPJContaTitular
                    WHERE CNPJ = @CNPJ";
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@CNPJ", dadosBancarios.CNPJ);
                    cmd.Parameters.AddWithValue("@Banco", dadosBancarios.Banco);
                    cmd.Parameters.AddWithValue("@Agencia", dadosBancarios.Agencia);
                    cmd.Parameters.AddWithValue("@TipoConta", dadosBancarios.TipoConta);
                    cmd.Parameters.AddWithValue("@NumeroConta", dadosBancarios.NumeroConta);
                    cmd.Parameters.AddWithValue("@CNPJContaTitular", dadosBancarios.CNPJContaTitular);

                    await cmd.ExecuteNonQueryAsync();
                }
            }
        }

        private async Task UpdateContatosData(ContatosData contatos)
        {
            var connectionString = _configuration.GetConnectionString("PortalArcomixDB");
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                await con.OpenAsync();
                string query = @"
                    UPDATE Tbl_ContatosFornecedor
                    SET ContatoVendedor = @ContatoVendedor,
                        DDDVendedor = @DDDVendedor,
                        TelefoneVendedor = @TelefoneVendedor,
                        EmailVendedor = @EmailVendedor,
                        ContatoGerente = @ContatoGerente,
                        DDDGerente = @DDDGerente,
                        TelefoneGerente = @TelefoneGerente,
                        EmailGerente = @EmailGerente,
                        ResponsavelFinanceiro = @ResponsavelFinanceiro,
                        DDDRespFinanceiro = @DDDRespFinanceiro,
                        TelefoneRespFinanceiro = @TelefoneRespFinanceiro,
                        EmailRespFinanceiro = @EmailRespFinanceiro,
                        DDDTelefoneFixoEmpresa = @DDDTelefoneFixoEmpresa,
                        TelefoneFixoEmpresa = @TelefoneFixoEmpresa
                    WHERE CNPJ = @CNPJ";
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@CNPJ", contatos.CNPJ);
                    cmd.Parameters.AddWithValue("@ContatoVendedor", contatos.ContatoVendedor);
                    cmd.Parameters.AddWithValue("@DDDVendedor", contatos.DDDVendedor);
                    cmd.Parameters.AddWithValue("@TelefoneVendedor", contatos.TelefoneVendedor);
                    cmd.Parameters.AddWithValue("@EmailVendedor", contatos.EmailVendedor);
                    cmd.Parameters.AddWithValue("@ContatoGerente", contatos.ContatoGerente);
                    cmd.Parameters.AddWithValue("@DDDGerente", contatos.DDDGerente);
                    cmd.Parameters.AddWithValue("@TelefoneGerente", contatos.TelefoneGerente);
                    cmd.Parameters.AddWithValue("@EmailGerente", contatos.EmailGerente);
                    cmd.Parameters.AddWithValue("@ResponsavelFinanceiro", contatos.ResponsavelFinanceiro);
                    cmd.Parameters.AddWithValue("@DDDRespFinanceiro", contatos.DDDRespFinanceiro);
                    cmd.Parameters.AddWithValue("@TelefoneRespFinanceiro", contatos.TelefoneRespFinanceiro);
                    cmd.Parameters.AddWithValue("@EmailRespFinanceiro", contatos.EmailRespFinanceiro);
                    cmd.Parameters.AddWithValue("@DDDTelefoneFixoEmpresa", contatos.DDDTelefoneFixoEmpresa);
                    cmd.Parameters.AddWithValue("@TelefoneFixoEmpresa", contatos.TelefoneFixoEmpresa);

                    await cmd.ExecuteNonQueryAsync();
                }
            }
        }

        private async Task UpdateFornecedorNegociacaoData(FornecedorNegociacao fornecedorNegociacao)
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
                    cmd.Parameters.AddWithValue("@CNPJ", fornecedorNegociacao.CNPJ);
                    cmd.Parameters.AddWithValue("@DataBaseVencimento", fornecedorNegociacao.DataBaseVencimento);
                    cmd.Parameters.AddWithValue("@PrazoPagamentoDias", fornecedorNegociacao.PrazoPagamentoDias);
                    cmd.Parameters.AddWithValue("@PrazoEntregaMedioDias", fornecedorNegociacao.PrazoEntregaMedioDias);
                    cmd.Parameters.AddWithValue("@PrazoMedioAtrasoDias", fornecedorNegociacao.PrazoMedioAtrasoDias);
                    cmd.Parameters.AddWithValue("@PrazoMedioVisitaDias", fornecedorNegociacao.PrazoMedioVisitaDias);
                    cmd.Parameters.AddWithValue("@VerbaCadastro", fornecedorNegociacao.VerbaCadastro);
                    cmd.Parameters.AddWithValue("@MotivoVerbaZerada", fornecedorNegociacao.MotivoVerbaZerada);
                    cmd.Parameters.AddWithValue("@JustificativaSemVerba", fornecedorNegociacao.JustificativaSemVerba);
                    cmd.Parameters.AddWithValue("@DivisaoVerba", fornecedorNegociacao.DivisaoVerba);
                    cmd.Parameters.AddWithValue("@ContratoFornecedor", fornecedorNegociacao.ContratoFornecedor);
                    cmd.Parameters.AddWithValue("@ApuracaoContrato", fornecedorNegociacao.ApuracaoContrato);
                    cmd.Parameters.AddWithValue("@TipoContrato", fornecedorNegociacao.TipoContrato);
                    cmd.Parameters.AddWithValue("@MotivoSemContrato", fornecedorNegociacao.MotivoSemContrato);
                    cmd.Parameters.AddWithValue("@JustificativaSemContrato", fornecedorNegociacao.JustificativaSemContrato);
                    cmd.Parameters.AddWithValue("@TotalPercentualVarejo", fornecedorNegociacao.TotalPercentualVarejo);
                    cmd.Parameters.AddWithValue("@TotalPercentualAtacado", fornecedorNegociacao.TotalPercentualAtacado);
                    cmd.Parameters.AddWithValue("@LogisticoVarejo", fornecedorNegociacao.LogisticoVarejo);
                    cmd.Parameters.AddWithValue("@DevolucaoVarejo", fornecedorNegociacao.DevolucaoVarejo);
                    cmd.Parameters.AddWithValue("@AniversarioVarejo", fornecedorNegociacao.AniversarioVarejo);
                    cmd.Parameters.AddWithValue("@ReinauguracaoVarejo", fornecedorNegociacao.ReinauguracaoVarejo);
                    cmd.Parameters.AddWithValue("@CadastroVarejo", fornecedorNegociacao.CadastroVarejo);
                    cmd.Parameters.AddWithValue("@FinanceiroVarejo", fornecedorNegociacao.FinanceiroVarejo);
                    cmd.Parameters.AddWithValue("@MarketingVarejo", fornecedorNegociacao.MarketingVarejo);
                    cmd.Parameters.AddWithValue("@LogisticoAtacado", fornecedorNegociacao.LogisticoAtacado);
                    cmd.Parameters.AddWithValue("@DevolucaoAtacado", fornecedorNegociacao.DevolucaoAtacado);
                    cmd.Parameters.AddWithValue("@AniversarioAtacado", fornecedorNegociacao.AniversarioAtacado);
                    cmd.Parameters.AddWithValue("@ReinauguracaoAtacado", fornecedorNegociacao.ReinauguracaoAtacado);
                    cmd.Parameters.AddWithValue("@CadastroAtacado", fornecedorNegociacao.CadastroAtacado);
                    cmd.Parameters.AddWithValue("@FinanceiroAtacado", fornecedorNegociacao.FinanceiroAtacado);
                    cmd.Parameters.AddWithValue("@MarketingAtacado", fornecedorNegociacao.MarketingAtacado);

                    await cmd.ExecuteNonQueryAsync();
                }
            }
        }
    }

    public class FornecedorData
    {
        public string CNPJ { get; set; }
        public string RazaoSocial { get; set; }
        public string Fantasia { get; set; }
        public string IE { get; set; }
        public string CNAE { get; set; }
        public string CEP { get; set; }
        public string Logradouro { get; set; }
        public string NumeroEndereco { get; set; }
        public string Bairro { get; set; }
        public string Complemento { get; set; }
        public string UF { get; set; }
        public string Cidade { get; set; }
        public string MicroEmpresa { get; set; }
        public string ContribuiICMS { get; set; }
        public string ContribuiIPI { get; set; }
        public string TipoFornecedor { get; set; }
        public string FornecedorAlimentos { get; set; }
        public string CompradorPrincipal { get; set; }
    }

    public class DadosBancariosData
    {
        public string CNPJ { get; set; }
        public string Banco { get; set; }
        public string Agencia { get; set; }
        public string TipoConta { get; set; }
        public string NumeroConta { get; set; }
        public string CNPJContaTitular { get; set; }
    }

    public class ContatosData
    {
        public string CNPJ { get; set; }
        public string ContatoVendedor { get; set; }
        public string DDDVendedor { get; set; }
        public string TelefoneVendedor { get; set; }
        public string EmailVendedor { get; set; }
        public string ContatoGerente { get; set; }
        public string DDDGerente { get; set; }
        public string TelefoneGerente { get; set; }
        public string EmailGerente { get; set; }
        public string ResponsavelFinanceiro { get; set; }
        public string DDDRespFinanceiro { get; set; }
        public string TelefoneRespFinanceiro { get; set; }
        public string EmailRespFinanceiro { get; set; }
        public string DDDTelefoneFixoEmpresa { get; set; }
        public string TelefoneFixoEmpresa { get; set; }
    }

    public class FornecedorNegociacao
    {
        public string CNPJ { get; set; }
        public string DataBaseVencimento { get; set; }
        public int PrazoPagamentoDias { get; set; }
        public int PrazoEntregaMedioDias { get; set; }
        public int PrazoMedioAtrasoDias { get; set; }
        public int PrazoMedioVisitaDias { get; set; }
        public decimal VerbaCadastro { get; set; }
        public string MotivoVerbaZerada { get; set; }
        public string JustificativaSemVerba { get; set; }
        public string DivisaoVerba { get; set; }
        public string ContratoFornecedor { get; set; }
        public string ApuracaoContrato { get; set; }
        public string TipoContrato { get; set; }
        public string MotivoSemContrato { get; set; }
        public string JustificativaSemContrato { get; set; }
        public decimal TotalPercentualVarejo { get; set; }
        public decimal TotalPercentualAtacado { get; set; }
        public decimal LogisticoVarejo { get; set; }
        public decimal DevolucaoVarejo { get; set; }
        public decimal AniversarioVarejo { get; set; }
        public decimal ReinauguracaoVarejo { get; set; }
        public decimal CadastroVarejo { get; set; }
        public decimal FinanceiroVarejo { get; set; }
        public decimal MarketingVarejo { get; set; }
        public decimal LogisticoAtacado { get; set; }
        public decimal DevolucaoAtacado { get; set; }
        public decimal AniversarioAtacado { get; set; }
        public decimal ReinauguracaoAtacado { get; set; }
        public decimal CadastroAtacado { get; set; }
        public decimal FinanceiroAtacado { get; set; }
        public decimal MarketingAtacado { get; set; }
    }
}
