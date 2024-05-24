using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace PortalArcomix.Pages
{
    public class IndexModel : PageModel
    {
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

        public IActionResult OnPost()
        {
            // Redirect to the Privacy page
            return RedirectToPage("/Privacy");
        }
    }
}