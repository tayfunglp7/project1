using Microsoft.Data.SqlClient;
using OkulYonetim.Models;

namespace OkulYonetim.Data;

public class BolumRepository
{
    private readonly string _baglantiMetni;

    public BolumRepository(IConfiguration configuration)
    {
        _baglantiMetni = configuration.GetConnectionString("OkulDb")!;
    }

    private Bolum SatiriNesneyeCevir(SqlDataReader okuyucu)
    {
        Bolum b = new Bolum();

        b.BolumId      = okuyucu.GetInt64(okuyucu.GetOrdinal("bolum_id"));
        b.FakulteId    = okuyucu.GetInt64(okuyucu.GetOrdinal("fakulte_id"));
        b.BolumAdi     = okuyucu.GetString(okuyucu.GetOrdinal("bolum_adi"));
        b.BolumAdres   = okuyucu.GetString(okuyucu.GetOrdinal("bolum_adres"));
        b.BolumTelefon = okuyucu.GetString(okuyucu.GetOrdinal("bolum_telefon"));
        b.BolumEposta  = okuyucu.GetString(okuyucu.GetOrdinal("bolum_eposta"));
        b.CreatedDate  = okuyucu.GetDateTime(okuyucu.GetOrdinal("created_date"));
        b.IsActive     = okuyucu.GetString(okuyucu.GetOrdinal("is_active"));

        int sutunNo = okuyucu.GetOrdinal("updated_date");
        b.UpdatedDate = okuyucu.IsDBNull(sutunNo) ? null : okuyucu.GetDateTime(sutunNo);

        return b;
    }

    /// <summary>
    /// Aktif bölümleri, bağlı oldukları fakültenin adıyla birlikte getirir.
    /// </summary>
    public List<Bolum> TumunuGetir()
    {
        List<Bolum> liste = new List<Bolum>();

        // ⭐ İKİ TABLOYU BİRLEŞTİRİYORUZ
        string sql = @"SELECT b.bolum_id, b.fakulte_id, b.bolum_adi, b.bolum_adres,
                              b.bolum_telefon, b.bolum_eposta,
                              b.created_date, b.updated_date, b.is_active,
                              f.fakulte_ad
                       FROM bolum b
                       INNER JOIN fakulte f ON b.fakulte_id = f.fakulte_id
                       WHERE b.is_active = '1'
                       ORDER BY f.fakulte_ad, b.bolum_adi";
        using (SqlConnection baglanti = new SqlConnection(_baglantiMetni))
        using (SqlCommand komut = new SqlCommand(sql, baglanti))
        {
            baglanti.Open();
            using (SqlDataReader okuyucu = komut.ExecuteReader())
            {
                while (okuyucu.Read())
                {
                    Bolum b = SatiriNesneyeCevir(okuyucu);
                    // JOIN'den gelen ekstra sütun
                    b.FakulteAd = okuyucu.GetString(okuyucu.GetOrdinal("fakulte_ad"));
                    liste.Add(b);
                }
            }
        }
        return liste;
    }
}