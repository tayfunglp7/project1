using Microsoft.Data.SqlClient;   // ADO.NET sınıfları buradan geliyor
using OkulYonetim.Models;

namespace OkulYonetim.Data;

public class FakulteRepository
{
     // Bağlantı dizesini burada saklayacağız
    private readonly string _baglantiMetni;

    // Yapıcı metot (constructor):
    // ASP.NET Core, appsettings.json'ı okuyan "configuration" nesnesini // bize otomatik verir. 
    // Buna "Dependency Injection" denir (Adım 5'te açıklayacağız).
    public FakulteRepository(IConfiguration configuration)
    {
        _baglantiMetni = configuration.GetConnectionString("OkulDb")!;
    }
    
   /// <summary>
    /// Aktif tüm fakülteleri veritabanından okur ve liste olarak döner.
    /// </summary>
    public List<Fakulte> TumunuGetir()
    {
        // 1️⃣ Boş bir liste hazırla — verileri buraya dolduracağız
        List<Fakulte> liste = new List<Fakulte>();


        // 2️⃣ Çalıştıracağımız SQL sorgusu
        //    @"..." → çok satırlı metin yazmayı sağlar (verbatim string)
        string sql = @"SELECT fakulte_id, fakulte_ad, fakulte_adres,
                              fakulte_telefon, fakulte_eposta,
                              created_date, updated_date, is_active
                       FROM fakulte
                       WHERE is_active = '1'
                       ORDER BY fakulte_ad";
    }


}