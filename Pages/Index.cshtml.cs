using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PortalArcomix.Data;
using PortalArcomix.Data.Entities;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace PortalArcomix.Pages
{
    public class IndexModel : PageModel
    {
        private readonly OracleDbContext _context;

        public IndexModel(OracleDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            // Check if the user is authenticated
            if (User?.Identity?.IsAuthenticated != true)
            {
                // Redirect to the Login page if the user is not authenticated
                return RedirectToPage("/Login");
            }

            // Retrieve the CNPJ claim from the authenticated user
            var cnpjClaim = User.Claims.FirstOrDefault(c => c.Type == "CNPJ")?.Value;

            if (cnpjClaim != null)
            {
                // Check if the CNPJ exists in the Tbl_Fornecedor table
                var fornecedorExists = await _context.Tbl_Fornecedor.AnyAsync(f => f.CNPJ == cnpjClaim);

                if (!fornecedorExists)
                {
                    // Insert a new entry with the CNPJ
                    var newFornecedor = new Tbl_Fornecedor
                    {
                        CNPJ = cnpjClaim
                        // Set other properties as needed
                    };

                    _context.Tbl_Fornecedor.Add(newFornecedor);
                    await _context.SaveChangesAsync();
                }
            }

            // If authenticated, continue with the normal page processing
            return Page();
        }
    }
}
