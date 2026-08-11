
using Microsoft.AspNetCore.Mvc; // MVC araçlarını kullanabilmek için


namespace OkulYonetim.Controllers;

public class TanitimController : Controller
{
    // ↑ "Controller" sınıfından türetiyoruz.
    //   Bu sayede View(), RedirectToAction() gibi hazır metotlar geliyor.

    public IActionResult Ben()
    {
        // ViewBag: Controller'dan View'a küçük veri taşımanın en kolay yolu
        ViewBag.AdSoyad = "Harun AKSAYA ";
        ViewBag.Bolum = "Bilgisayar Bilimleri";

        return View();   // Views/Tanitim/Merhaba.cshtml dosyasını arar
    }


}


