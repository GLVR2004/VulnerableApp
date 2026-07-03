using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using VulnerableApp.Models;

namespace VulnerableApp.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    public IActionResult Index()
    {
        var watch = Stopwatch.StartNew();
        _logger.LogInformation("Entrada a Index. Usuario: {User}, IP: {IP}", 
            User.Identity?.Name ?? "Anónimo", HttpContext.Connection.RemoteIpAddress);

        try
        {
            var result = View();
            watch.Stop();
            _logger.LogInformation("Salida de Index. Tiempo: {T}ms", watch.ElapsedMilliseconds);
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error en Index");
            throw;
        }
    }

    public IActionResult Privacy()
    {
        _logger.LogInformation("Acceso a Privacy por usuario: {User}", User.Identity?.Name ?? "Anónimo");
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        var requestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier;
        _logger.LogError("Error detectado. RequestId: {RequestId}", requestId);
        
        return View(new ErrorViewModel { RequestId = requestId });
    }
}
