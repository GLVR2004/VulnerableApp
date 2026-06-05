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
            var user = _db.Users.Find(id);
            if (user == null) return NotFound();

            return Ok(new { user.Id, user.Username, user.Email });
        }

        [HttpGet("users")]
        public IActionResult GetAllUsers()
        {
            return Ok(_db.Users.ToList());
        }
    }
}
