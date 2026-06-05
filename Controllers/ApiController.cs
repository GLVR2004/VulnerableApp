using Microsoft.AspNetCore.Mvc;
using VulnerableApp.Data;

namespace VulnerableApp.Controllers
{
    [ApiController]
    [Route("api")]
    public class ApiController : ControllerBase
    {
        private readonly AppDbContext _db;
        public ApiController(AppDbContext db) { _db = db; }

        [HttpGet("user/{id}")]
       public IActionResult GetUser(int id)
{
    // Obtener el ID del usuario desde la sesión
    var currentUserId = HttpContext.Session.GetInt32("UserId");
    
    // CORRECCIÓN: Si no hay sesión o el ID no coincide, 
    // devolvemos Unauthorized (401) en lugar de Forbid (403)
    if (currentUserId == null || currentUserId != id) 
    {
        return Unauthorized(new { message = "Acceso denegado: No tienes permiso para ver este recurso." });
    }

    var user = _db.Users.Find(id);
    if (user == null) return NotFound();

    return Ok(new { user.Id, user.Username, user.Email });
}
    }
}
