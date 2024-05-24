using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using System;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace PortalArcomix.Pages
{
    public class MeusDadosModel : PageModel
    {
        private readonly IConfiguration _configuration;

        public MeusDadosModel(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [BindProperty]
        public string OldPassword { get; set; } = string.Empty;

        [BindProperty]
        public string NewPassword { get; set; } = string.Empty;

        [BindProperty]
        public string ConfirmPassword { get; set; } = string.Empty;

        public string ErrorMessage { get; set; } = string.Empty;
        public string SuccessMessage { get; set; } = string.Empty;

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
            if (HttpContext.User.Identity == null || !HttpContext.User.Identity.IsAuthenticated)
            {
                return RedirectToPage("/Login");
            }

            string userEmail = HttpContext.User.Identity.Name ?? string.Empty;

            if (NewPassword != ConfirmPassword)
            {
                ErrorMessage = "As senhas digitadas são diferentes";
                return Page();
            }

            if (!IsValidPassword(NewPassword))
            {
                ErrorMessage = "A Senha deve ter no minimo 8 caracteres, Letras Maiusculas, Minusculas e Numeros";
                return Page();
            }

            string connectionString = _configuration.GetConnectionString("PortalArcomixDB")!;

            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    await con.OpenAsync();
                    SqlCommand cmd = new SqlCommand("SELECT Senha FROM Tbl_Usuario WHERE Email=@Email", con);
                    cmd.Parameters.AddWithValue("@Email", userEmail);
                    var result = await cmd.ExecuteScalarAsync();
                    string existingPassword = result?.ToString() ?? string.Empty;

                    if (existingPassword == OldPassword)
                    {
                        SqlCommand updateCmd = new SqlCommand("UPDATE Tbl_Usuario SET Senha=@NewPassword WHERE Email=@Email", con);
                        updateCmd.Parameters.AddWithValue("@NewPassword", NewPassword);
                        updateCmd.Parameters.AddWithValue("@Email", userEmail);
                        await updateCmd.ExecuteNonQueryAsync();

                        SuccessMessage = "Senha Alterada com sucesso";
                    }
                    else
                    {
                        ErrorMessage = "Senha Atual Incorreta";
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Erro ao atualizar a senha: {ex.Message}";
            }

            return Page();
        }

        private bool IsValidPassword(string password)
        {
            if (password.Length < 8) return false;
            if (!password.Any(char.IsUpper)) return false;
            if (!(password.Any(char.IsLetter) && password.Any(char.IsDigit))) return false;

            return true;
        }
    }
}
