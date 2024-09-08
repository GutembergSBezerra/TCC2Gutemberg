using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using PortalArcomix.Data;
using PortalArcomix.Data.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace PortalArcomix.Pages
{
    public class LoginModel : PageModel
    {
        private readonly IConfiguration _configuration;
        private readonly OracleDbContext _context;

        public LoginModel(IConfiguration configuration, OracleDbContext context)
        {
            _configuration = configuration;
            _context = context;
        }

        [BindProperty]
        public string Email { get; set; } = string.Empty;

        [BindProperty]
        public string Password { get; set; } = string.Empty;

        public string ErrorMessage { get; set; } = string.Empty;

        public void OnGet()
        {
            ViewData["HideNavbarAndFooter"] = true;
        }

        public async Task<IActionResult> OnPostAsync()
        {
            ViewData["HideNavbarAndFooter"] = true;

            var user = await AuthenticateUserAsync(Email, Password);

            if (user != null)
            {
                // Create claims for the user
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, Email),
                    new Claim("ID_Usuario", user.ID_Usuario.ToString())  // Claim for user ID
                };

                // Create claims identity
                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                // Sign in the user
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));

                // Redirect to the Index page
                return RedirectToPage("/Index");
            }

            // If login fails, set the error message
            ErrorMessage = "Email ou Senha Incorretos";
            return Page();
        }

        private async Task<Tbl_Usuario?> AuthenticateUserAsync(string email, string password)
        {
            return await _context.Tbl_Usuario
                                 .FirstOrDefaultAsync(u => u.Email == email && u.Senha == password);
        }
    }
}
