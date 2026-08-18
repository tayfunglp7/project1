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

        b.BolumId = okuyucu.GetInt64(okuyucu.GetOrdinal("bolum_id"));
        b.FakulteId = okuyucu.GetInt64(okuyucu.GetOrdinal("fakulte_id"));
        b.BolumAdi = okuyucu.GetString(okuyucu.GetOrdinal("bolum_adi"));
        b.BolumAdres = okuyucu.GetString(okuyucu.GetOrdinal("bolum_adres"));
        b.BolumTelefon = okuyucu.GetString(okuyucu.GetOrdinal("bolum_telefon"));
        b.BolumEposta = okuyucu.GetString(okuyucu.GetOrdinal("bolum_eposta"));
        b.CreatedDate = okuyucu.GetDateTime(okuyucu.GetOrdinal("created_date"));
        b.IsActive = okuyucu.GetString(okuyucu.GetOrdinal("is_active"));

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
                       ORDER BY created_date asc";
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
    public Bolum? IdIleGetir(long id)
    {
        Bolum? sonuc = null;

        string sql = @"SELECT bolum_id, fakulte_id, bolum_adi, bolum_adres,
                              bolum_telefon, bolum_eposta,
                              created_date, updated_date, is_active
                       FROM bolum
                       WHERE bolum_id = @id";

        using (SqlConnection baglanti = new SqlConnection(_baglantiMetni))
        using (SqlCommand komut = new SqlCommand(sql, baglanti))
        {
            komut.Parameters.AddWithValue("@id", id);
            baglanti.Open();

            using (SqlDataReader okuyucu = komut.ExecuteReader())
            {
                if (okuyucu.Read())
                    sonuc = SatiriNesneyeCevir(okuyucu);
            }
        }

        return sonuc;
    }

    public void Ekle(Bolum bolum)
    {
        string sql = @"INSERT INTO bolum
                          (fakulte_id, bolum_adi, bolum_adres, bolum_telefon,
                           bolum_eposta, created_date, updated_date, is_active)
                       VALUES
                          (@fakulteId, @adi, @adres, @telefon,
                           @eposta, @createdDate, NULL, @isActive)";

        using (SqlConnection baglanti = new SqlConnection(_baglantiMetni))
        using (SqlCommand komut = new SqlCommand(sql, baglanti))
        {
            komut.Parameters.AddWithValue("@fakulteId", bolum.FakulteId);
            komut.Parameters.AddWithValue("@adi", bolum.BolumAdi);
            komut.Parameters.AddWithValue("@adres", bolum.BolumAdres);
            komut.Parameters.AddWithValue("@telefon", bolum.BolumTelefon);
            komut.Parameters.AddWithValue("@eposta", bolum.BolumEposta);
            komut.Parameters.AddWithValue("@createdDate", DateTime.Now);
            komut.Parameters.AddWithValue("@isActive", "1");

            baglanti.Open();
            komut.ExecuteNonQuery();
        }
    }

    public void Guncelle(Bolum bolum)
    {
        string sql = @"UPDATE bolum
                       SET fakulte_id    = @fakulteId,
                           bolum_adi     = @adi,
                           bolum_adres   = @adres,
                           bolum_telefon = @telefon,
                           bolum_eposta  = @eposta,
                           updated_date  = @updatedDate
                       WHERE bolum_id    = @id";

        using (SqlConnection baglanti = new SqlConnection(_baglantiMetni))
        using (SqlCommand komut = new SqlCommand(sql, baglanti))
        {
            komut.Parameters.AddWithValue("@fakulteId", bolum.FakulteId);
            komut.Parameters.AddWithValue("@adi", bolum.BolumAdi);
            komut.Parameters.AddWithValue("@adres", bolum.BolumAdres);
            komut.Parameters.AddWithValue("@telefon", bolum.BolumTelefon);
            komut.Parameters.AddWithValue("@eposta", bolum.BolumEposta);
            komut.Parameters.AddWithValue("@updatedDate", DateTime.Now);
            komut.Parameters.AddWithValue("@id", bolum.BolumId);

            baglanti.Open();
            komut.ExecuteNonQuery();
        }
    }

    public void PasifYap(long id)
    {
        string sql = @"UPDATE bolum
                       SET is_active = '0', updated_date = @updatedDate
                       WHERE bolum_id = @id";

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