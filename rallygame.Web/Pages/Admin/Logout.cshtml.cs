using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace rallygame.Web.Pages.Admin
{
    [IgnoreAntiforgeryToken]
    public class LogoutModel : PageModel
    {
        public IActionResult OnPost()
        {
            HttpContext.Session.Clear();
            return RedirectToPage("/Admin/Login");
        }
        
        public IActionResult OnGet()
        {
            HttpContext.Session.Clear();
            return RedirectToPage("/Admin/Login");
        }
    }
}