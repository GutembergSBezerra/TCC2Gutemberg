using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PortalArcomix.Data.Entities;
using PortalArcomix.Data;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace PortalArcomix.Pages
{
    public class ProdutosVEModel : PageModel
    {
        private readonly OracleDbContext _context;

        public ProdutosVEModel(OracleDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Tbl_Produto Produto { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)  // id is nullable to handle both new and existing products
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

            if (id == null)  // If no id is provided, prepare for a new product
            {
                Produto = new Tbl_Produto();  // Initialize a new product
            }
            else  // If an id is provided, load the existing product for editing
            {
                Produto = await _context.Tbl_Produto.FirstOrDefaultAsync(p => p.ID == id);

                if (Produto == null)
                {
                    return NotFound();  // Return 404 if the product is not found
                }
            }

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

            if (!ModelState.IsValid)
            {
                return Page();
            }

            // Handle MARCA field based on the "Nova Marca" selection
            if (Request.Form["Produto.MARCA"] == "Nova Marca")
            {
                Produto.MARCA = Request.Form["novaMarcaInput"];
            }

            if (Produto.ID == 0)  // If ID is 0, it's a new product
            {
                Produto.CNPJ = cnpjClaim;  // Set the CNPJ to the claimed CNPJ
                _context.Tbl_Produto.Add(Produto);
            }
            else  // Otherwise, update the existing product
            {
                var produtoToUpdate = await _context.Tbl_Produto.FirstOrDefaultAsync(p => p.ID == Produto.ID && p.CNPJ == cnpjClaim);

                if (produtoToUpdate == null)
                {
                    return NotFound();
                }

                // Update the product fields
                produtoToUpdate.GESTORCOMPRAS = Produto.GESTORCOMPRAS;
                produtoToUpdate.DESCRICAOPRODUTO = Produto.DESCRICAOPRODUTO;
                produtoToUpdate.IMPORTADO = Produto.IMPORTADO;
                produtoToUpdate.MARCA = Produto.MARCA;

                _context.Attach(produtoToUpdate).State = EntityState.Modified;
            }

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProdutoExists(Produto.ID))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            // Redirect to the Index page after successful save
            return RedirectToPage("/Index");
        }

        private bool ProdutoExists(int id)
        {
            return _context.Tbl_Produto.Any(e => e.ID == id);
        }
    }
}
