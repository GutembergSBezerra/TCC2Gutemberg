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


