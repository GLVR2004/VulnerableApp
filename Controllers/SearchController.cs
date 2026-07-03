using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using VulnerableApp.Data;
using VulnerableApp.Models;
using System.Linq;
using System.Collections.Generic;

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
            _logger.LogInformation("Entrando a Search.Index");
            if (string.IsNullOrEmpty(search))
            {
                return View(new List<User>());
            }

            var users = _db.Users
                           .Where(u => u.Username != null && u.Username.Contains(search))
                           .ToList();
                _logger.LogInformation(
                "Usuario:{User} IP:{IP} Ruta:{Route}",
                HttpContext.Session.GetString("User"),
                HttpContext.Connection.RemoteIpAddress,
                HttpContext.Request.Path);

            return View(users);
        }
    }
}
