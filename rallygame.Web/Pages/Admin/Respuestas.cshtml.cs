using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using rallygame.Web.Services;

namespace rallygame.Web.Pages.Admin
{
    public class RespuestasModel : PageModel
    {
        private readonly IAdminService _adminService;

        public RespuestasModel(IAdminService adminService)
        {
            _adminService = adminService;
        }

        public List<Models.Respuesta> Respuestas { get; set; } = new();

        public async Task<IActionResult> OnGetAsync(string? token = null)
        {
            if (HttpContext.Session.GetString("AdminUser") == null)
                return RedirectToPage("/Admin/Login");
            
            Respuestas = await _adminService.GetAllRespuestasAsync(token);
            return Page();
        }
    }
}