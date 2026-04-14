using Microsoft.AspNetCore.Mvc;
using rallygame.Web.Services;

namespace rallygame.Web.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class QRController : ControllerBase
    {
        private readonly IQRCodeService _qrService;

        public QRController(IQRCodeService qrService)
        {
            _qrService = qrService;
        }

        [HttpGet("{codigo}")]
        public IActionResult GetQR(string codigo)
        {
            if (string.IsNullOrEmpty(codigo))
                return BadRequest();

            var qrBytes = _qrService.GenerateQRCode(codigo);
            return File(qrBytes, "image/png");
        }
    }
}