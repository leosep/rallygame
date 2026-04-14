using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using rallygame.Web.Data;
using rallygame.Web.Models;

namespace rallygame.Web.Services
{
    public interface IUsuarioService
    {
        Task<Usuario> CreateTokenAsync(string nombres, string? identificacion);
        Task<Usuario?> GetByTokenAsync(string token);
    }

    public class UsuarioService : IUsuarioService
    {
        private readonly ApplicationDbContext _context;

        public UsuarioService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Usuario> CreateTokenAsync(string nombres, string? identificacion)
        {
            var token = await GenerateUniqueTokenAsync(6);
            var usuario = new Usuario
            {
                Nombres = nombres,
                Identificacion = identificacion,
                Token = token,
                Fecha = DateTime.Now,
                Activo = true
            };

            _context.Usuarios.Add(usuario);
            await _context.SaveChangesAsync();
            return usuario;
        }

        public async Task<Usuario?> GetByTokenAsync(string token)
        {
            return await _context.Usuarios
                .FirstOrDefaultAsync(u => u.Token == token && u.Activo);
        }

        private async Task<string> GenerateUniqueTokenAsync(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            const int maxRetries = 10;
            int retries = 0;
            
            while (retries < maxRetries)
            {
                string token;
                var result = new char[length];
                using (var rng = RandomNumberGenerator.Create())
                {
                    var randomBytes = new byte[length];
                    rng.GetBytes(randomBytes);
                    for (int i = 0; i < length; i++)
                    {
                        result[i] = chars[randomBytes[i] % chars.Length];
                    }
                }
                token = new string(result);
                
                var exists = await _context.Usuarios.AnyAsync(u => u.Token == token);
                if (!exists)
                    return token;
                
                retries++;
            }
            
            throw new InvalidOperationException("No se pudo generar un token único");
        }
    }
}