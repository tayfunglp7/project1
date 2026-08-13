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
    /// SqlDataReader'dan gelen bir satırı Fakulte nesnesine çevirir.
    /// Aynı kodu her metotta tekrar yazmamak için ayrı metot yaptık.
    /// </summary>
    private Fakulte SatiriNesneyeCevir(SqlDataReader okuyucu)
    {
        Fakulte f = new Fakulte();

        f.FakulteId = okuyucu.GetInt64(okuyucu.GetOrdinal("fakulte_id"));
        f.FakulteAd = okuyucu.GetString(okuyucu.GetOrdinal("fakulte_ad"));
        f.FakulteAdres = okuyucu.GetString(okuyucu.GetOrdinal("fakulte_adres"));
        f.FakulteTelefon = okuyucu.GetString(okuyucu.GetOrdinal("fakulte_telefon"));
        f.FakulteEposta = okuyucu.GetString(okuyucu.GetOrdinal("fakulte_eposta"));
        f.CreatedDate = okuyucu.GetDateTime(okuyucu.GetOrdinal("created_date"));
        f.IsActive = okuyucu.GetString(okuyucu.GetOrdinal("is_active"));

        int sutunNo = okuyucu.GetOrdinal("updated_date");
        f.UpdatedDate = okuyucu.IsDBNull(sutunNo) ? null : okuyucu.GetDateTime(sutunNo);

        return f;
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
                        // 9️⃣ Nesneyi listeye ekle
                        liste.Add(SatiriNesneyeCevir(okuyucu));
                    }

                }

            }
        }
        // 🔟 using blokları biter -> bağlantı otomatik kapanır
        return liste;
    }

    /// <summary>
    /// Verilen id'ye sahip fakülteyi getirir. Bulunamazsa null döner.
    /// </summary>
    public Fakulte? IdIleGetir(long id)
    {
        Fakulte? sonuc = null;

        string sql = @"SELECT fakulte_id, fakulte_ad, fakulte_adres,
                              fakulte_telefon, fakulte_eposta,
                              created_date, updated_date, is_active
                       FROM fakulte
                       WHERE fakulte_id = @id";

        using (SqlConnection baglanti = new SqlConnection(_baglantiMetni))
        using (SqlCommand komut = new SqlCommand(sql, baglanti))
        {
            // ⭐ Parametre: değeri SQL metnine YAPIŞTIRMIYORUZ
            komut.Parameters.AddWithValue("@id", id);

            baglanti.Open();
            using (SqlDataReader okuyucu = komut.ExecuteReader())
            {
                if (okuyucu.Read())          // while değil, if — tek satır bekliyoruz
                {
                    sonuc = SatiriNesneyeCevir(okuyucu);
                }
            }
        }

        return sonuc;
    }

    /// <summary>
    /// Yeni fakülte ekler.
    /// </summary>
    public void Ekle(Fakulte fakulte)
    {
        // DİKKAT: fakulte_id yazmıyoruz! IDENTITY olduğu için SQL Server kendi veriyor.
        string sql = @"INSERT INTO fakulte
                          (fakulte_ad, fakulte_adres, fakulte_telefon,
                           fakulte_eposta, created_date, updated_date, is_active)
                       VALUES
                          (@ad, @adres, @telefon, @eposta, @createdDate, NULL, @isActive)";

        using (SqlConnection baglanti = new SqlConnection(_baglantiMetni))
        using (SqlCommand komut = new SqlCommand(sql, baglanti))
        {
            komut.Parameters.AddWithValue("@ad", fakulte.FakulteAd);
            komut.Parameters.AddWithValue("@adres", fakulte.FakulteAdres);
            komut.Parameters.AddWithValue("@telefon", fakulte.FakulteTelefon);
            komut.Parameters.AddWithValue("@eposta", fakulte.FakulteEposta);
            komut.Parameters.AddWithValue("@createdDate", DateTime.Now);  // ⭐ tarihi biz veriyoruz
            komut.Parameters.AddWithValue("@isActive", "1");           // ⭐ yeni kayıt aktiftir

            baglanti.Open();
            komut.ExecuteNonQuery();   // Veri dönmeyen komutlar için: INSERT, UPDATE, DELETE
        }
    }

    /// <summary>
    /// Var olan fakülteyi günceller.
    /// </summary>
    public void Guncelle(Fakulte fakulte)
    {
        string sql = @"UPDATE fakulte
                       SET fakulte_ad      = @ad,
                           fakulte_adres   = @adres,
                           fakulte_telefon = @telefon,
                           fakulte_eposta  = @eposta,
                           updated_date    = @updatedDate
                       WHERE fakulte_id    = @id";

        using (SqlConnection baglanti = new SqlConnection(_baglantiMetni))
        using (SqlCommand komut = new SqlCommand(sql, baglanti))
        {
            komut.Parameters.AddWithValue("@ad", fakulte.FakulteAd);
            komut.Parameters.AddWithValue("@adres", fakulte.FakulteAdres);
            komut.Parameters.AddWithValue("@telefon", fakulte.FakulteTelefon);
            komut.Parameters.AddWithValue("@eposta", fakulte.FakulteEposta);
            komut.Parameters.AddWithValue("@updatedDate", DateTime.Now);
            komut.Parameters.AddWithValue("@id", fakulte.FakulteId);

            baglanti.Open();
            komut.ExecuteNonQuery();
        }
    }

    /// <summary>
    /// Kaydı gerçekten silmez, pasif duruma alır (soft delete).
    /// </summary>
    public void PasifYap(long id)
    {
        string sql = @"UPDATE fakulte
                       SET is_active = '0',
                           updated_date = @updatedDate
                       WHERE fakulte_id = @id";

        using (SqlConnection baglanti = new SqlConnection(_baglantiMetni))
        using (SqlCommand komut = new SqlCommand(sql, baglanti))
        {
            komut.Parameters.AddWithValue("@updatedDate", DateTime.Now);
            komut.Parameters.AddWithValue("@id", id);

            baglanti.Open();
            komut.ExecuteNonQuery();
        }
    }
}