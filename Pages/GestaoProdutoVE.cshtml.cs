using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace PortalArcomix.Pages
{
    public class GestaoProdutoVEModel : PageModel
    {
        private readonly IConfiguration _configuration;

        public GestaoProdutoVEModel(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public List<ProdutoViewModel> Produtos { get; set; } = new();

        [BindProperty]
        public string? Filter { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            if (!User.IsInRole("Fornecedor"))
            {
                return Forbid();
            }

            await LoadProdutosAsync();
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!User.IsInRole("Fornecedor"))
            {
                return Forbid();
            }

            await LoadProdutosAsync(Filter);
            return Page();
        }

        private async Task LoadProdutosAsync(string? filter = null)
        {
            var connectionString = _configuration.GetConnectionString("PortalArcomixDB");
            var cnpjClaim = User.FindFirst("CNPJ")?.Value;

            if (string.IsNullOrEmpty(cnpjClaim))
            {
                // Handle case where CNPJ is not found
                return;
            }

            using (var conn = new SqlConnection(connectionString))
            {
                var query = @"
                    SELECT p.ID, p.Marca, p.DescricaoProduto, p.GestorCompras, se.EAN13 
                    FROM Tbl_Produto p
                    LEFT JOIN Tbl_ProdutoSubEmbalagem se ON p.ID = se.ProdutoID
                    WHERE p.CNPJ = @CNPJ";

                if (!string.IsNullOrEmpty(filter))
                {
                    var filters = filter.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                                        .Select(f => f.Trim()).ToArray();

                    if (filters.Length > 0)
                    {
                        var conditions = new List<string>();

                        for (int i = 0; i < filters.Length; i++)
                        {
                            var paramName = $"@Filter{i}";
                            var subConditions = new List<string>
                            {
                                $"p.ID LIKE {paramName}",
                                $"p.Marca LIKE {paramName}",
                                $"p.DescricaoProduto LIKE {paramName}",
                                $"p.GestorCompras LIKE {paramName}",
                                $"se.EAN13 LIKE {paramName}"
                            };

                            conditions.Add("(" + string.Join(" OR ", subConditions) + ")");
                        }

                        query += " AND (" + string.Join(" AND ", conditions) + ")";
                    }

                    using (var cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@CNPJ", cnpjClaim);

                        for (int i = 0; i < filters.Length; i++)
                        {
                            cmd.Parameters.AddWithValue($"@Filter{i}", "%" + filters[i] + "%");
                        }

                        await conn.OpenAsync();
                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                Produtos.Add(new ProdutoViewModel
                                {
                                    ID = reader.GetInt32(0),
                                    Marca = reader.IsDBNull(1) ? null : reader.GetString(1),
                                    DescricaoProduto = reader.IsDBNull(2) ? null : reader.GetString(2),
                                    GestorCompras = reader.IsDBNull(3) ? null : reader.GetString(3),
                                    EAN13 = reader.IsDBNull(4) ? null : reader.GetString(4)
                                });
                            }
                        }
                    }
                }
                else
                {
                    using (var cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@CNPJ", cnpjClaim);

                        await conn.OpenAsync();
                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                Produtos.Add(new ProdutoViewModel
                                {
                                    ID = reader.GetInt32(0),
                                    Marca = reader.IsDBNull(1) ? null : reader.GetString(1),
                                    DescricaoProduto = reader.IsDBNull(2) ? null : reader.GetString(2),
                                    GestorCompras = reader.IsDBNull(3) ? null : reader.GetString(3),
                                    EAN13 = reader.IsDBNull(4) ? null : reader.GetString(4)
                                });
                            }
                        }
                    }
                }
            }
        }

        public class ProdutoViewModel
        {
            public int ID { get; set; }
            public string? Marca { get; set; }
            public string? DescricaoProduto { get; set; }
            public string? GestorCompras { get; set; }
            public string? EAN13 { get; set; }
        }
    }
}