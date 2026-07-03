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
        public IActionResult Login(string username, string password)
        {
            var watch = Stopwatch.StartNew();
            var ip = HttpContext.Connection.RemoteIpAddress?.ToString();

            // 2. Registro de entrada SIN la contraseña
            _logger.LogInformation("Intento de inicio de sesión para usuario: {Username}, IP: {IP}", username, ip);

            // Nota: El uso de FromSqlRaw aquí es vulnerable a SQL Injection. 
            // Aunque instrumentes el log, la práctica seguramente espera que identifiques esta vulnerabilidad.
            var user = _db.Users
                .FromSqlRaw("SELECT * FROM Users WHERE Username = {0} AND Password = {1}", username, password)
                .FirstOrDefault();

            watch.Stop();

            if (user != null)
            {
                _logger.LogInformation("Inicio de sesión exitoso para: {Username}. Tiempo: {T}ms", username, watch.ElapsedMilliseconds);
                HttpContext.Session.SetString("Username", user.Username ?? "Usuario");
                return RedirectToAction("Dashboard");
            }

            // 3. Registro de advertencia por falla
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
