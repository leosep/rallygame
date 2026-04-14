using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace rallygame.Web.Models
{
    public class Respuesta
    {
        [Key]
        public int Id { get; set; }
        
        public int IdUsuario { get; set; }
        
        [ForeignKey("IdUsuario")]
        public virtual Usuario? Usuario { get; set; }
        
        public int IdPregunta { get; set; }
        
        [ForeignKey("IdPregunta")]
        public virtual Pregunta? Pregunta { get; set; }
        
        public int IdOpcion { get; set; }
        
        [ForeignKey("IdOpcion")]
        public virtual Opcion? Opcion { get; set; }
        
        public bool Correcto { get; set; }
        
        public DateTime Fecha { get; set; } = DateTime.Now;
    }
}
