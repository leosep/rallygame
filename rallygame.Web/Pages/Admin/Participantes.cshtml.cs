using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using rallygame.Web.Services;

namespace rallygame.Web.Pages.Admin
{
    public class ParticipantesModel : PageModel
    {
        private readonly IAdminService _adminService;

        public ParticipantesModel(IAdminService adminService)
        {
            _adminService = adminService;
        }

        public List<Models.Usuario> Usuarios { get; set; } = new();

        public async Task<IActionResult> OnGetAsync()
        {
            if (HttpContext.Session.GetString("AdminUser") == null)
                return RedirectToPage("/Admin/Login");
            
            Usuarios = await _adminService.GetAllUsuariosAsync();
            return Page();
        }
    }
}