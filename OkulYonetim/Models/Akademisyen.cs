using System.ComponentModel.DataAnnotations;

namespace OkulYonetim.Models;

/// <summary>
/// akademisyen tablosunun C# karşılığı.
/// Ogrenci sınıfının neredeyse aynısıdır; tek fark "sinif" alanının olmamasıdır.
/// </summary>
public class Akademisyen
{
    public long AkademisyenId { get; set; }

    [Required(ErrorMessage = "Bölüm seçmelisiniz.")]
    [Display(Name = "Bölüm")]
    public long BolumId { get; set; }

    [Required(ErrorMessage = "Ad zorunludur.")]
    [StringLength(255)]
    [Display(Name = "Ad")]
    public string AkademisyenAd { get; set; } = "";

    [Required(ErrorMessage = "Soyad zorunludur.")]
    [StringLength(255)]
    [Display(Name = "Soyad")]
    public string AkademisyenSoyad { get; set; } = "";

    [Required(ErrorMessage = "Doğum tarihi zorunludur.")]
    [DataType(DataType.Date)]
    [Display(Name = "Doğum tarihi")]
    public DateTime AkademisyenDogumTarihi { get; set; }

    [Required(ErrorMessage = "Cinsiyet seçmelisiniz.")]
    [Display(Name = "Cinsiyet")]
    public string AkademisyenCinsiyet { get; set; } = "";

    [Required(ErrorMessage = "Adres zorunludur.")]
    [Display(Name = "Adres")]
    public string AkademisyenAdres { get; set; } = "";

    [Required(ErrorMessage = "Telefon zorunludur.")]
    [Phone(ErrorMessage = "Geçerli bir telefon numarası girin.")]
    [Display(Name = "Telefon")]
    public string AkademisyenTelefon { get; set; } = "";

    [Required(ErrorMessage = "E-posta zorunludur.")]
    [EmailAddress(ErrorMessage = "Geçerli bir e-posta adresi girin.")]
    [Display(Name = "E-posta")]
    public string AkademisyenEposta { get; set; } = "";

    [Required(ErrorMessage = "TC kimlik numarası zorunludur.")]
    [RegularExpression(@"^[1-9][0-9]{10}$",
        ErrorMessage = "TC kimlik numarası 11 haneli olmalı ve 0 ile başlamamalıdır.")]
    [Display(Name = "TC kimlik no")]
    public string AkademisyenTc { get; set; } = "";

    public DateTime CreatedDate { get; set; }
    public DateTime? UpdatedDate { get; set; }
    public string IsActive { get; set; } = "1";

    // ── Veritabanında OLMAYAN alanlar ────────────────────────────────
    // JOIN ile gelen bilgileri taşımak için var.
    // INSERT / UPDATE sorgularında KULLANILMAZ!

    [Display(Name = "Bölüm")]
    public string BolumAdi { get; set; } = "";

    [Display(Name = "Fakülte")]
    public string FakulteAd { get; set; } = "";

    // Hesaplanan alan — view'da @a.TamAd yazabilmek için
    public string TamAd => AkademisyenAd + " " + AkademisyenSoyad;
}