using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace rallygame.Web.Models
{
    public class Pregunta
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        public string Pregunta1 { get; set; } = string.Empty;
        
        [MaxLength(50)]
        public string? CodigoAlt { get; set; }
        
        public int? Orden { get; set; }
        
        public bool Activa { get; set; } = true;
        
        public virtual ICollection<Opcion> Opcions { get; set; } = new List<Opcion>();
    }
}
