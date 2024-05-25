using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Security.Claims;
using System.Threading.Tasks;

namespace PortalArcomix.Pages
{
    public class LoginModel : PageModel
    {
        private readonly IConfiguration _configuration;

        public LoginModel(IConfiguration configuration)
        {
            _configuration = configuration;
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

            string connectionString = _configuration.GetConnectionString("PortalArcomixDB")!;
            var (isAuthenticated, role, cnpj) = AuthenticateUser(Email, Password, connectionString);

            if (isAuthenticated && !string.IsNullOrEmpty(role))
            {
                // Create claims including the role
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, Email),
                    new Claim(ClaimTypes.Role, role)
                };

                // Add CNPJ claim if it exists
                if (!string.IsNullOrEmpty(cnpj))
                {
                    claims.Add(new Claim("CNPJ", cnpj));
                }

                // Create claims identity
                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                // Sign in
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));

                // Redirect to the Index page
                return RedirectToPage("/Index");
            }

            // If login fails, set the error message
            ErrorMessage = "Email ou Senha Incorretos";
            return Page();
        }

        private (bool, string, string) AuthenticateUser(string email, string password, string connectionString)
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();
                string query = "SELECT Senha, TipoUsuario, CNPJ FROM Tbl_Usuario WHERE Email=@Email";
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@Email", email);

                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        var storedPassword = reader["Senha"].ToString();
                        var role = reader["TipoUsuario"].ToString();
                        var cnpj = reader["CNPJ"]?.ToString();

                        // Check if the password matches and role is not null or empty
                        if (password == storedPassword && !string.IsNullOrEmpty(role))
                        {
                            return (true, role, cnpj);
                        }
                    }
                }
                return (false, string.Empty, string.Empty); // Ensure non-null strings are returned
            }
        }
    }
}





