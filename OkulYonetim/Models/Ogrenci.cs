using System.ComponentModel.DataAnnotations;

namespace OkulYonetim.Models;

public class Ogrenci
{
    public long OgrenciId { get; set; }

    [Required(ErrorMessage = "Bölüm seçmelisiniz.")]
    [Display(Name = "Bölüm")]
    public long BolumId { get; set; }

    [Required(ErrorMessage = "Ad zorunludur.")]
    [StringLength(255)]
    [Display(Name = "Ad")]
    public string OgrenciAd { get; set; } = "";

    [Required(ErrorMessage = "Soyad zorunludur.")]
    [StringLength(255)]
    [Display(Name = "Soyad")]
    public string OgrenciSoyad { get; set; } = "";

    // ⭐ YENİ: sayı alanı, aralık kontrolü ile
    [Required(ErrorMessage = "Sınıf zorunludur.")]
    [Range(1, 6, ErrorMessage = "Sınıf 1 ile 6 arasında olmalıdır.")]
    [Display(Name = "Sınıf")]
    public int OgrenciSinif { get; set; }

    // ⭐ YENİ: tarih alanı
    [Required(ErrorMessage = "Doğum tarihi zorunludur.")]
    [DataType(DataType.Date)]
    [Display(Name = "Doğum tarihi")]
    public DateTime OgrenciDogumTarihi { get; set; }

    // ⭐ YENİ: sınırlı seçenek
    [Required(ErrorMessage = "Cinsiyet seçmelisiniz.")]
    [Display(Name = "Cinsiyet")]
    public string OgrenciCinsiyet { get; set; } = "";

    [Required(ErrorMessage = "Adres zorunludur.")]
    [Display(Name = "Adres")]
    public string OgrenciAdres { get; set; } = "";

    [Required(ErrorMessage = "Telefon zorunludur.")]
    [Phone(ErrorMessage = "Geçerli bir telefon numarası girin.")]
    [Display(Name = "Telefon")]
    public string OgrenciTelefon { get; set; } = "";

    [Required(ErrorMessage = "E-posta zorunludur.")]
    [EmailAddress(ErrorMessage = "Geçerli bir e-posta adresi girin.")]
    [Display(Name = "E-posta")]
    public string OgrenciEposta { get; set; } = "";

    // ⭐ YENİ: desen (regex) kontrolü
    [Required(ErrorMessage = "TC kimlik numarası zorunludur.")]
    [RegularExpression(@"^[1-9][0-9]{10}$",
        ErrorMessage = "TC kimlik numarası 11 haneli olmalı ve 0 ile başlamamalıdır.")]
    [Display(Name = "TC kimlik no")]
    public string OgrenciTc { get; set; } = "";

    public DateTime CreatedDate { get; set; }
    public DateTime? UpdatedDate { get; set; }
    public string IsActive { get; set; } = "1";

    // JOIN ile gelecek — veritabanında yok
    [Display(Name = "Bölüm")]
    public string BolumAdi { get; set; } = "";

    // Kolaylık için hesaplanan alan
    public string TamAd => OgrenciAd + " " + OgrenciSoyad;
}