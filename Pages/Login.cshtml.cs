using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using System.Data;
using System.Data.SqlClient;

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

        public void OnGet()
        {
        }

        public IActionResult OnPost()
        {
            string connectionString = _configuration.GetConnectionString("PortalArcomixDB")!;
            bool isAuthenticated = AuthenticateUser(Email, Password, connectionString);

            if (isAuthenticated)
            {
                // Simulate a successful login and redirect to the home page
                return RedirectToPage("/Index");
            }

            // If login fails, show an error message
            ModelState.AddModelError(string.Empty, "Invalid login attempt.");
            return Page();
        }

        private bool AuthenticateUser(string email, string password, string connectionString)
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                string query = "SELECT COUNT(1) FROM Tbl_Usuario WHERE Email=@Email AND Senha=@Senha";
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@Email", email);
                cmd.Parameters.AddWithValue("@Senha", password);

                con.Open();
                int count = Convert.ToInt32(cmd.ExecuteScalar());
                return count == 1;
            }
        }
    }
}


