using System;
using System.ComponentModel.DataAnnotations;

namespace rallygame.Web.Models
{
    public class AdminUser
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        [MaxLength(100)]
        public string Username { get; set; } = string.Empty;
        
        [Required]
        public string PasswordHash { get; set; } = string.Empty;
        
        [MaxLength(100)]
        public string? Role { get; set; }
        
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        
        public bool Activo { get; set; } = true;
    }
}