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

    // ════════════════════════════════════════════════════════════════
    //  YARDIMCI: Bölüm açılır listesini hazırlar
    //
    //  Create ve Edit sayfalarında defalarca lazım olduğu için
    //  ayrı metoda alındı.
    // ════════════════════════════════════════════════════════════════
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


    // ════════════════════════════════════════════════════════════════
    //  3) YENİ KAYDI KAYDET
    //  POST: /Ogrenci/Create
    // ════════════════════════════════════════════════════════════════
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Create(Ogrenci ogrenci)
    {
        // Model'deki kurallara (Required, EmailAddress, Range, Regex...) uyuyor mu?
        // Tarayıcı doğrulaması kandırılabilir, bu yüzden sunucuda TEKRAR bakıyoruz.
        if (!ModelState.IsValid)
        {
            // ⭐ EN ÇOK UNUTULAN SATIR
            // ViewBag sadece o istek boyunca yaşar. POST yeni bir istektir,
            // önceki ViewBag yok olmuştur. Yeniden doldurmazsak açılır liste
            // boş gelir ve sayfa NullReferenceException ile çöker.
            BolumListesiniHazirla(ogrenci.BolumId);
            return View(ogrenci);   // Kullanıcının yazdıkları kaybolmasın
        }

        try
        {
            _ogrenciRepo.Ekle(ogrenci);

            TempData["Basarili"] = $"{ogrenci.TamAd} kaydedildi.";

            // POST-Redirect-GET: yönlendirme yapmazsak kullanıcı F5'e
            // bastığında aynı kayıt ikinci kez eklenir.
            return RedirectToAction("Index");
        }
        catch (SqlException ex)
        {
            BenzersizlikHatasiniIsle(ex);
            BolumListesiniHazirla(ogrenci.BolumId);
            return View(ogrenci);
        }
    }


    // ════════════════════════════════════════════════════════════════
    //  4) DÜZENLEME FORMU (dolu form göster)
    //  GET: /Ogrenci/Edit/5
    // ════════════════════════════════════════════════════════════════
    public IActionResult Edit(long id)
    {
        Ogrenci? ogrenci = _ogrenciRepo.IdIleGetir(id);

        // Repository null dönebilir — kontrol etmek ZORUNDAYIZ.
        // Kullanıcı adres çubuğuna /Ogrenci/Edit/99999 yazabilir.
        if (ogrenci == null)
        {
            return NotFound();
        }

        BolumListesiniHazirla(ogrenci.BolumId);   // mevcut bölüm seçili gelsin
        return View(ogrenci);
    }


    // ════════════════════════════════════════════════════════════════
    //  5) DÜZENLEMEYİ KAYDET
    //  POST: /Ogrenci/Edit/5
    // ════════════════════════════════════════════════════════════════
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Edit(Ogrenci ogrenci)
    {
        if (!ModelState.IsValid)
        {
            BolumListesiniHazirla(ogrenci.BolumId);
            return View(ogrenci);
        }

        try
        {
            _ogrenciRepo.Guncelle(ogrenci);

            TempData["Basarili"] = $"{ogrenci.TamAd} güncellendi.";
            return RedirectToAction("Index");
        }
        catch (SqlException ex)
        {
            // NOT: Öğrenci kendi e-postasını değiştirmeden kaydederse
            // hata ALMAZ. Çünkü UNIQUE kısıtı, satırın kendi mevcut
            // değeriyle güncellenmesine izin verir.
            // (08-ogrenci-crud.md alıştırma B'nin cevabı budur.)
            BenzersizlikHatasiniIsle(ex);
            BolumListesiniHazirla(ogrenci.BolumId);
            return View(ogrenci);
        }
    }

    // ════════════════════════════════════════════════════════════════
    //  6) SİLME ONAY SAYFASI
    //  GET: /Ogrenci/Delete/5
    // ════════════════════════════════════════════════════════════════
    public IActionResult Delete(long id)
    {
        Ogrenci? ogrenci = _ogrenciRepo.IdIleGetir(id);

        if (ogrenci == null)
        {
            return NotFound();
        }

        return View(ogrenci);
    }


    // ════════════════════════════════════════════════════════════════
    //  7) SİLMEYİ ONAYLA
    //  POST: /Ogrenci/Delete/5
    //
    //  Metot adı DeleteConfirmed çünkü C#'ta aynı isim + aynı imza ile
    //  iki metot olamaz. ActionName ile adresin yine /Delete olmasını
    //  sağlıyoruz.
    // ════════════════════════════════════════════════════════════════
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public IActionResult DeleteConfirmed(long id)
    {
        // Gerçekten silmiyoruz, is_active = '0' yapıyoruz (soft delete)
        _ogrenciRepo.PasifYap(id);

        TempData["Basarili"] = "Öğrenci silindi.";
        return RedirectToAction("Index");
    }

    public IActionResult Details(long id)
    {
        Ogrenci? ogrenci = _ogrenciRepo.IdIleGetir(id);

        if (ogrenci == null)
        {
            return NotFound();
        }

        return View(ogrenci);
    }








    // ════════════════════════════════════════════════════════════════
    //  YARDIMCI: SqlException'ı kullanıcı dostu mesaja çevirir
    //
    //  Create ve Edit'te aynı mantık tekrar ettiği için ayrıldı.
    //  ⚠️ ex.Message'ı OLDUĞU GİBİ kullanıcıya GÖSTERME!
    //     Tablo/sütun adlarını ifşa eder, saldırgana yol gösterir.
    // ════════════════════════════════════════════════════════════════
    private void BenzersizlikHatasiniIsle(SqlException ex)
    {
        // 2627 = UNIQUE KEY ihlali, 2601 = UNIQUE INDEX ihlali
        if (ex.Number == 2627 || ex.Number == 2601)
        {
            if (ex.Message.Contains("eposta"))
            {
                ModelState.AddModelError("OgrenciEposta",
                    "Bu e-posta adresi başka bir öğrenciye kayıtlı.");
            }
            else if (ex.Message.Contains("telefon"))
            {
                ModelState.AddModelError("OgrenciTelefon",
                    "Bu telefon numarası başka bir öğrenciye kayıtlı.");
            }
            else if (ex.Message.Contains("tc"))
            {
                ModelState.AddModelError("OgrenciTc",
                    "Bu TC kimlik numarası başka bir öğrenciye kayıtlı.");
            }
            else
            {
                ModelState.AddModelError("", "Bu kayıt zaten mevcut.");
            }
        }
        else
        {
            // Beklenmedik veritabanı hatası — detay verme
            ModelState.AddModelError("",
                "Kayıt sırasında bir sorun oluştu. Lütfen tekrar deneyin.");
        }
    }


}
