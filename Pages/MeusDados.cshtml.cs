using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using PortalArcomix.Data;
using PortalArcomix.Data.Entities;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace PortalArcomix.Pages
{
    public class MeusDadosModel : PageModel
    {
        private readonly IConfiguration _configuration;
        private readonly OracleDbContext _context;
        private const string IncorrectPasswordAttemptsSessionKey = "IncorrectPasswordAttempts";

        public MeusDadosModel(IConfiguration configuration, OracleDbContext context)
        {
            _configuration = configuration;
            _context = context;
        }

        [BindProperty]
        public string OldPassword { get; set; } = string.Empty;

        [BindProperty]
        public string NewPassword { get; set; } = string.Empty;

        [BindProperty]
        public string ConfirmPassword { get; set; } = string.Empty;

        [BindProperty]
        public string DeletePassword { get; set; } = string.Empty;

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

        public async Task<IActionResult> OnPostAsync(string action)
        {
            if (HttpContext.User.Identity == null || !HttpContext.User.Identity.IsAuthenticated)
            {
                return RedirectToPage("/Login");
            }

            string userEmail = HttpContext.User.Identity.Name ?? string.Empty;

            if (action == "Salvar")
            {
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

                try
                {
                    var user = _context.Tbl_Usuario.FirstOrDefault(u => u.Email == userEmail);

                    if (user != null && user.Senha == OldPassword)
                    {
                        user.Senha = NewPassword;
                        _context.Tbl_Usuario.Update(user);
                        await _context.SaveChangesAsync();

                        SuccessMessage = "Senha Alterada com sucesso";
                        HttpContext.Session.Remove(IncorrectPasswordAttemptsSessionKey); // Reset the attempt count on success
                    }
                    else
                    {
                        int incorrectAttempts = HttpContext.Session.GetInt32(IncorrectPasswordAttemptsSessionKey) ?? 0;
                        incorrectAttempts++;
                        HttpContext.Session.SetInt32(IncorrectPasswordAttemptsSessionKey, incorrectAttempts);

                        if (incorrectAttempts >= 3)
                        {
                            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                            return RedirectToPage("/Login");
                        }

                        ErrorMessage = $"Senha Atual Incorreta. Tentativas restantes: {3 - incorrectAttempts}";
                    }
                }
                catch (Exception ex)
                {
                    ErrorMessage = $"Erro ao atualizar a senha: {ex.Message}";
                }
            }
            else if (action == "Excluir")
            {
                try
                {
                    var user = _context.Tbl_Usuario.FirstOrDefault(u => u.Email == userEmail);

                    if (user != null && user.Senha == DeletePassword)
                    {
                        _context.Tbl_Usuario.Remove(user);
                        await _context.SaveChangesAsync();

                        SuccessMessage = "Conta excluída com sucesso";
                        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                        return RedirectToPage("/Login");
                    }
                    else
                    {
                        ErrorMessage = "Senha Atual Incorreta";
                    }
                }
                catch (Exception ex)
                {
                    ErrorMessage = $"Erro ao excluir a conta: {ex.Message}";
                }
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
