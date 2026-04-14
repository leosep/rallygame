using Microsoft.AspNetCore.Mvc;
using rallygame.Web.Services;
using System.Threading.Tasks;

namespace rallygame.Web.Controllers
{
    [ApiController]
    [Route("api/admin")]
    [ValidateAntiForgeryToken]
    public class AdminApiController : ControllerBase
    {
        private readonly IAdminService _adminService;

        public AdminApiController(IAdminService adminService)
        {
            _adminService = adminService;
        }

        private bool IsAdminAuthenticated()
        {
            return HttpContext.Session.GetString("AdminUser") != null;
        }

        [HttpDelete("pregunta/{id}")]
        public async Task<IActionResult> DeletePregunta(int id)
        {
            if (!IsAdminAuthenticated())
                return Unauthorized(new { error = "No autorizado" });

            if (id <= 0)
                return BadRequest(new { error = "ID inválido" });

            try
            {
                await _adminService.DeletePreguntaAsync(id);
                return Ok(new { success = true });
            }
            catch
            {
                return StatusCode(500, new { error = "Error al eliminar la pregunta" });
            }
        }

        [HttpDelete("usuario/{id}")]
        public async Task<IActionResult> DeleteUsuario(int id)
        {
            if (!IsAdminAuthenticated())
                return Unauthorized(new { error = "No autorizado" });

            if (id <= 0)
                return BadRequest(new { error = "ID inválido" });

            try
            {
                await _adminService.DeleteUsuarioAsync(id);
                return Ok(new { success = true });
            }
            catch
            {
                return StatusCode(500, new { error = "Error al eliminar el usuario" });
            }
        }

        [HttpDelete("respuesta/{id}")]
        public async Task<IActionResult> DeleteRespuesta(int id)
        {
            if (!IsAdminAuthenticated())
                return Unauthorized(new { error = "No autorizado" });

            if (id <= 0)
                return BadRequest(new { error = "ID inválido" });

            try
            {
                await _adminService.DeleteRespuestaAsync(id);
                return Ok(new { success = true });
            }
            catch
            {
                return StatusCode(500, new { error = "Error al eliminar la respuesta" });
            }
        }
    }
}