
using Microsoft.Data.SqlClient;
using OkulYonetim.Models;

namespace OkulYonetim.Data;

/// <summary>
/// akademisyen tablosunun veritabanı işlemleri.
///

///
/// OgrenciRepository ile farkları:
///   1) ogrenci_sinif alanı YOK  →  bir parametre eksik
///   2) JOIN'e fakulte tablosu da eklendi (fakülte adını da gösteriyoruz)
///   3) Geri kalan her şey birebir aynı
/// </summary>
public class AkademisyenRepository
{
    private readonly string _baglantiMetni;
    public AkademisyenRepository(IConfiguration configuration)
    {
        _baglantiMetni = configuration.GetConnectionString("OkulDb")!;
    }

    // ════════════════════════════════════════════════════════════════
    //  YARDIMCI: Okuyucudan gelen satırı nesneye çevirir
    // ════════════════════════════════════════════════════════════════
    private Akademisyen SatiriNesneyeCevir(SqlDataReader okuyucu)
    {
        Akademisyen a = new Akademisyen();

        a.AkademisyenId = okuyucu.GetInt64(okuyucu.GetOrdinal("akademisyen_id"));
        a.BolumId = okuyucu.GetInt64(okuyucu.GetOrdinal("bolum_id"));
        a.AkademisyenAd = okuyucu.GetString(okuyucu.GetOrdinal("akademisyen_ad"));
        a.AkademisyenSoyad = okuyucu.GetString(okuyucu.GetOrdinal("akademisyen_soyad"));

        // DATE tipi C# tarafında DateTime olarak okunur (saat kısmı 00:00 gelir)
        a.AkademisyenDogumTarihi = okuyucu.GetDateTime(okuyucu.GetOrdinal("akademisyen_dogum_tarihi"));

        a.AkademisyenCinsiyet = okuyucu.GetString(okuyucu.GetOrdinal("akademisyen_cinsiyet"));
        a.AkademisyenAdres = okuyucu.GetString(okuyucu.GetOrdinal("akademisyen_adres"));
        a.AkademisyenTelefon = okuyucu.GetString(okuyucu.GetOrdinal("akademisyen_telefon"));
        a.AkademisyenEposta = okuyucu.GetString(okuyucu.GetOrdinal("akademisyen_eposta"));
        a.AkademisyenTc = okuyucu.GetString(okuyucu.GetOrdinal("akademisyen_tc"));
        a.CreatedDate = okuyucu.GetDateTime(okuyucu.GetOrdinal("created_date"));
        a.IsActive = okuyucu.GetString(okuyucu.GetOrdinal("is_active"));

        // updated_date NULL olabilir — kontrolsüz okursak uygulama çöker
        int sutunNo = okuyucu.GetOrdinal("updated_date");
        a.UpdatedDate = okuyucu.IsDBNull(sutunNo) ? null : okuyucu.GetDateTime(sutunNo);

        return a;
    }


    // ════════════════════════════════════════════════════════════════
    //  1) READ — Tüm aktif akademisyenler
    //
    //  İKİ JOIN var: akademisyen → bolum → fakulte
    //  Böylece hem bölüm hem fakülte adını gösterebiliyoruz.
    // ════════════════════════════════════════════════════════════════
    public List<Akademisyen> TumunuGetir()
    {
        List<Akademisyen> liste = new List<Akademisyen>();

        string sql = @"SELECT a.akademisyen_id, a.bolum_id,
                              a.akademisyen_ad, a.akademisyen_soyad,
                              a.akademisyen_dogum_tarihi, a.akademisyen_cinsiyet,
                              a.akademisyen_adres, a.akademisyen_telefon,
                              a.akademisyen_eposta, a.akademisyen_tc,
                              a.created_date, a.updated_date, a.is_active,
                              b.bolum_adi,
                              f.fakulte_ad
                       FROM akademisyen a
                       INNER JOIN bolum   b ON a.bolum_id   = b.bolum_id
                       INNER JOIN fakulte f ON b.fakulte_id = f.fakulte_id
                       WHERE a.is_active = '1'
                       ORDER BY a.akademisyen_ad, a.akademisyen_soyad";

        using (SqlConnection baglanti = new SqlConnection(_baglantiMetni))
        using (SqlCommand komut = new SqlCommand(sql, baglanti))
        {
            baglanti.Open();

            using (SqlDataReader okuyucu = komut.ExecuteReader())
            {
                while (okuyucu.Read())
                {
                    Akademisyen a = SatiriNesneyeCevir(okuyucu);

                    // JOIN'den gelen ekstra sütunlar — modelde var, tabloda yok
                    a.BolumAdi = okuyucu.GetString(okuyucu.GetOrdinal("bolum_adi"));
                    a.FakulteAd = okuyucu.GetString(okuyucu.GetOrdinal("fakulte_ad"));

                    liste.Add(a);
                }
            }
        }

        return liste;
    }

