using Microsoft.AspNetCore.Mvc;
using VulnerableApp.Data;
using System.Diagnostics; // Necesario para Stopwatch

namespace VulnerableApp.Controllers
{
    [ApiController]
    [Route("api")]
    public class ApiController : ControllerBase
    {
        private readonly AppDbContext _db;
        private readonly ILogger<ApiController> _logger; // 1. Inyecta el logger

        public ApiController(AppDbContext db, ILogger<ApiController> logger) 
        { 
            _db = db; 
            _logger = logger; 
        }

        [HttpGet("user/{id}")]
        public IActionResult GetUser(int id)
        {
            var watch = Stopwatch.StartNew();
            var userAgent = HttpContext.Connection.RemoteIpAddress?.ToString();
            var currentUser = User.Identity?.Name ?? "Anónimo";

            // 2. Registro de Entrada
            _logger.LogInformation("Entrada a GetUser. ID solicitado: {Id}. Usuario: {User}, IP: {IP}", id, currentUser, userAgent);

            try
            {
                var currentUserId = HttpContext.Session.GetInt32("UserId");

                if (currentUserId == null || currentUserId != id) 
                {
                    // 3. Registro de Warning (Seguridad)
                    _logger.LogWarning("Intento de acceso no autorizado al ID: {Id} por usuario: {User}", id, currentUser);
                    return Unauthorized(new { message = "Acceso denegado." });
                }

                var user = _db.Users.Find(id);
                if (user == null) 
                {
                    _logger.LogWarning("Usuario con ID {Id} no encontrado", id);
                    return NotFound();
                }

                watch.Stop();
                // 4. Registro de Salida con Tiempo
                _logger.LogInformation("Salida de GetUser. Tiempo: {Tiempo}ms", watch.ElapsedMilliseconds);

                return Ok(new { user.Id, user.Username, user.Email });
            }
            catch (Exception ex)
            {
                // 5. Registro de Error
                _logger.LogError(ex, "Error crítico en GetUser para ID: {Id}", id);
                throw;
            }
        }
    }
}
