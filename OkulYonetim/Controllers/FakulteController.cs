using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using OkulYonetim.Models;
using OkulYonetim.Data;

namespace OkulYonetim.Controllers;

public class FakulteController : Controller
{
    private readonly FakulteRepository _repo;
    // Yapıcı metot: sistem bize hazır bir FakulteRepository veriyor
    public FakulteController(FakulteRepository repo)
    {
        _repo = repo;
    }
    // GET: /Fakulte
    public IActionResult Index()
    {
        // 1. Repository'den veriyi al
        List<Fakulte> fakulteler = _repo.TumunuGetir();

        // 2. View'a gönder
        return View(fakulteler);
    }
}
