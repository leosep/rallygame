using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using rallygame.Web.Data;
using rallygame.Web.Models;

namespace rallygame.Web.Services
{
    public interface IAdminService
    {
        Task<DashboardStats> GetDashboardStatsAsync();
        Task<List<Respuesta>> GetAllRespuestasAsync(string? token = null);
        Task<List<Usuario>> GetAllUsuariosAsync();
        Task<List<Pregunta>> GetAllPreguntasAsync();
        Task<Pregunta?> GetPreguntaByIdAsync(int id);
        Task<string> GenerateUniqueCodigoAltAsync();
        Task CreatePreguntaAsync(string pregunta, string? codigoAlt, bool activa, List<Opcion>? opciones);
        Task UpdatePreguntaAsync(int id, string pregunta, string? codigoAlt, bool activa, List<Opcion>? opciones);
        Task DeletePreguntaAsync(int id);
        Task DeleteUsuarioAsync(int id);
        Task DeleteRespuestaAsync(int id);
    }

    public class DashboardStats
    {
        public int TotalParticipantes { get; set; }
        public int TotalPreguntas { get; set; }
        public int TotalRespuestas { get; set; }
        public int RespuestasCorrectas { get; set; }
        public double PorcentajeAciertos { get; set; }
    }

    public class AdminService : IAdminService
    {
        private readonly ApplicationDbContext _context;

        public AdminService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<DashboardStats> GetDashboardStatsAsync()
        {
            var usuarios = await _context.Usuarios.ToListAsync();
            var preguntas = await _context.Preguntas.ToListAsync();
            var respuestas = await _context.Respuestas.ToListAsync();
            
            var stats = new DashboardStats
            {
                TotalParticipantes = usuarios.Count,
                TotalPreguntas = preguntas.Count,
                TotalRespuestas = respuestas.Count,
                RespuestasCorrectas = respuestas.Count(r => r.Correcto),
                PorcentajeAciertos = respuestas.Count > 0 
                    ? System.Math.Round((double)respuestas.Count(r => r.Correcto) / respuestas.Count * 100, 1) 
                    : 0
            };
            
            return stats;
        }

        public async Task<List<Respuesta>> GetAllRespuestasAsync(string? token = null)
        {
            var query = _context.Respuestas
                .Include(r => r.Usuario)
                .Include(r => r.Pregunta)
                .Include(r => r.Opcion)
                .OrderByDescending(r => r.Fecha)
                .AsQueryable();

            if (!string.IsNullOrEmpty(token))
            {
                query = query.Where(r => r.Usuario != null && r.Usuario.Token == token);
            }

            return await query.ToListAsync();
        }

        public async Task<List<Usuario>> GetAllUsuariosAsync()
        {
            return await _context.Usuarios
                .Where(u => u.Activo)
                .OrderByDescending(u => u.Fecha)
                .ToListAsync();
        }

        public async Task<List<Pregunta>> GetAllPreguntasAsync()
        {
            return await _context.Preguntas
                .Include(p => p.Opcions)
                .OrderBy(p => p.Orden ?? int.MaxValue)
                .ToListAsync();
        }

        public async Task<Pregunta?> GetPreguntaByIdAsync(int id)
        {
            return await _context.Preguntas
                .Include(p => p.Opcions)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<string> GenerateUniqueCodigoAltAsync()
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            const int maxRetries = 10;
            int retries = 0;
            
            while (retries < maxRetries)
            {
                string codigo;
                var result = new char[8];
                using (var rng = RandomNumberGenerator.Create())
                {
                    var randomBytes = new byte[8];
                    rng.GetBytes(randomBytes);
                    for (int i = 0; i < 8; i++)
                    {
                        result[i] = chars[randomBytes[i] % chars.Length];
                    }
                }
                codigo = new string(result);
                
                var exists = await _context.Preguntas.AnyAsync(p => p.CodigoAlt == codigo);
                if (!exists)
                    return codigo;
                
                retries++;
            }
            
            throw new InvalidOperationException("No se pudo generar un código único");
        }

