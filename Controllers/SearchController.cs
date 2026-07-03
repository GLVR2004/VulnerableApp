using System.Diagnostics; // Necesario para Stopwatch
using Microsoft.AspNetCore.Mvc;
using VulnerableApp.Data;
using VulnerableApp.Models;

namespace VulnerableApp.Controllers
{
    public class SearchController : Controller
    {
        private readonly AppDbContext _db;
        private readonly ILogger<SearchController> _logger;

        public SearchController(AppDbContext db, ILogger<SearchController> logger)
        {
            _db = db;
            _logger = logger;
        }

        public IActionResult Index(string search)
        {
            var watch = Stopwatch.StartNew();
            var user = User.Identity?.Name ?? "Anónimo";
            var ip = HttpContext.Connection.RemoteIpAddress?.ToString();

            _logger.LogInformation("Entrada a Search.Index. Término: {Search}. Usuario: {User}, IP: {IP}", search, user, ip);

            try
            {
                if (string.IsNullOrEmpty(search))
                {
                    _logger.LogInformation("Búsqueda vacía, devolviendo lista vacía.");
                    return View(new List<User>());
                }

                var users = _db.Users
                               .Where(u => u.Username != null && u.Username.Contains(search))
                               .ToList();

                watch.Stop();
                _logger.LogInformation("Salida de Search.Index. Resultados: {Count}, Tiempo: {T}ms", users.Count, watch.ElapsedMilliseconds);

                return View(users);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error crítico en Search.Index al buscar: {Search}", search);
                throw;
            }
        }
    }
}
