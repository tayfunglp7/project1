using Microsoft.AspNetCore.Mvc;


namespace OkulYonetim.Controllers;

public class TanitimController : Controller
{
    public IActionResult Ben()
    {
        ViewBag.AdSoyad = "Tayfun Gölpunar";
        ViewBag.Bolum = "Yazılım";

        return View();
    }

}