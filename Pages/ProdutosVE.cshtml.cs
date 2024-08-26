using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PortalArcomix.Data.Entities;
using PortalArcomix.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using System;

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
        public Tbl_ProdutoVendaCompra ProdutoVendaCompra { get; set; }

        [BindProperty]
        public Tbl_ProdutoSubEmbalagem ProdutoSubEmbalagem { get; set; }

        [BindProperty]
        public Tbl_ProdutoComentarios? ProdutoComentario { get; set; }  // Binding for comments

        [BindProperty]
        public List<string?> AvailableMarcas { get; set; }  // For storing unique Marca values

        public async Task<IActionResult> OnGetAsync(bool newProduct = false)
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

            if (newProduct)
            {
                // Initialize a new product object for creation
                Produto = new Tbl_Produto
                {
                    CNPJ = cnpjClaim,
                };
                ProdutoVendaCompra = new Tbl_ProdutoVendaCompra();
                ProdutoSubEmbalagem = new Tbl_ProdutoSubEmbalagem();
                ProdutoComentario = null; // No comments yet
            }
            else
            {
                var produtoIdClaim = User.Claims.FirstOrDefault(c => c.Type == "ProdutoID")?.Value;

                if (!int.TryParse(produtoIdClaim, out int id))
                {
                    return NotFound(); // Handle the case where the ProdutoID claim is not valid
                }

                Produto = await _context.Tbl_Produto.FirstOrDefaultAsync(p => p.ID == id);
                ProdutoVendaCompra = await _context.Tbl_ProdutoVendaCompra.FirstOrDefaultAsync(p => p.ID == id);
                ProdutoSubEmbalagem = await _context.Tbl_ProdutoSubEmbalagem.FirstOrDefaultAsync(p => p.ID == id);

                if (Produto == null)
                {
                    return NotFound();
                }

                if (ProdutoVendaCompra == null)
                {
                    ProdutoVendaCompra = new Tbl_ProdutoVendaCompra { ID = Produto.ID };
                }

                if (ProdutoSubEmbalagem == null)
                {
                    ProdutoSubEmbalagem = new Tbl_ProdutoSubEmbalagem { ID = Produto.ID };
                }
            }

            // Retrieve the list of unique MARCA values
            AvailableMarcas = await _context.Tbl_Produto
                .Where(p => !string.IsNullOrEmpty(p.MARCA))
                .Select(p => p.MARCA)
                .Distinct()
                .ToListAsync();

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(bool newProduct = false)
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
                if (!newProduct)
                {
                    // Re-fetch the Produto from the database to maintain the correct ID and other properties
                    var produtoIdClaim = User.Claims.FirstOrDefault(c => c.Type == "ProdutoID")?.Value;

                    if (!int.TryParse(produtoIdClaim, out int id))
                    {
                        return NotFound(); // Handle the case where the ProdutoID claim is not valid
                    }

                    Produto = await _context.Tbl_Produto.FirstOrDefaultAsync(p => p.ID == id);
                    ProdutoVendaCompra = await _context.Tbl_ProdutoVendaCompra.FirstOrDefaultAsync(p => p.ID == id);
                    ProdutoSubEmbalagem = await _context.Tbl_ProdutoSubEmbalagem.FirstOrDefaultAsync(p => p.ID == id);
                }
                return Page(); // Return the page with the original data
            }

            // Check for unique EAN13 in Tbl_ProdutoVendaCompra
            if (!string.IsNullOrEmpty(ProdutoVendaCompra.EAN13))
            {
                var existingProdutoVendaCompra = await _context.Tbl_ProdutoVendaCompra
                    .FirstOrDefaultAsync(pvc => pvc.EAN13 == ProdutoVendaCompra.EAN13);

                if (existingProdutoVendaCompra != null && existingProdutoVendaCompra.ID != Produto.ID)
                {
                    ModelState.AddModelError("ProdutoVendaCompra.EAN13", "EAN já Cadastrado para outro produto");
                    return Page();
                }
            }

            // Check for unique EAN13 in Tbl_ProdutoSubEmbalagem
            if (!string.IsNullOrEmpty(ProdutoSubEmbalagem.EAN13))
            {
                var existingProdutoSubEmbalagem = await _context.Tbl_ProdutoSubEmbalagem
                    .FirstOrDefaultAsync(pse => pse.EAN13 == ProdutoSubEmbalagem.EAN13);

                if (existingProdutoSubEmbalagem != null && existingProdutoSubEmbalagem.ID != Produto.ID)
                {
                    ModelState.AddModelError("ProdutoSubEmbalagem.EAN13", "EAN já Cadastrado para outro produto");
                    return Page();
                }
            }

            if (Request.Form["Produto.MARCA"] == "Nova Marca")
            {
                Produto.MARCA = Request.Form["novaMarcaInput"];
            }

            if (Produto.ID == 0)
            {
                // New product: save product first to generate ID
                Produto.CNPJ = cnpjClaim;
                _context.Tbl_Produto.Add(Produto);
                await _context.SaveChangesAsync();

                // After saving, Produto.ID is now available
                ProdutoVendaCompra.ID = Produto.ID;
                ProdutoSubEmbalagem.ID = Produto.ID;

                _context.Tbl_ProdutoVendaCompra.Add(ProdutoVendaCompra);
                _context.Tbl_ProdutoSubEmbalagem.Add(ProdutoSubEmbalagem);

                // Save the comment if provided
                if (!string.IsNullOrWhiteSpace(Request.Form["ProdutoComentario.COMENTARIO"]))
                {
                    ProdutoComentario = new Tbl_ProdutoComentarios
                    {
                        PRODUTOID = Produto.ID,
                        ID_USUARIO = int.Parse(User.Claims.FirstOrDefault(c => c.Type == "ID_Usuario")?.Value ?? "0"),
                        COMENTARIO = Request.Form["ProdutoComentario.COMENTARIO"],
                        DATACOMENTARIO = DateTime.Now
                    };

                    _context.Tbl_ProdutoComentarios.Add(ProdutoComentario);
                }
            }
            else
            {
                // Existing product: update
                var produtoToUpdate = await _context.Tbl_Produto.FirstOrDefaultAsync(p => p.ID == Produto.ID && p.CNPJ == cnpjClaim);
                var produtoVendaCompraToUpdate = await _context.Tbl_ProdutoVendaCompra.FirstOrDefaultAsync(pvc => pvc.ID == Produto.ID);
                var produtoSubEmbalagemToUpdate = await _context.Tbl_ProdutoSubEmbalagem.FirstOrDefaultAsync(pse => pse.ID == Produto.ID);

                if (produtoToUpdate == null || produtoVendaCompraToUpdate == null || produtoSubEmbalagemToUpdate == null)
                {
                    return NotFound();
                }

                // Update the product details
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
                produtoToUpdate.DESCRICAOCOMPLETA = Produto.DESCRICAOCOMPLETA;

                _context.Attach(produtoToUpdate).State = EntityState.Modified;

                // Update related entities
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

                produtoSubEmbalagemToUpdate.EAN13 = ProdutoSubEmbalagem.EAN13;
                produtoSubEmbalagemToUpdate.REFERENCIA = ProdutoSubEmbalagem.REFERENCIA;
                produtoSubEmbalagemToUpdate.PESOBRUTOKG = ProdutoSubEmbalagem.PESOBRUTOKG;
                produtoSubEmbalagemToUpdate.PESOLIQUIDOKG = ProdutoSubEmbalagem.PESOLIQUIDOKG;
                produtoSubEmbalagemToUpdate.ALTURACM = ProdutoSubEmbalagem.ALTURACM;
                produtoSubEmbalagemToUpdate.LARGURACM = ProdutoSubEmbalagem.LARGURACM;
                produtoSubEmbalagemToUpdate.PROFUNDIDADECM = ProdutoSubEmbalagem.PROFUNDIDADECM;
                produtoSubEmbalagemToUpdate.EMBALAGEM = ProdutoSubEmbalagem.EMBALAGEM;
                produtoSubEmbalagemToUpdate.QUANTIDADEUNIDADES = ProdutoSubEmbalagem.QUANTIDADEUNIDADES;

                _context.Attach(produtoSubEmbalagemToUpdate).State = EntityState.Modified;

                // Save the comment if provided
                if (!string.IsNullOrWhiteSpace(Request.Form["ProdutoComentario.COMENTARIO"]))
                {
                    ProdutoComentario = new Tbl_ProdutoComentarios
                    {
                        PRODUTOID = Produto.ID,
                        ID_USUARIO = int.Parse(User.Claims.FirstOrDefault(c => c.Type == "ID_Usuario")?.Value ?? "0"),
                        COMENTARIO = Request.Form["ProdutoComentario.COMENTARIO"],
                        DATACOMENTARIO = DateTime.Now
                    };

                    _context.Tbl_ProdutoComentarios.Add(ProdutoComentario);
                }
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

            return RedirectToPage("/Index");
        }

        private bool ProdutoExists(int id)
        {
            return _context.Tbl_Produto.Any(e => e.ID == id);
        }
    }
}