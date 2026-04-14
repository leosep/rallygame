using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace rallygame.Web.Models
{
    public class Usuario
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        [MaxLength(200)]
        public string Nombres { get; set; } = string.Empty;
        
        [MaxLength(50)]
        public string? Identificacion { get; set; }
        
        [Required]
        [MaxLength(10)]
        public string Token { get; set; } = string.Empty;
        
        public DateTime Fecha { get; set; } = DateTime.Now;
        
        public bool Activo { get; set; } = true;
        
        public virtual ICollection<Respuesta> Respuestas { get; set; } = new List<Respuesta>();
    }
}
