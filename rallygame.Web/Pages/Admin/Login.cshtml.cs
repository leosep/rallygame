using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using rallygame.Web.Data;
using System.Security.Cryptography;
using System.Text;

namespace rallygame.Web.Pages.Admin
{
    public class LoginModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        [BindProperty]
        public string Username { get; set; } = string.Empty;

        [BindProperty]
        public string Password { get; set; } = string.Empty;

        public string? ErrorMessage { get; set; }

        public LoginModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult OnGet()
        {
            if (HttpContext.Session.GetString("AdminUser") != null)
            {
                return RedirectToPage("/Admin/Dashboard");
            }
            return Page();
        }

        public IActionResult OnPost()
        {
            if (string.IsNullOrWhiteSpace(Username) || string.IsNullOrWhiteSpace(Password))
            {
                ErrorMessage = "Por favor ingresa usuario y contraseña";
                return Page();
            }

            var admin = _context.AdminUsers
                .FirstOrDefault(a => a.Username == Username && a.Activo);

            if (admin == null)
            {
                ErrorMessage = "Credenciales incorrectas";
                return Page();
            }

            var passwordHash = HashPassword(Password);
            if (admin.PasswordHash != passwordHash)
            {
                ErrorMessage = "Credenciales incorrectas";
                return Page();
            }

            HttpContext.Session.SetString("AdminUser", Username);
            return RedirectToPage("/Admin/Dashboard");
        }

        private static string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            var bytes = Encoding.UTF8.GetBytes(password);
            var hash = sha256.ComputeHash(bytes);
            return Convert.ToBase64String(hash);
        }
    }
}