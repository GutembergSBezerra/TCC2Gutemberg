using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
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

        public string ErrorMessage { get; set; } = string.Empty;
        public string SuccessMessage { get; set; } = string.Empty;

        public IActionResult OnGet()
        {
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

            try
            {
                string senha = GenerateValidPassword();

                var novoUsuario = new Tbl_Usuario
                {
                    Email = Email,
                    Senha = senha,
                    Usuario = Usuario
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
                    Subject = "Bem-vindo ao Sistema de Fatos Relevantes Simplificados!",
                    Body = $"Olá {usuario},\n\nÉ com grande satisfação que damos as boas-vindas ao Sistema de Fatos Relevantes Simplificados! Nosso objetivo é facilitar o acesso e a compreensão dos fatos relevantes do mercado de ações, tornando as informações financeiras mais acessíveis para novos investidores.\n\nPara começar, aqui está sua senha temporária: {password}\n\nSiga estas etapas para acessar o sistema pela primeira vez:\n\n1. Acesse [link do site].\n2. Insira seu e-mail e a senha temporária fornecida acima.\n3. Você será solicitado a alterar sua senha no primeiro acesso. Recomendamos que escolha uma senha forte e segura.\n\nCaso tenha dúvidas ou enfrente dificuldades, entre em contato com nossa equipe de suporte através do e-mail [email de suporte] ou pelo telefone [número de suporte]. Estamos aqui para ajudar você a explorar todas as funcionalidades do sistema e a tomar decisões de investimento mais informadas.\n\nObrigado por fazer parte da nossa plataforma! Esperamos que você aproveite todos os recursos que oferecemos para melhorar sua jornada como investidor.\n\nAtenciosamente,\nEquipe do Sistema de Fatos Relevantes Simplificados",
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