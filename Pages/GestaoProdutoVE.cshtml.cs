using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PortalArcomix.Data.Entities;
using PortalArcomix.Data;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace PortalArcomix.Pages
{
    public class GestaoProdutoVEModel : PageModel
    {
        private readonly OracleDbContext _context;

        public GestaoProdutoVEModel(OracleDbContext context)
        {
            _context = context;
        }

        public List<Tbl_Produto> Produtos { get; set; } = new List<Tbl_Produto>();

        [BindProperty]
        public string Filter { get; set; } = string.Empty;

        public async Task<IActionResult> OnGetAsync()
        {
            if (User?.Identity?.IsAuthenticated != true)
            {
                return RedirectToPage("/Login");
            }

            var cnpjClaim = User.Claims.FirstOrDefault(c => c.Type == "CNPJ")?.Value;

            if (cnpjClaim == null)
            {
                return RedirectToPage("/Login");
            }

            IQueryable<Tbl_Produto> query = _context.Tbl_Produto
                .Where(p => p.CNPJ == cnpjClaim);

            if (!string.IsNullOrEmpty(Filter))
            {
                string[] filterParts = Filter.Split('%'); // Split filter by %

                foreach (var part in filterParts)
                {
                    string trimmedPart = part.Trim().ToUpper();
                    int idValue;
                    bool isIdFilter = int.TryParse(trimmedPart, out idValue);

                    if (isIdFilter)
                    {
                        query = query.Where(p => p.ID == idValue);
                    }
                    else
                    {
                        query = query.Where(p => EF.Functions.Like(p.MARCA.ToUpper(), $"%{trimmedPart}%") ||
                                                 EF.Functions.Like(p.DESCRICAOPRODUTO.ToUpper(), $"%{trimmedPart}%") ||
                                                 EF.Functions.Like(p.GESTORCOMPRAS.ToUpper(), $"%{trimmedPart}%"));
                    }
                }
            }

            Produtos = await query.ToListAsync();

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (User?.Identity?.IsAuthenticated != true)
            {
                return RedirectToPage("/Login");
            }

            var cnpjClaim = User.Claims.FirstOrDefault(c => c.Type == "CNPJ")?.Value;

            if (cnpjClaim == null)
            {
                return RedirectToPage("/Login");
            }

            IQueryable<Tbl_Produto> query = _context.Tbl_Produto
                .Where(p => p.CNPJ == cnpjClaim);

            if (!string.IsNullOrEmpty(Filter))
            {
                string[] filterParts = Filter.Split('%'); // Split filter by %

                foreach (var part in filterParts)
                {
                    string trimmedPart = part.Trim().ToUpper();
                    int idValue;
                    bool isIdFilter = int.TryParse(trimmedPart, out idValue);

                    if (isIdFilter)
                    {
                        query = query.Where(p => p.ID == idValue);
                    }
                    else
                    {
                        query = query.Where(p => EF.Functions.Like(p.MARCA.ToUpper(), $"%{trimmedPart}%") ||
                                                 EF.Functions.Like(p.DESCRICAOPRODUTO.ToUpper(), $"%{trimmedPart}%") ||
                                                 EF.Functions.Like(p.GESTORCOMPRAS.ToUpper(), $"%{trimmedPart}%"));
                    }
                }
            }

            Produtos = await query.ToListAsync();

            return Page();
        }

        public async Task<IActionResult> OnPostClaimProductIdAsync(int id)
        {
            // Retrieve the existing claims
            var identity = (ClaimsIdentity)User.Identity;
            var existingClaim = identity.FindFirst("ProdutoID");

            // If the claim already exists, remove it
            if (existingClaim != null)
            {
                identity.RemoveClaim(existingClaim);
            }

            // Add the new claim with the product ID from Tbl_Produto
            identity.AddClaim(new Claim("ProdutoID", id.ToString()));

            // Update the user's claims in the authentication cookie
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(identity));

            return RedirectToPage("/ProdutosVE"); // Redirect to the next page
        }

    }
}