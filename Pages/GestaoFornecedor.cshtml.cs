using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace PortalArcomix.Pages
{
    public class GestaoFornecedorModel : PageModel
    {
        private readonly IConfiguration _configuration;

        public GestaoFornecedorModel(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [BindProperty]
        public string Filter { get; set; }

        public List<Fornecedor> Fornecedores { get; set; } = new List<Fornecedor>();

        public async Task<IActionResult> OnGet()
        {
            // Check if the user is authenticated
            if (User?.Identity?.IsAuthenticated != true)
            {
                // Redirect to the Login page if the user is not authenticated
                return RedirectToPage("/Login");
            }

            await LoadFornecedoresAsync();
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            // Check if the user is authenticated
            if (User?.Identity?.IsAuthenticated != true)
            {
                // Redirect to the Login page if the user is not authenticated
                return RedirectToPage("/Login");
            }

            await LoadFornecedoresAsync(Filter);
            return Page();
        }

        public IActionResult OnPostClaimCnpj(string cnpj)
        {
            if (User?.Identity?.IsAuthenticated != true)
            {
                return RedirectToPage("/Login");
            }

            // Set the session with the CNPJ for all users except fornecedores
            var userRole = User.Claims.FirstOrDefault(c => c.Type == "role")?.Value;
            if (userRole != "fornecedor")
            {
                HttpContext.Session.SetString("CNPJ", cnpj);
            }

            // Redirect to the FornecedorVI page
            return RedirectToPage("/FornecedorVI");
        }

        private async Task LoadFornecedoresAsync(string filter = null)
        {
            string connectionString = _configuration.GetConnectionString("PortalArcomixDB");

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                string query = @"SELECT [ID], [CNPJ], [RazaoSocial], [FornecedorAlimentos], [CompradorPrincipal]
                                 FROM [Tbl_Fornecedor]";
                if (!string.IsNullOrEmpty(filter))
                {
                    // Split the filter string by comma and trim each part
                    string[] filters = filter.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                                             .Select(f => f.Trim()).ToArray();

                    if (filters.Length > 0)
                    {
                        List<string> conditions = new List<string>();

                        for (int i = 0; i < filters.Length; i++)
                        {
                            string paramName = $"@Filter{i}";
                            List<string> subConditions = new List<string>
                            {
                                $"[ID] LIKE {paramName}",
                                $"[CNPJ] LIKE {paramName}",
                                $"[RazaoSocial] LIKE {paramName}",
                                $"[FornecedorAlimentos] LIKE {paramName}",
                                $"[CompradorPrincipal] LIKE {paramName}"
                            };

                            // Combine sub-conditions with OR and wrap them in parentheses
                            conditions.Add("(" + string.Join(" OR ", subConditions) + ")");
                        }

                        // Combine main conditions with AND
                        query += " WHERE " + string.Join(" AND ", conditions);
                    }

                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        for (int i = 0; i < filters.Length; i++)
                        {
                            cmd.Parameters.AddWithValue($"@Filter{i}", "%" + filters[i] + "%");
                        }

                        con.Open();
                        SqlDataReader reader = await cmd.ExecuteReaderAsync();
                        while (await reader.ReadAsync())
                        {
                            Fornecedores.Add(new Fornecedor
                            {
                                ID = (int)reader["ID"],
                                CNPJ = reader["CNPJ"].ToString(),
                                RazaoSocial = reader["RazaoSocial"].ToString(),
                                FornecedorAlimentos = reader["FornecedorAlimentos"].ToString(),
                                CompradorPrincipal = reader["CompradorPrincipal"].ToString()
                            });
                        }
                    }
                }
                else
                {
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        con.Open();
                        SqlDataReader reader = await cmd.ExecuteReaderAsync();
                        while (await reader.ReadAsync())
                        {
                            Fornecedores.Add(new Fornecedor
                            {
                                ID = (int)reader["ID"],
                                CNPJ = reader["CNPJ"].ToString(),
                                RazaoSocial = reader["RazaoSocial"].ToString(),
                                FornecedorAlimentos = reader["FornecedorAlimentos"].ToString(),
                                CompradorPrincipal = reader["CompradorPrincipal"].ToString()
                            });
                        }
                    }
                }
            }
        }

        public class Fornecedor
        {
            public int ID { get; set; }
            public string CNPJ { get; set; }
            public string RazaoSocial { get; set; }
            public string FornecedorAlimentos { get; set; }
            public string CompradorPrincipal { get; set; }
        }
    }
}



