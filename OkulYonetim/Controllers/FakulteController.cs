using Microsoft.AspNetCore.Mvc;
using OkulYonetim.Data;
using OkulYonetim.Models;

namespace OkulYonetim.Controllers;

public class FakulteController : Controller
{
    private readonly FakulteRepository _repo;

    public FakulteController(FakulteRepository repo)
    {
        _repo = repo;
    }

    // ═══════════════════════════════════════════
    // 1) LİSTELEME
    // GET: /Fakulte
    // ═══════════════════════════════════════════
    public IActionResult Index()
    {
        var fakulteler = _repo.TumunuGetir();
        return View(fakulteler);
    }

    // ═══════════════════════════════════════════
    // 2) YENİ KAYIT FORMU (boş form göster)
    // GET: /Fakulte/Create
    // ═══════════════════════════════════════════
    public IActionResult Create()
    {
        return View();
    }

    // ═══════════════════════════════════════════
    // 3) YENİ KAYDI KAYDET
    // POST: /Fakulte/Create
    // ═══════════════════════════════════════════
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Create(Fakulte fakulte)
    {
        // Model'deki kurallara uyuyor mu?
        if (!ModelState.IsValid)
        {
            // Uymuyorsa formu geri göster — kullanıcının yazdıkları kaybolmasın
            return View(fakulte);
        }

        _repo.Ekle(fakulte);

        // Başarı mesajı (bir sonraki sayfada gösterilecek)
        TempData["Basarili"] = "Fakülte kaydedildi.";

        // Listeye geri dön
        return RedirectToAction("Index");
    }

    // ═══════════════════════════════════════════
    // 4) DÜZENLEME FORMU (dolu form göster)
    // GET: /Fakulte/Edit/5
    // ═══════════════════════════════════════════
    public IActionResult Edit(long id)
    {
        var fakulte = _repo.IdIleGetir(id);

        if (fakulte == null)
        {
            return NotFound();   // 404 sayfası
        }

        return View(fakulte);
    }

    // ═══════════════════════════════════════════
    // 5) DÜZENLEMEYİ KAYDET
    // POST: /Fakulte/Edit/5
    // ═══════════════════════════════════════════
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Edit(Fakulte fakulte)
    {
        if (!ModelState.IsValid)
        {
            return View(fakulte);
        }

        _repo.Guncelle(fakulte);
        TempData["Basarili"] = "Fakülte güncellendi.";
        return RedirectToAction("Index");
    }

    // ═══════════════════════════════════════════
    // 6) SİLME ONAY SAYFASI
    // GET: /Fakulte/Delete/5
    // ═══════════════════════════════════════════
    public IActionResult Delete(long id)
    {
        var fakulte = _repo.IdIleGetir(id);

        if (fakulte == null)
        {
            return NotFound();
        }

        return View(fakulte);
    }

    // ═══════════════════════════════════════════
    // 7) SİLMEYİ ONAYLA
    // POST: /Fakulte/Delete/5
    // ═══════════════════════════════════════════
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public IActionResult DeleteConfirmed(long id)
    {
        _repo.PasifYap(id);
        TempData["Basarili"] = "Fakülte silindi.";
        return RedirectToAction("Index");
    }

    // ═══════════════════════════════════════════
    // 8) DETAY FORMU (dolu form göster)
    // GET: /Fakulte/Details/5
    // ═══════════════════════════════════════════
    public IActionResult Details(long id)
    {
        var fakulte = _repo.IdIleGetir(id);

        if (fakulte == null)
        {
            return NotFound();
        }

        return View(fakulte);
    }
}