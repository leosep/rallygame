using Microsoft.EntityFrameworkCore;
using rallygame.Web.Models;

namespace rallygame.Web.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Pregunta> Preguntas { get; set; }
        public DbSet<Opcion> Opcions { get; set; }
        public DbSet<Respuesta> Respuestas { get; set; }
        public DbSet<AdminUser> AdminUsers { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            
            builder.Entity<Usuario>()
                .HasIndex(u => u.Token)
                .IsUnique();
            
            builder.Entity<Pregunta>()
                .HasIndex(p => p.CodigoAlt);
            
            builder.Entity<Pregunta>()
                .HasMany(p => p.Opcions)
                .WithOne(o => o.Pregunta)
                .HasForeignKey(o => o.IdPregunta)
                .OnDelete(DeleteBehavior.Cascade);
            
            builder.Entity<Respuesta>()
                .HasOne(r => r.Usuario)
                .WithMany(u => u.Respuestas)
                .HasForeignKey(r => r.IdUsuario)
                .OnDelete(DeleteBehavior.Restrict);
            
            builder.Entity<Respuesta>()
                .HasOne(r => r.Pregunta)
                .WithMany()
                .HasForeignKey(r => r.IdPregunta)
                .OnDelete(DeleteBehavior.Restrict);
            
            builder.Entity<Respuesta>()
                .HasOne(r => r.Opcion)
                .WithMany()
                .HasForeignKey(r => r.IdOpcion)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<AdminUser>()
                .HasIndex(a => a.Username)
                .IsUnique();
        }
    }
}