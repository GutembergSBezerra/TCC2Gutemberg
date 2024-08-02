using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PortalArcomix.Data;
using PortalArcomix.Data.Entities;
using System.Linq;
using System.Threading.Tasks;

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

        [BindProperty]
        public Tbl_FornecedorSegurancaAlimentos SegurancaAlimentos { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            if (User?.Identity?.IsAuthenticated != true)
            {
                return RedirectToPage("/Login");
            }

            var cnpjClaim = User.Claims.FirstOrDefault(c => c.Type == "CNPJ")?.Value;

            if (cnpjClaim == null)
            {
                return NotFound();
            }

            Fornecedor = await _context.Tbl_Fornecedor.FirstOrDefaultAsync(f => f.CNPJ == cnpjClaim);

            if (Fornecedor == null)
            {
                return NotFound();
            }

            DadosBancarios = await _context.Tbl_FornecedorDadosBancarios.FirstOrDefaultAsync(db => db.CNPJ == cnpjClaim);
            Contatos = await _context.Tbl_FornecedorContatos.FirstOrDefaultAsync(c => c.CNPJ == cnpjClaim);
            SegurancaAlimentos = await _context.Tbl_FornecedorSegurancaAlimentos.FirstOrDefaultAsync(sa => sa.CNPJ == cnpjClaim);

            bool isNew = false;

            if (DadosBancarios == null)
            {
                DadosBancarios = new Tbl_FornecedorDadosBancarios { CNPJ = cnpjClaim };
                _context.Tbl_FornecedorDadosBancarios.Add(DadosBancarios);
                isNew = true;
            }

            if (Contatos == null)
            {
                Contatos = new Tbl_FornecedorContatos { CNPJ = cnpjClaim };
                _context.Tbl_FornecedorContatos.Add(Contatos);
                isNew = true;
            }

            if (SegurancaAlimentos == null)
            {
                SegurancaAlimentos = new Tbl_FornecedorSegurancaAlimentos { CNPJ = cnpjClaim };
                _context.Tbl_FornecedorSegurancaAlimentos.Add(SegurancaAlimentos);
                isNew = true;
            }

            if (isNew)
            {
                await _context.SaveChangesAsync();
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

            if (cnpjClaim == null || cnpjClaim != Fornecedor.CNPJ)
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                return Page();
            }

            var fornecedorToUpdate = await _context.Tbl_Fornecedor.FirstOrDefaultAsync(f => f.CNPJ == cnpjClaim);
            var dadosBancariosToUpdate = await _context.Tbl_FornecedorDadosBancarios.FirstOrDefaultAsync(db => db.CNPJ == cnpjClaim);
            var contatosToUpdate = await _context.Tbl_FornecedorContatos.FirstOrDefaultAsync(c => c.CNPJ == cnpjClaim);
            var segurancaAlimentosToUpdate = await _context.Tbl_FornecedorSegurancaAlimentos.FirstOrDefaultAsync(sa => sa.CNPJ == cnpjClaim);

            if (fornecedorToUpdate == null || dadosBancariosToUpdate == null || contatosToUpdate == null || segurancaAlimentosToUpdate == null)
            {
                return NotFound();
            }

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

            dadosBancariosToUpdate.BANCO = DadosBancarios.BANCO;
            dadosBancariosToUpdate.AGENCIA = DadosBancarios.AGENCIA;
            dadosBancariosToUpdate.TIPOCONTA = DadosBancarios.TIPOCONTA;
            dadosBancariosToUpdate.NUMEROCONTA = DadosBancarios.NUMEROCONTA;
            dadosBancariosToUpdate.CNPJCONTATITULAR = DadosBancarios.CNPJCONTATITULAR;

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

            segurancaAlimentosToUpdate.PARECER = SegurancaAlimentos.PARECER;
            segurancaAlimentosToUpdate.DESCRICAOAVALIACAO = SegurancaAlimentos.DESCRICAOAVALIACAO;
            segurancaAlimentosToUpdate.ATIVIDADEEMPRESA = SegurancaAlimentos.ATIVIDADEEMPRESA;
            segurancaAlimentosToUpdate.ORGAOFISCALIZACAO = SegurancaAlimentos.ORGAOFISCALIZACAO;
            segurancaAlimentosToUpdate.NUMEROREGISTROORGAO = SegurancaAlimentos.NUMEROREGISTROORGAO;
            segurancaAlimentosToUpdate.RESPONSAVELTECNICO = SegurancaAlimentos.RESPONSAVELTECNICO;
            segurancaAlimentosToUpdate.DDDTECNICO = SegurancaAlimentos.DDDTECNICO;
            segurancaAlimentosToUpdate.TELEFONETECNICO = SegurancaAlimentos.TELEFONETECNICO;
            segurancaAlimentosToUpdate.EMAILTECNICO = SegurancaAlimentos.EMAILTECNICO;
            segurancaAlimentosToUpdate.REGRESPTECNICO = SegurancaAlimentos.REGRESPTECNICO;

            segurancaAlimentosToUpdate.BPFIMPLANTADOPRATICADO = SegurancaAlimentos.BPFIMPLANTADOPRATICADO;
            segurancaAlimentosToUpdate.SISTEMATRATAMENTORESIDUOS = SegurancaAlimentos.SISTEMATRATAMENTORESIDUOS;
            segurancaAlimentosToUpdate.CONTROLESAUDECOLABORADORES = SegurancaAlimentos.CONTROLESAUDECOLABORADORES;
            segurancaAlimentosToUpdate.CIPMIPIMPLEMENTADO = SegurancaAlimentos.CIPMIPIMPLEMENTADO;
            segurancaAlimentosToUpdate.SISTEMAIDENTIFICACAORASTREABILIDADE = SegurancaAlimentos.SISTEMAIDENTIFICACAORASTREABILIDADE;
            segurancaAlimentosToUpdate.PROCEDIMENTOPADRAOFABRICACAO = SegurancaAlimentos.PROCEDIMENTOPADRAOFABRICACAO;
            segurancaAlimentosToUpdate.CONTROLEQUALIDADEIMPLEMENTADO = SegurancaAlimentos.CONTROLEQUALIDADEIMPLEMENTADO;
            segurancaAlimentosToUpdate.PROCEDIMENTOPADRAORECEBIMENTO = SegurancaAlimentos.PROCEDIMENTOPADRAORECEBIMENTO;
            segurancaAlimentosToUpdate.SISTEMATRATAMENTORECLAMACOES = SegurancaAlimentos.SISTEMATRATAMENTORECLAMACOES;
            segurancaAlimentosToUpdate.TRANSPORTEPROPRIO = SegurancaAlimentos.TRANSPORTEPROPRIO;
            segurancaAlimentosToUpdate.FICHATECNICAPRODUTOS = SegurancaAlimentos.FICHATECNICAPRODUTOS;
            segurancaAlimentosToUpdate.CONTROLEPRODUTOSNAOCONFORME = SegurancaAlimentos.CONTROLEPRODUTOSNAOCONFORME;
            segurancaAlimentosToUpdate.EXIGEHIGIENIZACAOCIP = SegurancaAlimentos.EXIGEHIGIENIZACAOCIP;
            segurancaAlimentosToUpdate.REGISTROSCOMPROVACAOSISTEMAS = SegurancaAlimentos.REGISTROSCOMPROVACAOSISTEMAS;
            segurancaAlimentosToUpdate.LICENCASPERTINENTES = SegurancaAlimentos.LICENCASPERTINENTES;
            segurancaAlimentosToUpdate.ENVIAAMOSTRASCLIENTE = SegurancaAlimentos.ENVIAAMOSTRASCLIENTE;
            segurancaAlimentosToUpdate.CONTROLEAGUAABASTECIMENTO = SegurancaAlimentos.CONTROLEAGUAABASTECIMENTO;


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

        private bool FornecedorExists(string cnpj)
        {
            return _context.Tbl_Fornecedor.Any(e => e.CNPJ == cnpj);
        }
    }
}


