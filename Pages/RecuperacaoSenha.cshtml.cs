using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;

namespace PortalArcomix.Pages
{
    public class RecuperacaoSenhaModel : PageModel
    {
        private readonly IConfiguration _configuration;

        public RecuperacaoSenhaModel(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [BindProperty]
        public string Email { get; set; } = string.Empty;

        public string ErrorMessage { get; set; } = string.Empty;

        public bool IsSuccess { get; set; } = false;

        public void OnGet()
        {
            ViewData["HideNavbarAndFooter"] = true;
        }

        public async Task<IActionResult> OnPostAsync()
        {
            ViewData["HideNavbarAndFooter"] = true;

            string? connectionString = _configuration.GetConnectionString("OracleDbContext");

            if (string.IsNullOrEmpty(connectionString))
            {
                ErrorMessage = "Connection string is null or empty.";
                IsSuccess = false;
                return Page();
            }

            if (!EmailExists(Email, connectionString))
            {
                ErrorMessage = "Email não encontrado.";
                IsSuccess = false;
                return Page();
            }
            else
            {
                var userDetails = GetUserDetailsByEmail(Email, connectionString, out string tempPassword);
                if (userDetails != null)
                {
                    await SendEmailWithPassword(Email, tempPassword, userDetails.Item2);
                    ErrorMessage = "Recuperação de senha enviada para o seu email.";
                    IsSuccess = true;
                    return Page();
                }
                else
                {
                    ErrorMessage = "No user found with that email address.";
                    IsSuccess = false;
                    return Page();
                }
            }
        }

        private bool EmailExists(string email, string connectionString)
        {
            using (var con = new OracleConnection(connectionString))
            {
                con.Open();
                using (var cmd = new OracleCommand("SELECT COUNT(*) FROM TBL_USUARIO WHERE Email = :Email", con))
                {
                    cmd.Parameters.Add(new OracleParameter("Email", email));
                    int userCount = Convert.ToInt32(cmd.ExecuteScalar());
                    return userCount > 0;
                }
            }
        }

        private Tuple<string, string>? GetUserDetailsByEmail(string email, string connectionString, out string tempPassword)
        {
            tempPassword = GenerateValidPassword();
            using (var con = new OracleConnection(connectionString))
            {
                con.Open();
                using (var cmd = new OracleCommand("SELECT Usuario FROM TBL_USUARIO WHERE Email = :Email", con))
                {
                    cmd.Parameters.Add(new OracleParameter("Email", email));
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            string username = reader["Usuario"].ToString() ?? string.Empty;

                            // Update the password to the temporary password
                            UpdatePassword(email, tempPassword, connectionString);

                            return new Tuple<string, string>(tempPassword, username);
                        }
                    }
                }
            }
            return null;
        }

        private void UpdatePassword(string email, string tempPassword, string connectionString)
        {
            using (var con = new OracleConnection(connectionString))
            {
                con.Open();
                using (var cmd = new OracleCommand("UPDATE TBL_USUARIO SET Senha = :Senha WHERE Email = :Email", con))
                {
                    cmd.Parameters.Add(new OracleParameter("Senha", tempPassword));
                    cmd.Parameters.Add(new OracleParameter("Email", email));
                    cmd.ExecuteNonQuery();
                }
            }
        }

        private string GenerateValidPassword()
        {
            var random = new Random();
            string password = string.Empty;
            password += "ABCDEFGHIJKLMNOPQRSTUVWXYZ"[random.Next(26)];
            password += "0123456789"[random.Next(10)];
            string validChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            for (int i = 2; i < 8; i++)
            {
                password += validChars[random.Next(validChars.Length)];
            }
            password = new string(password.OrderBy(x => Guid.NewGuid()).ToArray());
            return password;
        }

        private async Task SendEmailWithPassword(string toEmail, string tempPassword, string username)
        {
            try
            {
                MailMessage mail = new MailMessage
                {
                    From = new MailAddress("gutemberg@hgstech.com.br"),
                    Subject = "Sua senha temporária de acesso ao Portal Arco-mix",
                    Body = $"Olá {username}, \n\nConforme solicitado, estamos enviando uma senha temporária de acesso ao sistema. Aqui está sua senha temporária: {tempPassword}\n\nPor motivos de segurança, recomendamos que você altere sua senha imediatamente após o acesso. \n\nSe você não solicitou uma senha temporária, por favor, ignore este e-mail ou notifique-nos imediatamente.\n\nAtenciosamente,\nEquipe de Suporte do Portal Arco-mix",
                    IsBodyHtml = false
                };

                mail.To.Add(toEmail);

                SmtpClient smtpServer = new SmtpClient("smtp.hostinger.com", 587)
                {
                    EnableSsl = true,
                    Credentials = new System.Net.NetworkCredential("gutemberg@hgstech.com.br", "HGs@8788")
                };

                await smtpServer.SendMailAsync(mail);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error sending email: {ex.Message}");
            }
        }
    }
}
