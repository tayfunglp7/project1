using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using OkulYonetim.Models;

namespace OkulYonetim.Controllers;

public class FakulteController : Controller
{
    private readonly ILogger<FakulteController> _logger;

    public FakulteController(ILogger<FakulteController> logger)
    {
        _logger = logger;
    }

    public IActionResult FakulteListesi()
    {
        return View();
    }

    public IActionResult FakulteEkle()
    {
        return View();
    }

    public IActionResult FakulteDuzenle()
    {
        return View();
    }

      public IActionResult FakulteSil()
    {
        return View();
    }

   

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
