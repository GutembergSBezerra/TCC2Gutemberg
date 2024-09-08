using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PortalArcomix.Data;
using PortalArcomix.Data.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TCC2Gutemberg.Pages
{
    public class EmpresasSelecionadasModel : PageModel
    {
        private readonly OracleDbContext _context;

        public EmpresasSelecionadasModel(OracleDbContext context)
        {
            _context = context;
        }

        // Lista de todas as empresas disponíveis
        public List<Tbl_Empresa> Empresas { get; set; } = new List<Tbl_Empresa>();

        // Lista das empresas selecionadas pelo usuário atual
        public List<int> EmpresasSelecionadas { get; set; } = new List<int>();

        public async Task<IActionResult> OnGetAsync()
        {
            // Carregar todas as empresas disponíveis
            Empresas = await _context.Tbl_Empresa.ToListAsync();

            // Obter o usuário logado
            var usuarioLogado = await _context.Tbl_Usuario
                .Include(u => u.UsuarioEmpresas)
                .FirstOrDefaultAsync(u => u.Email == User.Identity.Name);

            if (usuarioLogado != null)
            {
                EmpresasSelecionadas = usuarioLogado.UsuarioEmpresas
                    .Select(ue => ue.ID_Empresa)
                    .ToList();
            }

            return Page();
        }


        public async Task<IActionResult> OnPostAsync(List<int> empresasSelecionadas)
        {
            var usuarioLogado = await _context.Tbl_Usuario
                .Include(u => u.UsuarioEmpresas)
                .FirstOrDefaultAsync(u => u.Email == User.Identity.Name);

            if (usuarioLogado == null)
            {
                return RedirectToPage("/Login");
            }

            // Remover as associações atuais
            _context.Tbl_Usuario_Empresa.RemoveRange(usuarioLogado.UsuarioEmpresas);

            // Adicionar as novas associações
            foreach (var empresaId in empresasSelecionadas)
            {
                var usuarioEmpresa = new Tbl_Usuario_Empresa
                {
                    ID_Usuario = usuarioLogado.ID_Usuario,
                    ID_Empresa = empresaId
                };
                _context.Tbl_Usuario_Empresa.Add(usuarioEmpresa);
            }

            await _context.SaveChangesAsync();

            return RedirectToPage("/EmpresasSelecionadas");
        }


    }
}
