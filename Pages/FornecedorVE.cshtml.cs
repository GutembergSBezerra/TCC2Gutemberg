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

        public async Task<IActionResult> OnGetAsync()
        {
            var cnpj = User.FindFirst("CNPJ")?.Value;
            if (string.IsNullOrEmpty(cnpj))
            {
                return RedirectToPage("/Login");
            }

            Fornecedor = await GetFornecedorData(cnpj);
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

            await UpdateFornecedorData(Fornecedor);

            // Refresh the data
            Fornecedor = await GetFornecedorData(cnpj);

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
}