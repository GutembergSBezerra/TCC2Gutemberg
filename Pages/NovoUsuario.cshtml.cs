using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Mvc.Rendering;
using PortalArcomix.Data;
using PortalArcomix.Data.Entities;
using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace PortalArcomix.Pages
{
    public class NovoUsuarioModel : PageModel
    {
        private readonly IConfiguration _configuration;
        private readonly OracleDbContext _context;

        public NovoUsuarioModel(IConfiguration configuration, OracleDbContext context)
        {
            _configuration = configuration;
            _context = context;
        }

        [BindProperty]
        [Required(ErrorMessage = "O campo Email é obrigatório.")]
        [EmailAddress(ErrorMessage = "Email inválido.")]
        public string Email { get; set; } = string.Empty;

        [BindProperty]
        [Required(ErrorMessage = "O campo Usuário é obrigatório.")]
        public string Usuario { get; set; } = string.Empty;

        [BindProperty]
        [Required(ErrorMessage = "O campo Tipo Usuario é obrigatório.")]
        public string TipoUsuario { get; set; } = string.Empty;

        [BindProperty]
        public string? CNPJ { get; set; } = string.Empty;

        public string ErrorMessage { get; set; } = string.Empty;
        public string SuccessMessage { get; set; } = string.Empty;

        public SelectList TipoUsuarioOptions { get; } = new SelectList(new[]
        {
            new { Value = "", Text = "" },
            new { Value = "Fornecedor", Text = "Fornecedor" },
            new { Value = "Comprador", Text = "Comprador" },
            new { Value = "Gerência", Text = "Gerência" },
            new { Value = "GC", Text = "GC" },
            new { Value = "Cadastro", Text = "Cadastro" },
            new { Value = "Controladoria", Text = "Controladoria" },
            new { Value = "Logistica", Text = "Logistica" }
        }, "Value", "Text");

        public IActionResult OnGet()
        {
            // Check if the user is authenticated
            if (User?.Identity?.IsAuthenticated != true)
            {
                // Redirect to the Login page if the user is not authenticated
                return RedirectToPage("/Login");
            }

            // If authenticated, continue with the normal page processing
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            if (await VerificarEmailAsync(Email))
            {
                ErrorMessage = "Email já cadastrado.";
                return Page();
            }

            if (TipoUsuario == "Fornecedor" && (CNPJ == null || CNPJ.Length != 14))
            {
                ErrorMessage = "O CNPJ deve conter exatamente 14 dígitos.";
                return Page();
            }

            try
            {
                string senha = GenerateValidPassword();

                var novoUsuario = new Tbl_Usuario
                {
                    Email = Email,
                    Senha = senha,
                    TipoUsuario = TipoUsuario,
                    Usuario = Usuario,
                    CNPJ = TipoUsuario == "Fornecedor" ? CNPJ : null
                };

                _context.Tbl_Usuario.Add(novoUsuario);
                await _context.SaveChangesAsync();

                SendEmail(Email, senha, Usuario);

                SuccessMessage = "Cadastro realizado com sucesso!";
                return Page();
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Erro ao cadastrar usuário: {ex.Message}";
                return Page();
            }
        }

        private async Task<bool> VerificarEmailAsync(string email)
        {
            try
            {
                return await _context.Tbl_Usuario.AnyAsync(u => u.Email == email);
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Erro ao verificar email: {ex.Message}";
                return false;
            }
        }

        private string GenerateValidPassword()
        {
            var random = new Random();
            string password;
            do
            {
                password = string.Empty;
                password += "ABCDEFGHIJKLMNOPQRSTUVWXYZ"[random.Next(26)];
                password += "0123456789"[random.Next(10)];
                string validChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
                for (int i = 2; i < 8; i++)
                {
                    password += validChars[random.Next(validChars.Length)];
                }
                password = new string(password.OrderBy(x => Guid.NewGuid()).ToArray());
            } while (!IsValidPassword(password));

            return password;
        }

        private bool IsValidPassword(string password)
        {
            if (password.Length < 8) return false;
            if (!password.Any(char.IsUpper)) return false;
            if (!(password.Any(char.IsLetter) && password.Any(char.IsDigit))) return false;
            return true;
        }

        private void SendEmail(string toEmail, string password, string usuario)
        {
            try
            {
                MailMessage mail = new MailMessage
                {
                    From = new MailAddress("gutemberg@hgstech.com.br"),
                    Subject = "Bem-vindo ao Portal Arco-mix! Sua senha de acesso temporária",
                    Body = $"Olá {usuario},\n\nÉ com prazer que damos as boas-vindas ao Portal do Arco-mix! Como parte do processo de configuração inicial, estamos enviando sua senha temporária para garantir seu acesso ao sistema.\n\nAqui está sua senha temporária: {password}\n\nPor favor, siga estas etapas para acessar o sistema pela primeira vez:\n\nAcesse [link do site].\nInsira seu endereço de e-mail e a senha temporária fornecida acima.\nVocê será solicitado a alterar sua senha na primeira vez que fizer login. Escolha uma senha forte e segura que você possa lembrar facilmente.\n\nSe você encontrar alguma dificuldade ou tiver dúvidas, sinta-se à vontade para entrar em contato com nossa equipe de suporte através do e-mail [email de suporte] ou do telefone [número de suporte]. Estamos aqui para ajudar!\n\nObrigado por fazer parte da nossa comunidade. Esperamos que você aproveite ao máximo todos os recursos e benefícios que o Portal do Arco-mix tem a oferecer.\n\nAtenciosamente,\nEquipe Arco-mix",
                    IsBodyHtml = false
                };
                mail.To.Add(toEmail);

                SmtpClient smtpServer = new SmtpClient("smtp.hostinger.com", 587)
                {
                    EnableSsl = true,
                    Credentials = new System.Net.NetworkCredential("gutemberg@hgstech.com.br", "HGs@8788")
                };

                smtpServer.Send(mail);
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Erro ao enviar e-mail: {ex.Message}";
            }
        }
    }
}


