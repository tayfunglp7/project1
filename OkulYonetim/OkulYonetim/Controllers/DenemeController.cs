
using Microsoft.AspNetCore.Mvc; // MVC araçlarını kullanabilmek için


namespace OkulYonetim.Controllers;

public class DenemeController : Controller
{
    // ↑ "Controller" sınıfından türetiyoruz.
    //   Bu sayede View(), RedirectToAction() gibi hazır metotlar geliyor.

    public IActionResult Merhaba()
    {
        // ViewBag: Controller'dan View'a küçük veri taşımanın en kolay yolu
        ViewBag.Mesaj = "Merhaba! Bu benim ilk sayfam.";
        ViewBag.Tarih = DateTime.Now;
        ViewBag.Sayi = 50;

        return View();   // Views/Deneme/Merhaba.cshtml dosyasını arar
    }


}
