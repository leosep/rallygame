using Microsoft.AspNetCore.Mvc;
using rallygame.Web.Services;

namespace rallygame.Web.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PreguntaController : ControllerBase
    {
        private readonly IPreguntaService _service;
        private readonly IUsuarioService _usuarioService;

        public PreguntaController(IPreguntaService service, IUsuarioService usuarioService)
        {
            _service = service;
            _usuarioService = usuarioService;
        }

        [HttpGet]
        public async Task<IActionResult> GetPregunta([FromQuery] string codigoAlt, [FromQuery] string token)
        {
            if (string.IsNullOrWhiteSpace(codigoAlt))
                return BadRequest(new { error = "Codigo es requerido" });
            
            if (string.IsNullOrWhiteSpace(token))
                return Unauthorized(new { error = "Token es requerido" });
            
            var pregunta = await _service.GetByCodigoAsync(codigoAlt, token);
            
            if (pregunta == null)
                return NotFound(new { error = "Código no válido" });

            if (!pregunta.Activa)
                return BadRequest(new { error = "Esta pregunta ya no está activa" });

            var usuario = await _usuarioService.GetByTokenAsync(token);
            if (usuario == null)
                return Unauthorized(new { error = "Usuario no válido" });

            var hasAnswered = await _service.HasAnsweredAsync(pregunta.Id, usuario.Id);
            if (hasAnswered)
                return Ok(new { existe = true });

            var opciones = pregunta.Opcions?.Select(o => new
            {
                id = o.Id,
                respuesta = o.Respuesta,
                correcto = o.Correcto,
                orden = o.Orden
            }).ToList();

            return Ok(new
            {
                id = pregunta.Id,
                pregunta1 = pregunta.Pregunta1,
                codigoAlt = pregunta.CodigoAlt,
                orden = pregunta.Orden,
                activa = pregunta.Activa,
                opcions = opciones
            });
        }

        [HttpPost("respuesta")]
        public async Task<IActionResult> SubmitRespuesta([FromBody] RespuestaRequest request)
        {
            if (request == null)
                return BadRequest(new { error = "Solicitud inválida" });
            
            if (request.Pregunta <= 0)
                return BadRequest(new { error = "Pregunta inválida" });
            
            if (request.Opcion <= 0)
                return BadRequest(new { error = "Opción inválida" });
            
            if (string.IsNullOrWhiteSpace(request.Token))
                return BadRequest(new { error = "Token es requerido" });
            
            var usuario = await _usuarioService.GetByTokenAsync(request.Token);
            if (usuario == null)
                return Unauthorized(new { error = "Usuario no válido" });

            var hasAnswered = await _service.HasAnsweredAsync(request.Pregunta, usuario.Id);
            if (hasAnswered)
                return BadRequest(new { error = "Ya respondiste esta pregunta" });

            var respuesta = await _service.SubmitRespuestaAsync(request.Pregunta, request.Opcion, usuario.Id);
            
            return Ok(new { correcto = respuesta?.Correcto ?? false });
        }
    }

    public class RespuestaRequest
    {
        public int Pregunta { get; set; }
        public int Opcion { get; set; }
        public string Token { get; set; } = string.Empty;
    }
}