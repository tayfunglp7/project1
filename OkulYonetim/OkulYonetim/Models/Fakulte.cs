namespace OkulYonetim.Models;

public class Fakulte
{
    // Veritabanındaki her sütun için bir özellik (property) yazıyoruz.

    public long FakulteId { get; set; }              // fakulte_id  (BIGINT → long)
    public string FakulteAd { get; set; } = "";      // fakulte_ad
    public string FakulteAdres { get; set; } = "";   // fakulte_adres
    public string FakulteTelefon { get; set; } = ""; // fakulte_telefon
    public string FakulteEposta { get; set; } = "";  // fakulte_eposta

    public DateTime CreatedDate { get; set; }        // created_date
    public DateTime? UpdatedDate { get; set; }       // updated_date — NULL olabilir!
    public string IsActive { get; set; } = "1";      // is_active
}

//Repository
//"Veritabanı işlerini yapan sınıfa Repository (depo) diyoruz. 
//Neden ayrı bir sınıf? Çünkü Controller'ın işi trafiği yönetmek, SQL yazmak değil. 
// SQL'i tek bir yerde toplarsak, ileride değiştirmemiz gerektiğinde tek yere bakarız."