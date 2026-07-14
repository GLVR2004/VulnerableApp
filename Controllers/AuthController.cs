using Microsoft.AspNetCore.Mvc;
using VulnerableApp.Data;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace VulnerableApp.Controllers
{
    public class AuthController : Controller
    {
        private readonly AppDbContext _db;
        private readonly ILogger<AuthController> _logger; // 1. Inyecta el Logger

        public AuthController(AppDbContext db, ILogger<AuthController> logger)
        {
            _db = db;
            _logger = logger;
        }

        [HttpGet]
        public IActionResult Login()
        {
            _logger.LogInformation("Carga de vista Login");
            return View();
        }

[HttpPost]
public IActionResult Login(string username, string P_key)
{
    var watch = Stopwatch.StartNew();
    var ip = HttpContext.Connection.RemoteIpAddress?.ToString();

    _logger.LogInformation("Intento de inicio de sesión para usuario: {Username}, IP: {IP}", username, ip);

    var user = _db.Users
        .FromSqlInterpolated($"SELECT * FROM Users WHERE Username = {username} AND P_key = {P_key}")
        .FirstOrDefault();

    watch.Stop();

    if (user != null)
    {
        _logger.LogInformation("Inicio de sesión exitoso para: {Username}. Tiempo: {T}ms", username, watch.ElapsedMilliseconds);
                HttpContext.Session.SetInt32("UserId", user.Id);
        return RedirectToAction("Dashboard");
    }

    _logger.LogWarning("Inicio de sesión fallido para usuario: {Username}, IP: {IP}", username, ip);
    ViewBag.Error = "Credenciales incorrectas";
    return View();
}

        public IActionResult Dashboard()
        {
            var user = HttpContext.Session.GetString("Username");
            if (user == null) 
            {
                _logger.LogWarning("Acceso denegado a Dashboard: usuario no autenticado");
                return RedirectToAction("Login");
            }
            
            _logger.LogInformation("Acceso al Dashboard: {Username}", user);
            return View((object)user);
        }
    }
}
