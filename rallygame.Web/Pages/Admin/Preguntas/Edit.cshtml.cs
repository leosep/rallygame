using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using rallygame.Web.Services;

namespace rallygame.Web.Pages.Admin
{
    public class EditPreguntaModel : PageModel
    {
        private readonly IAdminService _adminService;

        public EditPreguntaModel(IAdminService adminService)
        {
            _adminService = adminService;
        }

        [BindProperty]
        public int Id { get; set; }

        [BindProperty]
        public string Pregunta1 { get; set; } = string.Empty;

        [BindProperty]
        public string? CodigoAlt { get; set; }

        [BindProperty]
        public bool Activa { get; set; }

        [BindProperty]
        public List<OpcionEdit> Opciones { get; set; } = new();

        [BindProperty]
        public int CorrectoIndex { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            if (HttpContext.Session.GetString("AdminUser") == null)
                return RedirectToPage("/Admin/Login");
            
            var pregunta = await _adminService.GetPreguntaByIdAsync(id);
            if (pregunta == null)
                return NotFound();

            Id = pregunta.Id;
            Pregunta1 = pregunta.Pregunta1;
            CodigoAlt = pregunta.CodigoAlt;
            Activa = pregunta.Activa;

            Opciones = pregunta.Opcions
                .Select((o, i) => new OpcionEdit
                {
                    Id = o.Id,
                    Respuesta = o.Respuesta,
                    Correcto = o.Correcto
                }).ToList();

            CorrectoIndex = Opciones.FindIndex(o => o.Correcto);

            while (Opciones.Count < 4)
                Opciones.Add(new OpcionEdit());

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (string.IsNullOrWhiteSpace(Pregunta1))
                ModelState.AddModelError("Pregunta1", "La pregunta es requerida");

            var opcionesValidas = Opciones.Where(o => !string.IsNullOrWhiteSpace(o.Respuesta)).ToList();
            if (opcionesValidas.Count < 2)
                ModelState.AddModelError("", "Debe tener al menos 2 opciones");

            if (CorrectoIndex < 0 || CorrectoIndex >= Opciones.Count || string.IsNullOrWhiteSpace(Opciones[CorrectoIndex].Respuesta))
                ModelState.AddModelError("", "Debe seleccionar una respuesta correcta");

            if (!ModelState.IsValid)
                return Page();

            var opciones = Opciones
                .Where(o => !string.IsNullOrWhiteSpace(o.Respuesta))
                .Select((o, i) => new Models.Opcion
                {
                    Id = o.Id,
                    Respuesta = o.Respuesta ?? string.Empty,
                    Correcto = Opciones.IndexOf(o) == CorrectoIndex,
                    Orden = i + 1
                }).ToList();

            try
            {
                await _adminService.UpdatePreguntaAsync(Id, Pregunta1, CodigoAlt, Activa, opciones);
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError("", ex.Message);
                return Page();
            }

            return RedirectToPage("/Admin/Preguntas");
        }
    }

    public class OpcionEdit
    {
        public int Id { get; set; }
        public string? Respuesta { get; set; }
        public bool Correcto { get; set; }
    }
}