        public async Task CreatePreguntaAsync(string pregunta, string? codigoAlt, bool activa, List<Opcion>? opciones)
        {
            if (opciones == null || opciones.Count == 0)
            {
                throw new InvalidOperationException("Se requiere al menos una opción");
            }

            if (!opciones.Any(o => o.Correcto))
            {
                throw new InvalidOperationException("Se debe seleccionar una opción como correcta");
            }

            if (!string.IsNullOrWhiteSpace(codigoAlt))
            {
                var existingWithCode = await _context.Preguntas
                    .FirstOrDefaultAsync(p => p.CodigoAlt == codigoAlt);
                if (existingWithCode != null)
                {
                    throw new InvalidOperationException("El código alternativo ya está en uso");
                }
            }

            var maxOrden = await _context.Preguntas.MaxAsync(p => (int?)p.Orden) ?? 0;
            var nuevaPregunta = new Pregunta
            {
                Pregunta1 = pregunta,
                CodigoAlt = codigoAlt,
                Activa = activa,
                Orden = maxOrden + 1
            };

            _context.Preguntas.Add(nuevaPregunta);
            await _context.SaveChangesAsync();

            foreach (var op in opciones)
            {
                op.IdPregunta = nuevaPregunta.Id;
            }

            _context.Opcions.AddRange(opciones);
            await _context.SaveChangesAsync();
        }

        public async Task UpdatePreguntaAsync(int id, string pregunta, string? codigoAlt, bool activa, List<Opcion>? opciones)
        {
            var preguntaExistente = await _context.Preguntas
                .Include(p => p.Opcions)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (preguntaExistente == null) return;

            if (!string.IsNullOrWhiteSpace(codigoAlt))
            {
                var existingWithCode = await _context.Preguntas
                    .FirstOrDefaultAsync(p => p.CodigoAlt == codigoAlt && p.Id != id);
                if (existingWithCode != null)
                {
                    throw new InvalidOperationException("El código alternativo ya está en uso por otra pregunta");
                }
            }

            preguntaExistente.Pregunta1 = pregunta;
            preguntaExistente.CodigoAlt = string.IsNullOrWhiteSpace(codigoAlt) ? null : codigoAlt;
            preguntaExistente.Activa = activa;

            if (opciones == null || opciones.Count == 0)
            {
                throw new InvalidOperationException("Se requiere al menos una opción");
            }

            if (!opciones.Any(o => o.Correcto))
            {
                throw new InvalidOperationException("Se debe seleccionar una opción como correcta");
            }

            _context.Opcions.RemoveRange(preguntaExistente.Opcions);
            foreach (var op in opciones)
            {
                op.IdPregunta = id;
                op.Activa = true;
            }
            _context.Opcions.AddRange(opciones);

            await _context.SaveChangesAsync();
        }

        public async Task DeletePreguntaAsync(int id)
        {
            var pregunta = await _context.Preguntas
                .Include(p => p.Opcions)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (pregunta != null)
            {
                _context.Opcions.RemoveRange(pregunta.Opcions);
                _context.Preguntas.Remove(pregunta);
                await _context.SaveChangesAsync();
            }
        }

        public async Task DeleteUsuarioAsync(int id)
        {
            var usuario = await _context.Usuarios
                .Include(u => u.Respuestas)
                .FirstOrDefaultAsync(u => u.Id == id);

            if (usuario != null)
            {
                if (usuario.Respuestas != null && usuario.Respuestas.Any())
                {
                    _context.Respuestas.RemoveRange(usuario.Respuestas);
                }
                _context.Usuarios.Remove(usuario);
                await _context.SaveChangesAsync();
            }
        }

        public async Task DeleteRespuestaAsync(int id)
        {
            var respuesta = await _context.Respuestas.FindAsync(id);
            if (respuesta != null)
            {
                _context.Respuestas.Remove(respuesta);
                await _context.SaveChangesAsync();
            }
        }
    }
}