    // ════════════════════════════════════════════════════════════════
    //  2) READ — Tek akademisyen (Edit / Delete / Details için)
    // ════════════════════════════════════════════════════════════════
    public Akademisyen? IdIleGetir(long id)
    {
        Akademisyen? sonuc = null;

        string sql = @"SELECT a.akademisyen_id, a.bolum_id,
                              a.akademisyen_ad, a.akademisyen_soyad,
                              a.akademisyen_dogum_tarihi, a.akademisyen_cinsiyet,
                              a.akademisyen_adres, a.akademisyen_telefon,
                              a.akademisyen_eposta, a.akademisyen_tc,
                              a.created_date, a.updated_date, a.is_active,
                              b.bolum_adi,
                              f.fakulte_ad
                       FROM akademisyen a
                       INNER JOIN bolum   b ON a.bolum_id   = b.bolum_id
                       INNER JOIN fakulte f ON b.fakulte_id = f.fakulte_id
                       WHERE a.akademisyen_id = @id";

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
                    sonuc.FakulteAd = okuyucu.GetString(okuyucu.GetOrdinal("fakulte_ad"));
                }
            }
        }

        return sonuc;   // Bulunamazsa null — controller kontrol etmeli
    }


    // ════════════════════════════════════════════════════════════════
    //  3) CREATE — Yeni akademisyen ekle
    // ════════════════════════════════════════════════════════════════
    public void Ekle(Akademisyen akademisyen)
    {
        // akademisyen_id yazılmaz (IDENTITY)
        // bolum_adi / fakulte_ad hiç yazılmaz (tabloda yoklar)
        string sql = @"INSERT INTO akademisyen
                          (bolum_id, akademisyen_ad, akademisyen_soyad,
                           akademisyen_dogum_tarihi, akademisyen_cinsiyet,
                           akademisyen_adres, akademisyen_telefon,
                           akademisyen_eposta, akademisyen_tc,
                           created_date, updated_date, is_active)
                       VALUES
                          (@bolumId, @ad, @soyad,
                           @dogumTarihi, @cinsiyet,
                           @adres, @telefon,
                           @eposta, @tc,
                           @createdDate, NULL, @isActive)";

        using (SqlConnection baglanti = new SqlConnection(_baglantiMetni))
        using (SqlCommand komut = new SqlCommand(sql, baglanti))
        {
            komut.Parameters.AddWithValue("@bolumId", akademisyen.BolumId);
            komut.Parameters.AddWithValue("@ad", akademisyen.AkademisyenAd);
            komut.Parameters.AddWithValue("@soyad", akademisyen.AkademisyenSoyad);
            komut.Parameters.AddWithValue("@dogumTarihi", akademisyen.AkademisyenDogumTarihi);
            komut.Parameters.AddWithValue("@cinsiyet", akademisyen.AkademisyenCinsiyet);
            komut.Parameters.AddWithValue("@adres", akademisyen.AkademisyenAdres);
            komut.Parameters.AddWithValue("@telefon", akademisyen.AkademisyenTelefon);
            komut.Parameters.AddWithValue("@eposta", akademisyen.AkademisyenEposta);
            komut.Parameters.AddWithValue("@tc", akademisyen.AkademisyenTc);

            // ⭐ Tarih ve aktiflik kullanıcıdan DEĞİL, sistemden geliyor
            komut.Parameters.AddWithValue("@createdDate", DateTime.Now);
            komut.Parameters.AddWithValue("@isActive", "1");

            baglanti.Open();
            komut.ExecuteNonQuery();   // Veri dönmeyen komutlar için
        }

        // Benzersizlik ihlali (aynı e-posta / telefon / TC) burada
        // SqlException fırlatır. Controller'daki try-catch yakalar.
    }
}