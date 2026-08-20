using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;   // SelectList için
using Microsoft.Data.SqlClient;             // SqlException yakalamak için
using OkulYonetim.Data;
using OkulYonetim.Models;

public class OgrenciController : Controller
{
    private readonly OgrenciRepository _ogrenciRepo;
    private readonly BolumRepository _bolumRepo;

    public OgrenciController(OgrenciRepository ogrenciRepo, BolumRepository bolumRepo)
    {
        _ogrenciRepo = ogrenciRepo;
        _bolumRepo = bolumRepo;
    }

    private void BolumListesiniHazirla(long? secili = null)
    {
        var bolumler = _bolumRepo.TumunuGetir();

        // Açılır listede "Bilgisayar Mühendisliği (Mühendislik Fakültesi)"
        // şeklinde göstermek için fakülte adını da ekliyoruz.
        // Aynı adlı bölüm farklı fakültelerde varsa karışıklığı önler.
        var liste = bolumler.Select(b => new
        {
            Id = b.BolumId,
            Ad = b.BolumAdi + " (" + b.FakulteAd + ")"
        }).ToList();

        ViewBag.Bolumler = new SelectList(liste, "Id", "Ad", secili);
    }

    // ════════════════════════════════════════════════════════════════
    //  1) LİSTELEME
    //  GET: /Ogrenci
    // ════════════════════════════════════════════════════════════════
    public IActionResult Index()
    {
        List<Ogrenci> ogrenciler = _ogrenciRepo.TumunuGetir();
        return View(ogrenciler);
    }

    // ════════════════════════════════════════════════════════════════
    //  2) YENİ KAYIT FORMU (boş form göster)
    //  GET: /Ogrenci/Create
    // ════════════════════════════════════════════════════════════════
    public IActionResult Create()
    {
        BolumListesiniHazirla();
        return View();
    }

}
