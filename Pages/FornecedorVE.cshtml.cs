using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PortalArcomix.Data;
using PortalArcomix.Data.Entities;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace PortalArcomix.Pages
{
    public class FornecedorVEModel : PageModel
    {
        private readonly OracleDbContext _context;

        public FornecedorVEModel(OracleDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Tbl_Fornecedor Fornecedor { get; set; }

        [BindProperty]
        public Tbl_FornecedorDadosBancarios DadosBancarios { get; set; }

        [BindProperty]
        public Tbl_FornecedorContatos Contatos { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            // Check if the user is authenticated
            if (User?.Identity?.IsAuthenticated != true)
            {
                return RedirectToPage("/Login");
            }

            // Retrieve the CNPJ claim from the authenticated user
            var cnpjClaim = User.Claims.FirstOrDefault(c => c.Type == "CNPJ")?.Value;

            if (cnpjClaim == null)
            {
                return NotFound();
            }

            // Load the Fornecedor data from the database
            Fornecedor = await _context.Tbl_Fornecedor.FirstOrDefaultAsync(f => f.CNPJ == cnpjClaim);

            if (Fornecedor == null)
            {
                return NotFound();
            }

            // Load the DadosBancarios data from the database
            DadosBancarios = await _context.Tbl_FornecedorDadosBancarios.FirstOrDefaultAsync(db => db.CNPJ == cnpjClaim);

            if (DadosBancarios == null)
            {
                DadosBancarios = new Tbl_FornecedorDadosBancarios { CNPJ = cnpjClaim };
                _context.Tbl_FornecedorDadosBancarios.Add(DadosBancarios);
                await _context.SaveChangesAsync();
            }

            // Load the Contatos data from the database
            Contatos = await _context.Tbl_FornecedorContatos.FirstOrDefaultAsync(c => c.CNPJ == cnpjClaim);

            if (Contatos == null)
            {
                Contatos = new Tbl_FornecedorContatos { CNPJ = cnpjClaim };
                _context.Tbl_FornecedorContatos.Add(Contatos);
                await _context.SaveChangesAsync();
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            // Check if the user is authenticated
            if (User?.Identity?.IsAuthenticated != true)
            {
                return RedirectToPage("/Login");
            }

            // Retrieve the CNPJ claim from the authenticated user
            var cnpjClaim = User.Claims.FirstOrDefault(c => c.Type == "CNPJ")?.Value;

            if (cnpjClaim == null || cnpjClaim != Fornecedor.CNPJ)
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                return Page();
            }

            // Email validation
            ValidateEmailField(Contatos.EMAILVENDEDOR, nameof(Contatos.EMAILVENDEDOR));
            ValidateEmailField(Contatos.EMAILGERENTE, nameof(Contatos.EMAILGERENTE));
            ValidateEmailField(Contatos.EMAILRESPFINANCEIRO, nameof(Contatos.EMAILRESPFINANCEIRO));

            if (!ModelState.IsValid)
            {
                return Page();
            }

            var fornecedorToUpdate = await _context.Tbl_Fornecedor.FirstOrDefaultAsync(f => f.CNPJ == cnpjClaim);
            if (fornecedorToUpdate == null)
            {
                return NotFound();
            }

            var dadosBancariosToUpdate = await _context.Tbl_FornecedorDadosBancarios.FirstOrDefaultAsync(db => db.CNPJ == cnpjClaim);
            if (dadosBancariosToUpdate == null)
            {
                return NotFound();
            }

            var contatosToUpdate = await _context.Tbl_FornecedorContatos.FirstOrDefaultAsync(c => c.CNPJ == cnpjClaim);
            if (contatosToUpdate == null)
            {
                return NotFound();
            }

            // Update the Fornecedor properties
            fornecedorToUpdate.RAZAOSOCIAL = Fornecedor.RAZAOSOCIAL;
            fornecedorToUpdate.FANTASIA = Fornecedor.FANTASIA;
            fornecedorToUpdate.IE = Fornecedor.IE;
            fornecedorToUpdate.CNAE = Fornecedor.CNAE;
            fornecedorToUpdate.CEP = Fornecedor.CEP;
            fornecedorToUpdate.LOGRADOURO = Fornecedor.LOGRADOURO;
            fornecedorToUpdate.NUMEROENDERECO = Fornecedor.NUMEROENDERECO;
            fornecedorToUpdate.BAIRRO = Fornecedor.BAIRRO;
            fornecedorToUpdate.COMPLEMENTO = Fornecedor.COMPLEMENTO;
            fornecedorToUpdate.UF = Fornecedor.UF;
            fornecedorToUpdate.CIDADE = Fornecedor.CIDADE;
            fornecedorToUpdate.MICROEMPRESA = Fornecedor.MICROEMPRESA;
            fornecedorToUpdate.CONTRIBUIICMS = Fornecedor.CONTRIBUIICMS;
            fornecedorToUpdate.CONTRIBUIIPI = Fornecedor.CONTRIBUIIPI;
            fornecedorToUpdate.TIPOFORNECEDOR = Fornecedor.TIPOFORNECEDOR;
            fornecedorToUpdate.FORNECEDORALIMENTOS = Fornecedor.FORNECEDORALIMENTOS;
            fornecedorToUpdate.COMPRADORPRINCIPAL = Fornecedor.COMPRADORPRINCIPAL;

            // Update the DadosBancarios properties
            dadosBancariosToUpdate.BANCO = DadosBancarios.BANCO;
            dadosBancariosToUpdate.AGENCIA = DadosBancarios.AGENCIA;
            dadosBancariosToUpdate.TIPOCONTA = DadosBancarios.TIPOCONTA;
            dadosBancariosToUpdate.NUMEROCONTA = DadosBancarios.NUMEROCONTA;
            dadosBancariosToUpdate.CNPJCONTATITULAR = DadosBancarios.CNPJCONTATITULAR;

            // Update the Contatos properties
            contatosToUpdate.CONTATOVENDEDOR = Contatos.CONTATOVENDEDOR;
            contatosToUpdate.DDDVENDEDOR = Contatos.DDDVENDEDOR;
            contatosToUpdate.TELEFONEVENDEDOR = Contatos.TELEFONEVENDEDOR;
            contatosToUpdate.EMAILVENDEDOR = Contatos.EMAILVENDEDOR;
            contatosToUpdate.CONTATOGERENTE = Contatos.CONTATOGERENTE;
            contatosToUpdate.DDDGERENTE = Contatos.DDDGERENTE;
            contatosToUpdate.TELEFONEGERENTE = Contatos.TELEFONEGERENTE;
            contatosToUpdate.EMAILGERENTE = Contatos.EMAILGERENTE;
            contatosToUpdate.RESPONSAVELFINANCEIRO = Contatos.RESPONSAVELFINANCEIRO;
            contatosToUpdate.DDDRESPFINANCEIRO = Contatos.DDDRESPFINANCEIRO;
            contatosToUpdate.TELEFONERESPFINANCEIRO = Contatos.TELEFONERESPFINANCEIRO;
            contatosToUpdate.EMAILRESPFINANCEIRO = Contatos.EMAILRESPFINANCEIRO;
            contatosToUpdate.DDDTELEFONEFIXOEMPRESA = Contatos.DDDTELEFONEFIXOEMPRESA;
            contatosToUpdate.TELEFONEFIXOEMPRESA = Contatos.TELEFONEFIXOEMPRESA;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!FornecedorExists(Fornecedor.CNPJ))
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

        private void ValidateEmailField(string email, string fieldName)
        {
            if (!IsValidEmail(email))
            {
                ModelState.AddModelError(fieldName, "Email inválido.");
            }
        }

        private bool FornecedorExists(string cnpj)
        {
            return _context.Tbl_Fornecedor.Any(e => e.CNPJ == cnpj);
        }

        private bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }
    }
}

