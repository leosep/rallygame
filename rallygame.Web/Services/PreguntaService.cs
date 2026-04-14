using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using rallygame.Web.Data;
using rallygame.Web.Models;

namespace rallygame.Web.Services
{
    public interface IPreguntaService
    {
        Task<Pregunta?> GetByCodigoAsync(string codigoAlt, string tokenUsuario);
        Task<Respuesta?> SubmitRespuestaAsync(int preguntaId, int opcionId, int usuarioId);
        Task<bool> HasAnsweredAsync(int preguntaId, int usuarioId);
    }

    public class PreguntaService : IPreguntaService
    {
        private readonly ApplicationDbContext _context;

        public PreguntaService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Pregunta?> GetByCodigoAsync(string codigoAlt, string tokenUsuario)
        {
            var pregunta = await _context.Preguntas
                .Include(p => p.Opcions)
                .FirstOrDefaultAsync(p => p.CodigoAlt == codigoAlt && p.Activa == true);

            return pregunta;
        }

        public async Task<bool> HasAnsweredAsync(int preguntaId, int usuarioId)
        {
            return await _context.Respuestas
                .AnyAsync(r => r.IdPregunta == preguntaId && r.IdUsuario == usuarioId);
        }

        public async Task<Respuesta?> SubmitRespuestaAsync(int preguntaId, int opcionId, int usuarioId)
        {
            var opcion = await _context.Opcions
                .FirstOrDefaultAsync(o => o.Id == opcionId && o.IdPregunta == preguntaId);
            
            if (opcion == null) return null;

            var correcta = opcion.Correcto;

            var respuesta = new Respuesta
            {
                IdUsuario = usuarioId,
                IdPregunta = preguntaId,
                IdOpcion = opcionId,
                Correcto = correcta,
                Fecha = DateTime.Now
            };

            _context.Respuestas.Add(respuesta);
            await _context.SaveChangesAsync();
            
            return respuesta;
        }
    }
}