using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace rallygame.Web.Models
{
    public class Opcion
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        public string Respuesta { get; set; } = string.Empty;
        
        public bool Correcto { get; set; }
        
        public int IdPregunta { get; set; }
        
        [ForeignKey("IdPregunta")]
        public virtual Pregunta? Pregunta { get; set; }
        
        public int? Orden { get; set; }
        
        public bool Activa { get; set; } = true;
    }
}
