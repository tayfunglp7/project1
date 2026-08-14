using System.ComponentModel.DataAnnotations;

namespace OkulYonetim.Models;

public class Bolum
{
    public long BolumId { get; set; }

    [Required(ErrorMessage = "Fakülte seçmelisiniz.")]
    [Display(Name = "Bağlı olduğu fakülte")]
    public long FakulteId { get; set; }          // ⭐ yabancı anahtar

    [Required(ErrorMessage = "Bölüm adı zorunludur.")]
    [StringLength(255)]
    [Display(Name = "Bölüm adı")]
    public string BolumAdi { get; set; } = "";   // ⚠️ dikkat: bolum_adi (fakültede _ad idi)

    [Required(ErrorMessage = "Adres zorunludur.")]
    [Display(Name = "Adres")]
    public string BolumAdres { get; set; } = "";

    [Required(ErrorMessage = "Telefon zorunludur.")]
    [Phone(ErrorMessage = "Geçerli bir telefon numarası girin.")]
    [Display(Name = "Telefon")]
    public string BolumTelefon { get; set; } = "";

    [Required(ErrorMessage = "E-posta zorunludur.")]
    [EmailAddress(ErrorMessage = "Geçerli bir e-posta adresi girin.")]
    [Display(Name = "E-posta")]
    public string BolumEposta { get; set; } = "";

    public DateTime CreatedDate { get; set; }
    public DateTime? UpdatedDate { get; set; }
    public string IsActive { get; set; } = "1";

    // ⭐ Veritabanında OLMAYAN alan!
    // JOIN sonucu gelen fakülte adını taşımak için ekledik.
    // Sadece listede göstermek için var.
    [Display(Name = "Fakülte")]
    public string FakulteAd { get; set; } = "";
}