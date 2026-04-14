using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using rallygame.Web.Services;

namespace rallygame.Web.Pages.Admin
{
    public class PreguntasModel : PageModel
    {
        private readonly IAdminService _adminService;

        public PreguntasModel(IAdminService adminService)
        {
            _adminService = adminService;
        }

        public List<Models.Pregunta> Preguntas { get; set; } = new();

        public async Task<IActionResult> OnGetAsync()
        {
            if (HttpContext.Session.GetString("AdminUser") == null)
                return RedirectToPage("/Admin/Login");
            
            Preguntas = await _adminService.GetAllPreguntasAsync();
            return Page();
        }
    }
}