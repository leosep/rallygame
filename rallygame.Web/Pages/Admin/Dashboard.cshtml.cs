using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using rallygame.Web.Services;

namespace rallygame.Web.Pages.Admin
{
    public class DashboardModel : PageModel
    {
        private readonly IAdminService _adminService;

        public DashboardModel(IAdminService adminService)
        {
            _adminService = adminService;
        }

        public DashboardStats Stats { get; set; } = new();
        public List<Models.Respuesta> RecentRespuestas { get; set; } = new();

        public async Task<IActionResult> OnGetAsync()
        {
            if (HttpContext.Session.GetString("AdminUser") == null)
                return RedirectToPage("/Admin/Login");
            
            Stats = await _adminService.GetDashboardStatsAsync();
            var respuestas = await _adminService.GetAllRespuestasAsync();
            RecentRespuestas = respuestas.Take(20).ToList();
            return Page();
        }
    }
}