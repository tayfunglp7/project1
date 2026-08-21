using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;   // SelectList için
using Microsoft.Data.SqlClient;             // SqlException yakalamak için
using OkulYonetim.Data;
using OkulYonetim.Models;

namespace OkulYonetim.Controllers;

/// <summary>
/// akademisyen tablosunun CRUD işlemleri.
///

///

public class AkademisyenController : Controller
{
    private readonly AkademisyenRepository _akademisyenRepo;
    private readonly BolumRepository _bolumRepo;

    // İKİ repository: biri akademisyen için, biri açılır liste için
    public AkademisyenController(AkademisyenRepository akademisyenRepo, BolumRepository bolumRepo)
    {
        _akademisyenRepo = akademisyenRepo;
        _bolumRepo = bolumRepo;
    }

    // ════════════════════════════════════════════════════════════════
    //  YARDIMCI: Bölüm açılır listesini hazırlar
    // ════════════════════════════════════════════════════════════════
    private void BolumListesiniHazirla(long? secili = null)
    {
        var bolumler = _bolumRepo.TumunuGetir();

        // "Bilgisayar Mühendisliği (Mühendislik Fakültesi)" şeklinde gösteriyoruz.
        // Aynı adlı bölüm farklı fakültelerde varsa karışıklığı önler.
        var liste = bolumler.Select(b => new
        {
            Id = b.BolumId,
            Ad = b.BolumAdi + " (" + b.FakulteAd + ")"
        }).ToList();

        ViewBag.Bolumler = new SelectList(liste, "Id", "Ad", secili);
    }


    // ════════════════════════════════════════════════════════════════
    //  YARDIMCI: SqlException'ı kullanıcı dostu mesaja çevirir
    //
    //  ⚠️ ex.Message'ı OLDUĞU GİBİ kullanıcıya GÖSTERME!
    //     Tablo ve sütun adlarını ifşa eder, saldırgana yol gösterir.
    // ════════════════════════════════════════════════════════════════
    private void BenzersizlikHatasiniIsle(SqlException ex)
    {
        // 2627 = UNIQUE KEY ihlali, 2601 = UNIQUE INDEX ihlali
        if (ex.Number == 2627 || ex.Number == 2601)
        {
            if (ex.Message.Contains("eposta"))
            {
                ModelState.AddModelError("AkademisyenEposta",
                    "Bu e-posta adresi başka bir akademisyene kayıtlı.");
            }
            else if (ex.Message.Contains("telefon"))
            {
                ModelState.AddModelError("AkademisyenTelefon",
                    "Bu telefon numarası başka bir akademisyene kayıtlı.");
            }
            else if (ex.Message.Contains("tc"))
            {
                ModelState.AddModelError("AkademisyenTc",
                    "Bu TC kimlik numarası başka bir akademisyene kayıtlı.");
            }
            else
            {
                ModelState.AddModelError("", "Bu kayıt zaten mevcut.");
            }
        }
        else
        {
            // Beklenmedik veritabanı hatası — teknik detay verme
            ModelState.AddModelError("",
                "Kayıt sırasında bir sorun oluştu. Lütfen tekrar deneyin.");
        }
    }

    // ════════════════════════════════════════════════════════════════
    //  1) LİSTELEME
    //  GET: /Akademisyen
    // ════════════════════════════════════════════════════════════════
    public IActionResult Index()
    {
        List<Akademisyen> akademisyenler = _akademisyenRepo.TumunuGetir();
        return View(akademisyenler);
    }

    // ════════════════════════════════════════════════════════════════
    //  2) YENİ KAYIT FORMU (boş form göster)
    //  GET: /Akademisyen/Create
    // ════════════════════════════════════════════════════════════════
    public IActionResult Create()
    {
        BolumListesiniHazirla();
        return View();
    }

    // ════════════════════════════════════════════════════════════════
    //  3) YENİ KAYDI KAYDET
    //  POST: /Akademisyen/Create
    // ════════════════════════════════════════════════════════════════
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Create(Akademisyen akademisyen)
    {
        // Tarayıcı doğrulaması F12 ile kandırılabilir.
        // Bu yüzden sunucuda TEKRAR kontrol ediyoruz.
        if (!ModelState.IsValid)
        {
            // ⭐ EN ÇOK UNUTULAN SATIR
            // ViewBag sadece o istek boyunca yaşar. POST yeni bir istektir,
            // önceki ViewBag yok olmuştur. Doldurmazsak açılır liste boş gelir
            // ve sayfa NullReferenceException ile çöker.
            BolumListesiniHazirla(akademisyen.BolumId);
            return View(akademisyen);   // Kullanıcının yazdıkları kaybolmasın
        }

        try
        {
            _akademisyenRepo.Ekle(akademisyen);

            TempData["Basarili"] = $"{akademisyen.TamAd} kaydedildi.";

            // POST-Redirect-GET: yönlendirme yapmazsak kullanıcı F5'e
            // bastığında aynı kayıt ikinci kez eklenir.
            return RedirectToAction("Index");
        }
        catch (SqlException ex)
        {
            BenzersizlikHatasiniIsle(ex);
            BolumListesiniHazirla(akademisyen.BolumId);
            return View(akademisyen);
        }
    }

}