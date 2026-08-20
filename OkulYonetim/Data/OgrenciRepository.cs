using Microsoft.Data.SqlClient;
using OkulYonetim.Models;

namespace OkulYonetim.Data;

public class OgrenciRepository
{
    private readonly string _baglantiMetni;
    public OgrenciRepository(IConfiguration configuration)
    {
        _baglantiMetni = configuration.GetConnectionString("OkulDb")!;
    }
    private Ogrenci SatiriNesneyeCevir(SqlDataReader okuyucu)
    {
        Ogrenci o = new Ogrenci();

        o.OgrenciId = okuyucu.GetInt64(okuyucu.GetOrdinal("ogrenci_id"));
        o.BolumId = okuyucu.GetInt64(okuyucu.GetOrdinal("bolum_id"));
        o.OgrenciAd = okuyucu.GetString(okuyucu.GetOrdinal("ogrenci_ad"));
        o.OgrenciSoyad = okuyucu.GetString(okuyucu.GetOrdinal("ogrenci_soyad"));

        // ⚠️ ogrenci_sinif veritabanında INT → GetInt32
        //    GetInt64 yazılırsa "Specified cast is not valid" hatası alınır.
        o.OgrenciSinif = okuyucu.GetInt32(okuyucu.GetOrdinal("ogrenci_sinif"));

        // DATE tipi C# tarafında yine DateTime olarak okunur
        o.OgrenciDogumTarihi = okuyucu.GetDateTime(okuyucu.GetOrdinal("ogrenci_dogum_tarihi"));

        o.OgrenciCinsiyet = okuyucu.GetString(okuyucu.GetOrdinal("ogrenci_cinsiyet"));
        o.OgrenciAdres = okuyucu.GetString(okuyucu.GetOrdinal("ogrenci_adres"));
        o.OgrenciTelefon = okuyucu.GetString(okuyucu.GetOrdinal("ogrenci_telefon"));
        o.OgrenciEposta = okuyucu.GetString(okuyucu.GetOrdinal("ogrenci_eposta"));
        o.OgrenciTc = okuyucu.GetString(okuyucu.GetOrdinal("ogrenci_tc"));
        o.CreatedDate = okuyucu.GetDateTime(okuyucu.GetOrdinal("created_date"));
        o.IsActive = okuyucu.GetString(okuyucu.GetOrdinal("is_active"));

        // updated_date NULL olabilir — önce kontrol et, yoksa uygulama çöker
        int sutunNo = okuyucu.GetOrdinal("updated_date");
        o.UpdatedDate = okuyucu.IsDBNull(sutunNo) ? null : okuyucu.GetDateTime(sutunNo);

        return o;
    }
    // ════════════════════════════════════════════════════════════════
    //  1) READ — Tüm aktif öğrenciler (bölüm adıyla birlikte)
    // ════════════════════════════════════════════════════════════════
    public List<Ogrenci> TumunuGetir()
    {
        List<Ogrenci> liste = new List<Ogrenci>();

        string sql = @"SELECT o.ogrenci_id, o.bolum_id, o.ogrenci_ad, o.ogrenci_soyad,
                              o.ogrenci_sinif, o.ogrenci_dogum_tarihi, o.ogrenci_cinsiyet,
                              o.ogrenci_adres, o.ogrenci_telefon, o.ogrenci_eposta,
                              o.ogrenci_tc, o.created_date, o.updated_date, o.is_active,
                              b.bolum_adi
                       FROM ogrenci o
                       INNER JOIN bolum b ON o.bolum_id = b.bolum_id
                       WHERE o.is_active = '1'
                       ORDER BY o.ogrenci_ad, o.ogrenci_soyad";

        using (SqlConnection baglanti = new SqlConnection(_baglantiMetni))
        using (SqlCommand komut = new SqlCommand(sql, baglanti))
        {
            baglanti.Open();

            using (SqlDataReader okuyucu = komut.ExecuteReader())
            {
                while (okuyucu.Read())
                {
                    Ogrenci o = SatiriNesneyeCevir(okuyucu);

                    // JOIN'den gelen ekstra sütun — modelde var, tabloda yok
                    o.BolumAdi = okuyucu.GetString(okuyucu.GetOrdinal("bolum_adi"));

                    liste.Add(o);
                }
            }
        }

        return liste;
    }
    // ════════════════════════════════════════════════════════════════
    //  2) READ — Tek öğrenci (düzenleme ve silme sayfaları için)
    // ════════════════════════════════════════════════════════════════
    public Ogrenci? IdIleGetir(long id)
    {
        Ogrenci? sonuc = null;

        // Not: Burada da JOIN kullanıyoruz ki Delete sayfasında
        // bölüm adını gösterebilelim.
        string sql = @"SELECT o.ogrenci_id, o.bolum_id, o.ogrenci_ad, o.ogrenci_soyad,
                              o.ogrenci_sinif, o.ogrenci_dogum_tarihi, o.ogrenci_cinsiyet,
                              o.ogrenci_adres, o.ogrenci_telefon, o.ogrenci_eposta,
                              o.ogrenci_tc, o.created_date, o.updated_date, o.is_active,
                              b.bolum_adi
                       FROM ogrenci o
                       INNER JOIN bolum b ON o.bolum_id = b.bolum_id
                       WHERE o.ogrenci_id = @id";

        using (SqlConnection baglanti = new SqlConnection(_baglantiMetni))
        using (SqlCommand komut = new SqlCommand(sql, baglanti))
        {
            // ⭐ Değer SQL metnine YAPIŞTIRILMAZ, parametre olarak verilir
            komut.Parameters.AddWithValue("@id", id);

            baglanti.Open();

            using (SqlDataReader okuyucu = komut.ExecuteReader())
            {
                // while değil if — tek satır bekliyoruz
                if (okuyucu.Read())
                {
                    sonuc = SatiriNesneyeCevir(okuyucu);
                    sonuc.BolumAdi = okuyucu.GetString(okuyucu.GetOrdinal("bolum_adi"));
                }
            }
        }

        return sonuc;   // Bulunamazsa null döner — controller bunu kontrol etmeli
    }
    // ════════════════════════════════════════════════════════════════
    //  3) CREATE — Yeni öğrenci ekle
    // ════════════════════════════════════════════════════════════════
    public void Ekle(Ogrenci ogrenci)
    {
        // DİKKAT: ogrenci_id yazılmaz (IDENTITY), bolum_adi hiç yazılmaz (tabloda yok)
        string sql = @"INSERT INTO ogrenci
                          (bolum_id, ogrenci_ad, ogrenci_soyad, ogrenci_sinif,
                           ogrenci_dogum_tarihi, ogrenci_cinsiyet, ogrenci_adres,
                           ogrenci_telefon, ogrenci_eposta, ogrenci_tc,
                           created_date, updated_date, is_active)
                       VALUES
                          (@bolumId, @ad, @soyad, @sinif,
                           @dogumTarihi, @cinsiyet, @adres,
                           @telefon, @eposta, @tc,
                           @createdDate, NULL, @isActive)";

        using (SqlConnection baglanti = new SqlConnection(_baglantiMetni))
        using (SqlCommand komut = new SqlCommand(sql, baglanti))
        {
            komut.Parameters.AddWithValue("@bolumId", ogrenci.BolumId);
            komut.Parameters.AddWithValue("@ad", ogrenci.OgrenciAd);
            komut.Parameters.AddWithValue("@soyad", ogrenci.OgrenciSoyad);
            komut.Parameters.AddWithValue("@sinif", ogrenci.OgrenciSinif);
            komut.Parameters.AddWithValue("@dogumTarihi", ogrenci.OgrenciDogumTarihi);
            komut.Parameters.AddWithValue("@cinsiyet", ogrenci.OgrenciCinsiyet);
            komut.Parameters.AddWithValue("@adres", ogrenci.OgrenciAdres);
            komut.Parameters.AddWithValue("@telefon", ogrenci.OgrenciTelefon);
            komut.Parameters.AddWithValue("@eposta", ogrenci.OgrenciEposta);
            komut.Parameters.AddWithValue("@tc", ogrenci.OgrenciTc);

            // ⭐ Tarihi ve aktiflik durumunu kullanıcıdan DEĞİL, sistemden alıyoruz
            komut.Parameters.AddWithValue("@createdDate", DateTime.Now);
            komut.Parameters.AddWithValue("@isActive", "1");

            baglanti.Open();

            // Veri dönmeyen komutlar için ExecuteNonQuery
            komut.ExecuteNonQuery();
        }

        // NOT: Benzersizlik ihlali (aynı e-posta / telefon / TC) burada
        // SqlException fırlatır. Onu Controller'daki try-catch yakalar.
        // Bkz. 08-ogrenci-crud.md → "Veritabanı hatalarını yakalamak"
    }
    // ════════════════════════════════════════════════════════════════
    //  4) UPDATE — Öğrenciyi güncelle
    // ════════════════════════════════════════════════════════════════
    public void Guncelle(Ogrenci ogrenci)
    {
        // ⚠️ WHERE satırını UNUTMA! Yoksa tablodaki TÜM öğrenciler
        //    aynı kişiye dönüşür.
        string sql = @"UPDATE ogrenci
                       SET bolum_id             = @bolumId,
                           ogrenci_ad           = @ad,
                           ogrenci_soyad        = @soyad,
                           ogrenci_sinif        = @sinif,
                           ogrenci_dogum_tarihi = @dogumTarihi,
                           ogrenci_cinsiyet     = @cinsiyet,
                           ogrenci_adres        = @adres,
                           ogrenci_telefon      = @telefon,
                           ogrenci_eposta       = @eposta,
                           ogrenci_tc           = @tc,
                           updated_date         = @updatedDate
                       WHERE ogrenci_id         = @id";

        using (SqlConnection baglanti = new SqlConnection(_baglantiMetni))
        using (SqlCommand komut = new SqlCommand(sql, baglanti))
        {
            komut.Parameters.AddWithValue("@bolumId", ogrenci.BolumId);
            komut.Parameters.AddWithValue("@ad", ogrenci.OgrenciAd);
            komut.Parameters.AddWithValue("@soyad", ogrenci.OgrenciSoyad);
            komut.Parameters.AddWithValue("@sinif", ogrenci.OgrenciSinif);
            komut.Parameters.AddWithValue("@dogumTarihi", ogrenci.OgrenciDogumTarihi);
            komut.Parameters.AddWithValue("@cinsiyet", ogrenci.OgrenciCinsiyet);
            komut.Parameters.AddWithValue("@adres", ogrenci.OgrenciAdres);
            komut.Parameters.AddWithValue("@telefon", ogrenci.OgrenciTelefon);
            komut.Parameters.AddWithValue("@eposta", ogrenci.OgrenciEposta);
            komut.Parameters.AddWithValue("@tc", ogrenci.OgrenciTc);
            komut.Parameters.AddWithValue("@updatedDate", DateTime.Now);
            komut.Parameters.AddWithValue("@id", ogrenci.OgrenciId);

            baglanti.Open();
            komut.ExecuteNonQuery();
        }

        // NOT: created_date'e hiç dokunmuyoruz — kayıt tarihi değişmemeli.
        // Bu yüzden Edit.cshtml'deki gizli CreatedDate alanı aslında
        // zorunlu değil; ama model bütünlüğü için bırakmak zararsız.
    }
    // ════════════════════════════════════════════════════════════════
    //  5) DELETE — Yumuşak silme (soft delete)
    // ════════════════════════════════════════════════════════════════
    public void PasifYap(long id)
    {
        // Kayıt SİLİNMİYOR, sadece pasif işaretleniyor.
        // TumunuGetir() içindeki WHERE is_active = '1' filtresi
        // bu kaydı listeden gizler.
        string sql = @"UPDATE ogrenci
                       SET is_active    = '0',
                           updated_date = @updatedDate
                       WHERE ogrenci_id = @id";

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