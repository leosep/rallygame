using Microsoft.AspNetCore.Mvc;
using rallygame.Web.Services;
using Microsoft.Extensions.Logging;

namespace rallygame.Web.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsuarioController : ControllerBase
    {
        private readonly IUsuarioService _usuarioService;
        private readonly ILogger<UsuarioController> _logger;

        public UsuarioController(IUsuarioService usuarioService, ILogger<UsuarioController> logger)
        {
            _usuarioService = usuarioService;
            _logger = logger;
        }

        [HttpPost("token")]
        public async Task<IActionResult> CreateToken([FromBody] UsuarioRequest request)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(request.Nombres))
                    return BadRequest(new { error = "El nombre es requerido" });
                
                if (request.Nombres.Trim().Length < 2)
                    return BadRequest(new { error = "El nombre debe tener al menos 2 caracteres" });
                
                if (request.Nombres.Trim().Length > 200)
                    return BadRequest(new { error = "El nombre es demasiado largo" });
                
                _logger.LogInformation("Creating token for: {Nombres}", request.Nombres);
                var usuario = await _usuarioService.CreateTokenAsync(request.Nombres.Trim(), request.Identificacion);
                return Ok(new { token = usuario.Token });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating token");
                return StatusCode(500, new { error = "Error al registrar usuario" });
            }
        }

        [HttpGet("verify")]
        public async Task<IActionResult> VerifyToken([FromQuery] string token)
        {
            _logger.LogInformation("Verify endpoint called with token: {Token}", token);
            if (string.IsNullOrWhiteSpace(token))
                return BadRequest(new { error = "Token es requerido" });
            
            var usuario = await _usuarioService.GetByTokenAsync(token);
            if (usuario == null)
                return NotFound(new { error = "Token inválido" });
            return Ok(new { nombres = usuario.Nombres, token = usuario.Token });
        }
    }

    public class TokenRequest
    {
        public string Token { get; set; } = string.Empty;
    }

    public class UsuarioRequest
    {
        public string Nombres { get; set; } = string.Empty;
        public string? Identificacion { get; set; }
    }
}