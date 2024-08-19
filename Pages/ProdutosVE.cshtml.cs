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

        [BindProperty]
        public Tbl_ProdutoVendaCompra ProdutoVendaCompra { get; set; }  // Binding for ProdutoVendaCompra entity

        public async Task<IActionResult> OnGetAsync(int? id)
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
                ProdutoVendaCompra = new Tbl_ProdutoVendaCompra();  // Initialize a new ProdutoVendaCompra
            }
            else  // If an id is provided, load the existing product for editing
            {
                Produto = await _context.Tbl_Produto.FirstOrDefaultAsync(p => p.ID == id);
                ProdutoVendaCompra = await _context.Tbl_ProdutoVendaCompra.FirstOrDefaultAsync(pvc => pvc.PRODUTOID == id);

                if (Produto == null)
                {
                    return NotFound();  // Return 404 if the product is not found
                }

                if (ProdutoVendaCompra == null)
                {
                    ProdutoVendaCompra = new Tbl_ProdutoVendaCompra { PRODUTOID = Produto.ID };  // Initialize if not found
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
                await _context.SaveChangesAsync();

                // Now create the corresponding ProdutoVendaCompra record
                ProdutoVendaCompra.ID = Produto.ID;  // Set ID to match the product ID
                _context.Tbl_ProdutoVendaCompra.Add(ProdutoVendaCompra);
            }
            else  // Otherwise, update the existing product
            {
                var produtoToUpdate = await _context.Tbl_Produto.FirstOrDefaultAsync(p => p.ID == Produto.ID && p.CNPJ == cnpjClaim);
                var produtoVendaCompraToUpdate = await _context.Tbl_ProdutoVendaCompra.FirstOrDefaultAsync(pvc => pvc.ID == Produto.ID);

                if (produtoToUpdate == null || produtoVendaCompraToUpdate == null)
                {
                    return NotFound();
                }

                // Update the Tbl_Produto fields
                produtoToUpdate.GESTORCOMPRAS = Produto.GESTORCOMPRAS;
                produtoToUpdate.DESCRICAOPRODUTO = Produto.DESCRICAOPRODUTO;
                produtoToUpdate.IMPORTADO = Produto.IMPORTADO;
                produtoToUpdate.MARCA = Produto.MARCA;
                produtoToUpdate.NCM = Produto.NCM;
                produtoToUpdate.CEST = Produto.CEST;
                produtoToUpdate.ICMS = Produto.ICMS;
                produtoToUpdate.IPI = Produto.IPI;
                produtoToUpdate.PIS = Produto.PIS;
                produtoToUpdate.COFINS = Produto.COFINS;
                produtoToUpdate.CUSTOUNIDADE = Produto.CUSTOUNIDADE;
                produtoToUpdate.CUSTOCAIXA = Produto.CUSTOCAIXA;
                produtoToUpdate.CODXML = Produto.CODXML;
                produtoToUpdate.EMBALAGEMFAT = Produto.EMBALAGEMFAT;
                produtoToUpdate.VERBACADASTRO = Produto.VERBACADASTRO;
                produtoToUpdate.MOTIVOVERBAZERADA = Produto.MOTIVOVERBAZERADA;

                _context.Attach(produtoToUpdate).State = EntityState.Modified;

                // Update the Tbl_ProdutoVendaCompra fields
                produtoVendaCompraToUpdate.EAN13 = ProdutoVendaCompra.EAN13;
                produtoVendaCompraToUpdate.REFERENCIA = ProdutoVendaCompra.REFERENCIA;
                produtoVendaCompraToUpdate.PESOBRUTOKG = ProdutoVendaCompra.PESOBRUTOKG;
                produtoVendaCompraToUpdate.PESOLIQUIDOKG = ProdutoVendaCompra.PESOLIQUIDOKG;
                produtoVendaCompraToUpdate.ALTURACM = ProdutoVendaCompra.ALTURACM;
                produtoVendaCompraToUpdate.LARGURACM = ProdutoVendaCompra.LARGURACM;
                produtoVendaCompraToUpdate.PROFUNDIDADECM = ProdutoVendaCompra.PROFUNDIDADECM;
                produtoVendaCompraToUpdate.DUN14 = ProdutoVendaCompra.DUN14;
                produtoVendaCompraToUpdate.REFERENCIADUN14 = ProdutoVendaCompra.REFERENCIADUN14;
                produtoVendaCompraToUpdate.PESOBRUTODUN14KG = ProdutoVendaCompra.PESOBRUTODUN14KG;
                produtoVendaCompraToUpdate.PESOLIQUIDODUN14KG = ProdutoVendaCompra.PESOLIQUIDODUN14KG;
                produtoVendaCompraToUpdate.ALTURADUN14CM = ProdutoVendaCompra.ALTURADUN14CM;
                produtoVendaCompraToUpdate.LARGURADUN14CM = ProdutoVendaCompra.LARGURADUN14CM;
                produtoVendaCompraToUpdate.PROFUNDIDADEDUN14CM = ProdutoVendaCompra.PROFUNDIDADEDUN14CM;
                produtoVendaCompraToUpdate.EMBALAGEM = ProdutoVendaCompra.EMBALAGEM;
                produtoVendaCompraToUpdate.QUANTIDADEUNIDADES = ProdutoVendaCompra.QUANTIDADEUNIDADES;
                produtoVendaCompraToUpdate.MESACAIXAS = ProdutoVendaCompra.MESACAIXAS;  
                produtoVendaCompraToUpdate.ALTURACAIXAS = ProdutoVendaCompra.ALTURACAIXAS;
                produtoVendaCompraToUpdate.SHELFLIFEDIAS = ProdutoVendaCompra.SHELFLIFEDIAS;

                _context.Attach(produtoVendaCompraToUpdate).State = EntityState.Modified;
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