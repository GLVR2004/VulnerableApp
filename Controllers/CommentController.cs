using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace VulnerableApp.Controllers
{
    public class CommentController : Controller
    {
        private static List<string> _comments = new();
        private readonly ILogger<CommentController> _logger; // 1. Inyección del logger

        public CommentController(ILogger<CommentController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            _logger.LogInformation("Cargando lista de comentarios. Total: {Count}", _comments.Count);
            return View(_comments);
        }

        [HttpPost]
        public IActionResult AddComment(string comment)
        {
            var watch = Stopwatch.StartNew();
            var user = User.Identity?.Name ?? "Anónimo";

            _logger.LogInformation("Intento de añadir comentario. Usuario: {User}, Contenido: {Comment}", user, comment);

            try
            {
                if (!string.IsNullOrEmpty(comment))
                {
                    _comments.Add(comment);
                    _logger.LogInformation("Comentario añadido exitosamente por {User}", user);
                }
                else
                {
                    _logger.LogWarning("Intento de añadir comentario vacío por {User}", user);
                }

                watch.Stop();
                _logger.LogInformation("Acción AddComment finalizada. Tiempo: {T}ms", watch.ElapsedMilliseconds);
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al procesar el comentario de {User}", user);
                throw;
            }
        }
    }
}
