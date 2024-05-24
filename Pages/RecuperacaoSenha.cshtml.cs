using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using System;
using System.Data;
using System.Data.SqlClient;
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

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            string? connectionString = _configuration.GetConnectionString("PortalArcomixDB");

            if (string.IsNullOrEmpty(connectionString))
            {
                ErrorMessage = "Connection string is null or empty.";
                return Page();
            }

            if (!EmailExists(Email, connectionString))
            {
                ErrorMessage = "Email não encontrado.";
                return Page();
            }
            else
            {
                var userDetails = GetUserDetailsByEmail(Email, connectionString);
                if (userDetails != null)
                {
                    await SendEmailWithPassword(Email, userDetails.Item1, userDetails.Item2);
                    ErrorMessage = "Recuperação de senha enviada pro seu email";
                    return Page();
                }
                else
                {
                    ErrorMessage = "No user found with that email address.";
                    return Page();
                }
            }
        }

        private bool EmailExists(string email, string connectionString)
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();
                using (SqlCommand cmd = new SqlCommand("SELECT COUNT(*) FROM Tbl_Usuario WHERE Email = @Email", con))
                {
                    cmd.Parameters.AddWithValue("@Email", email);
                    int userCount = Convert.ToInt32(cmd.ExecuteScalar());
                    return userCount > 0;
                }
            }
        }

        private Tuple<string, string>? GetUserDetailsByEmail(string email, string connectionString)
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();
                using (SqlCommand cmd = new SqlCommand("SELECT Senha, Usuario FROM Tbl_Usuario WHERE Email=@Email", con))
                {
                    cmd.Parameters.AddWithValue("@Email", email);
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    if (dt.Rows.Count > 0)
                    {
                        string password = dt.Rows[0]["Senha"].ToString() ?? string.Empty;
                        string username = dt.Rows[0]["Usuario"].ToString() ?? string.Empty;
                        return new Tuple<string, string>(password, username);
                    }
                }
            }
            return null;
        }

        private async Task SendEmailWithPassword(string toEmail, string password, string username)
        {
            try
            {
                MailMessage mail = new MailMessage
                {
                    From = new MailAddress("gutemberg@hgstech.com.br"),
                    Subject = "Sua senha de acesso ao Portal Arco-mix",
                    Body = $"Olá {username}, \n\nConforme solicitado, estamos enviando sua senha atual de acesso ao sistema. Aqui está sua senha: {password}\n\nRecomendamos que por motivos de segurança, você altere sua senha imediatamente após o acesso. \n\nSe você não solicitou sua senha, por favor, ignore este e-mail ou notifique-nos imediatamente.\n\nAtenciosamente,\nEquipe de Suporte do Portal Arco-mix",
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
