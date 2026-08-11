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
                       ORDER BY created_date asc";

        // 3️⃣ Bağlantıyı aç
        // using -> iş bitince bağlantıyı OTOMATİK kapatır. Çok önemli!
        using (SqlConnection baglanti = new SqlConnection(_baglantiMetni))
        {
            // 4️⃣ Komutu hazırla: hangi SQL, hangi bağlantı üzerinden
            using (SqlCommand komut = new SqlCommand(sql, baglanti))
            {

                baglanti.Open(); // 5️⃣ Bağlantıyı fiilen aç
                // 6️⃣ Sorguyu çalıştır ve okuyucuyu al
                using (SqlDataReader okuyucu = komut.ExecuteReader())
                {
                    // 7️⃣ Satır satır oku
                    // Read() -> sıradaki satıra geç. Satır kalmadıysa false döner.
                    while (okuyucu.Read())
                    {

                        Fakulte f = new Fakulte();
                        // 8️⃣ Sütunları C# nesnesine kopyala
                        f.FakulteId = okuyucu.GetInt64(okuyucu.GetOrdinal("fakulte_id"));
                        f.FakulteAd = okuyucu.GetString(okuyucu.GetOrdinal("fakulte_ad"));
                        f.FakulteAdres = okuyucu.GetString(okuyucu.GetOrdinal("fakulte_adres"));
                        f.FakulteTelefon = okuyucu.GetString(okuyucu.GetOrdinal("fakulte_telefon"));
                        f.FakulteEposta = okuyucu.GetString(okuyucu.GetOrdinal("fakulte_eposta"));
                        f.CreatedDate = okuyucu.GetDateTime(okuyucu.GetOrdinal("created_date"));
                        f.IsActive = okuyucu.GetString(okuyucu.GetOrdinal("is_active"));

                        // updated_date NULL olabilir - önce kontrol et!
                        int sutunNo = okuyucu.GetOrdinal("updated_date");
                        if (okuyucu.IsDBNull(sutunNo))
                            f.UpdatedDate = null;
                        else
                            f.UpdatedDate = okuyucu.GetDateTime(sutunNo);

                        // 9️⃣ Nesneyi listeye ekle
                        liste.Add(f);
                    }

                }

            }
        }
        // 🔟 using blokları biter -> bağlantı otomatik kapanır
        return liste;
    }
}