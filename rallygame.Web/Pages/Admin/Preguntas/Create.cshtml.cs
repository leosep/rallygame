using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using rallygame.Web.Services;

namespace rallygame.Web.Pages.Admin
{
    public class CreatePreguntaModel : PageModel
    {
        private readonly IAdminService _adminService;

        public CreatePreguntaModel(IAdminService adminService)
        {
            _adminService = adminService;
        }

        [BindProperty]
        public string Pregunta1 { get; set; } = string.Empty;

        [BindProperty]
        public string? CodigoAlt { get; set; }

        [BindProperty]
        public bool Activa { get; set; } = true;

        [BindProperty]
        public List<OpcionInput> Opciones { get; set; } = new()
        {
            new OpcionInput(),
            new OpcionInput(),
            new OpcionInput(),
            new OpcionInput()
        };

        [BindProperty]
        public int CorrectoIndex { get; set; }

        public async Task<IActionResult> OnPostAsync()
        {
            if (string.IsNullOrWhiteSpace(Pregunta1))
                ModelState.AddModelError("Pregunta1", "La pregunta es requerida");

            if (Opciones.Count(o => !string.IsNullOrWhiteSpace(o.Respuesta)) < 2)
                ModelState.AddModelError("", "Debe tener al menos 2 opciones");

            if (CorrectoIndex < 0 || CorrectoIndex >= Opciones.Count || string.IsNullOrWhiteSpace(Opciones[CorrectoIndex].Respuesta))
                ModelState.AddModelError("", "Debe seleccionar una respuesta correcta");

            if (!ModelState.IsValid)
                return Page();

            var opcionesValidas = Opciones.Where(o => !string.IsNullOrWhiteSpace(o.Respuesta)).ToList();
            if (!opcionesValidas.Any(o => Opciones.IndexOf(o) == CorrectoIndex))
                ModelState.AddModelError("", "La respuesta correcta debe ser una de las opciones válidas");

            if (!ModelState.IsValid)
                return Page();

            if (string.IsNullOrWhiteSpace(CodigoAlt))
            {
                CodigoAlt = await _adminService.GenerateUniqueCodigoAltAsync();
            }

            var opciones = Opciones
                .Where(o => !string.IsNullOrWhiteSpace(o.Respuesta))
                .Select((o, i) => new Models.Opcion
                {
                    Respuesta = o.Respuesta ?? string.Empty,
                    Correcto = Opciones.IndexOf(o) == CorrectoIndex,
                    Orden = i + 1
                }).ToList();

            await _adminService.CreatePreguntaAsync(Pregunta1, CodigoAlt, Activa, opciones);

            return RedirectToPage("/Admin/Preguntas");
        }
    }

    public class OpcionInput
    {
        public string? Respuesta { get; set; }
    }
}