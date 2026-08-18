using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using OkulYonetim.Models;
using Microsoft.AspNetCore.Mvc.Rendering;   // ⭐ SelectList için
using OkulYonetim.Data;

namespace OkulYonetim.Controllers;

public class BolumController : Controller
{
    private readonly BolumRepository _bolumRepo;
    private readonly FakulteRepository _fakulteRepo;   // ⭐ İKİ repository!

    public BolumController(BolumRepository bolumRepo, FakulteRepository fakulteRepo)
    {
        _bolumRepo = bolumRepo;
        _fakulteRepo = fakulteRepo;
    }

    /// <summary>
    /// Fakülte açılır listesini hazırlar.
    /// Create ve Edit sayfalarında tekrar tekrar lazım olduğu için ayrı metot.
    /// </summary>
    private void FakulteListesiniHazirla(long? secili = null)
    {
        var fakulteler = _fakulteRepo.TumunuGetir();

        // SelectList: (kaynak liste, value alanı, görünen metin alanı, seçili değer)
        ViewBag.Fakulteler = new SelectList(fakulteler, "FakulteId", "FakulteAd", secili);
    }

    // GET: /Bolum
    public IActionResult Index()
    {
        return View(_bolumRepo.TumunuGetir());
    }

    // GET: /Bolum/Create
    public IActionResult Create()
    {
        FakulteListesiniHazirla();     // ⭐ Formu göstermeden önce listeyi hazırla
        return View();
    }

    // POST: /Bolum/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Create(Bolum bolum)
    {
        if (!ModelState.IsValid)
        {
            FakulteListesiniHazirla(bolum.FakulteId);   // ⭐ Hata varsa listeyi TEKRAR hazırla
            return View(bolum);
        }

        _bolumRepo.Ekle(bolum);
        TempData["Basarili"] = "Bölüm kaydedildi.";
        return RedirectToAction("Index");
    }

    // GET: /Bolum/Edit/5
    public IActionResult Edit(long id)
    {
        var bolum = _bolumRepo.IdIleGetir(id);
        if (bolum == null) return NotFound();

        FakulteListesiniHazirla(bolum.FakulteId);   // ⭐ Mevcut fakülte seçili gelsin
        return View(bolum);
    }

    // POST: /Bolum/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Edit(Bolum bolum)
    {
        if (!ModelState.IsValid)
        {
            FakulteListesiniHazirla(bolum.FakulteId);
            return View(bolum);
        }

        _bolumRepo.Guncelle(bolum);
        TempData["Basarili"] = "Bölüm güncellendi.";
        return RedirectToAction("Index");
    }

    // GET: /Bolum/Delete/5
    public IActionResult Delete(long id)
    {
        var bolum = _bolumRepo.IdIleGetir(id);
        if (bolum == null) return NotFound();
        return View(bolum);
    }

    // POST: /Bolum/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public IActionResult DeleteConfirmed(long id)
    {
        _bolumRepo.PasifYap(id);
        TempData["Basarili"] = "Bölüm silindi.";
        return RedirectToAction("Index");
    }

    public IActionResult Details(long id)
    {
        var bolum = _bolumRepo.IdIleGetir(id);
        if (bolum == null) return NotFound();
        return View(bolum);
    }
}
