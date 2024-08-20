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
        public Tbl_ProdutoComentarios ProdutoComentario { get; set; }  // Binding for comments

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

            if (id == null)
            {
                Produto = new Tbl_Produto();
                ProdutoVendaCompra = new Tbl_ProdutoVendaCompra();
                ProdutoSubEmbalagem = new Tbl_ProdutoSubEmbalagem();
            }
            else
            {
                Produto = await _context.Tbl_Produto.FirstOrDefaultAsync(p => p.ID == id);
                ProdutoVendaCompra = await _context.Tbl_ProdutoVendaCompra.FirstOrDefaultAsync(p => p.ID == id);
                ProdutoSubEmbalagem = await _context.Tbl_ProdutoSubEmbalagem.FirstOrDefaultAsync(p => p.ID == id);

                if (Produto == null)
                {
                    return NotFound();
                }

                if (ProdutoVendaCompra == null)
                {
                    ProdutoVendaCompra = new Tbl_ProdutoVendaCompra { PRODUTOID = Produto.ID };
                }

                if (ProdutoSubEmbalagem == null)
                {
                    ProdutoSubEmbalagem = new Tbl_ProdutoSubEmbalagem { PRODUTOID = Produto.ID };
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

            if (!string.IsNullOrEmpty(ProdutoVendaCompra.EAN13) &&
                await _context.Tbl_ProdutoVendaCompra.AnyAsync(pvc => pvc.EAN13 == ProdutoVendaCompra.EAN13))
            {
                ModelState.AddModelError("ProdutoVendaCompra.EAN13", "EAN já Cadastrado");
                return Page();
            }

            if (!string.IsNullOrEmpty(ProdutoSubEmbalagem.EAN13) &&
                await _context.Tbl_ProdutoSubEmbalagem.AnyAsync(pse => pse.EAN13 == ProdutoSubEmbalagem.EAN13))
            {
                ModelState.AddModelError("ProdutoSubEmbalagem.EAN13", "EAN já Cadastrado");
                return Page();
            }

            if (Request.Form["Produto.MARCA"] == "Nova Marca")
            {
                Produto.MARCA = Request.Form["novaMarcaInput"];
            }

            if (Produto.ID == 0)
            {
                Produto.CNPJ = cnpjClaim;
                _context.Tbl_Produto.Add(Produto);
                await _context.SaveChangesAsync();

                ProdutoVendaCompra.ID = Produto.ID;
                ProdutoSubEmbalagem.ID = Produto.ID;

                _context.Tbl_ProdutoVendaCompra.Add(ProdutoVendaCompra);
                _context.Tbl_ProdutoSubEmbalagem.Add(ProdutoSubEmbalagem);
            }
            else
            {
                var produtoToUpdate = await _context.Tbl_Produto.FirstOrDefaultAsync(p => p.ID == Produto.ID && p.CNPJ == cnpjClaim);
                var produtoVendaCompraToUpdate = await _context.Tbl_ProdutoVendaCompra.FirstOrDefaultAsync(pvc => pvc.ID == Produto.ID);
                var produtoSubEmbalagemToUpdate = await _context.Tbl_ProdutoSubEmbalagem.FirstOrDefaultAsync(pse => pse.ID == Produto.ID);

                if (produtoToUpdate == null || produtoVendaCompraToUpdate == null || produtoSubEmbalagemToUpdate == null)
                {
                    return NotFound();
                }

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
            }

            // Save the comment if it's provided
            if (!string.IsNullOrWhiteSpace(ProdutoComentario?.COMENTARIO))
            {
                ProdutoComentario.PRODUTOID = Produto.ID;
                ProdutoComentario.ID_USUARIO = int.Parse(User.Claims.FirstOrDefault(c => c.Type == "ID_Usuario")?.Value ?? "0");
                ProdutoComentario.DATACOMENTARIO = DateTime.Now;

                _context.Tbl_ProdutoComentarios.Add(ProdutoComentario);
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