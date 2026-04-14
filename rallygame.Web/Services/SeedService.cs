using System.Security.Cryptography;
using System.Text;
using Microsoft.EntityFrameworkCore;
using rallygame.Web.Data;
using rallygame.Web.Models;

namespace rallygame.Web.Services
{
    public interface ISeedService
    {
        Task SeedDataAsync();
    }

    public class SeedService : ISeedService
    {
        private readonly ApplicationDbContext _context;

        public SeedService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task SeedDataAsync()
        {
            if (!await _context.AdminUsers.AnyAsync())
            {
                var adminUser = new AdminUser
                {
                    Username = "admin",
                    PasswordHash = HashPassword("admin5012"),
                    Role = "Admin",
                    Activo = true,
                    CreatedAt = DateTime.Now
                };
                _context.AdminUsers.Add(adminUser);
                await _context.SaveChangesAsync();
            }

            var existingUser = await _context.AdminUsers.FirstOrDefaultAsync(u => u.Username == "admin2");
            if (existingUser == null)
            {
                var adminUser2 = new AdminUser
                {
                    Username = "admin2",
                    PasswordHash = HashPassword("admin5013"),
                    Role = "Admin",
                    Activo = true,
                    CreatedAt = DateTime.Now
                };
                _context.AdminUsers.Add(adminUser2);
                await _context.SaveChangesAsync();
            }
            else if (existingUser != null && !existingUser.Activo)
            {
                existingUser.Activo = true;
                await _context.SaveChangesAsync();
            }

            if (await _context.Preguntas.AnyAsync())
                return;

            await _context.SaveChangesAsync();
        }

        private static string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            var bytes = Encoding.UTF8.GetBytes(password);
            var hash = sha256.ComputeHash(bytes);
            return Convert.ToBase64String(hash);
        }
    }
}