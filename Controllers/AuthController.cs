using Microsoft.AspNetCore.Mvc;
using VulnerableApp.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;

namespace VulnerableApp.Controllers
{
    public class AuthController : Controller
    {
        private readonly AppDbContext _db;

        public AuthController(AppDbContext db)
        {
            _db = db;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(string username, string password)
        {
            var user = _db.Users
                .FromSqlRaw("SELECT * FROM Users WHERE Username = {0} AND Password = {1}", username, password)
                .FirstOrDefault();

            if (user != null)
            {
                HttpContext.Session.SetString("Username", user.Username ?? "Usuario");
                return RedirectToAction("Dashboard");
            }

            ViewBag.Error = "Credenciales incorrectas";
            return View();
        }

        public IActionResult Dashboard()
        {
            var user = HttpContext.Session.GetString("Username");
            if (user == null) return RedirectToAction("Login");
            return View((object)user);
        }
    }
}
