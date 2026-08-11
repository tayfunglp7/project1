using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using OkulYonetim.Models;

namespace OkulYonetim.Controllers;

public class BolumController : Controller
{
    private readonly ILogger<BolumController> _logger;

    public BolumController(ILogger<BolumController> logger)
    {
        _logger = logger;
    }

    public IActionResult BolumListesi()
    {
        return View();
    }

    public IActionResult BolumEkle()
    {
        return View();
    }

    public IActionResult BolumDuzenle()
    {
        return View();
    }

      public IActionResult BolumSil()
    {
        return View();
    }

   

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
