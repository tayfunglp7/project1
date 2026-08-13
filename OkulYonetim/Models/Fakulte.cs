using System.ComponentModel.DataAnnotations;

namespace OkulYonetim.Models;

public class Fakulte
{
    public long FakulteId { get; set; }

    [Required(ErrorMessage = "Fakülte adı zorunludur.")]
    [StringLength(255, ErrorMessage = "Fakülte adı en fazla 255 karakter olabilir.")]
    [Display(Name = "Fakülte adı")]
    public string FakulteAd { get; set; } = "";

    [Required(ErrorMessage = "Adres zorunludur.")]
    [Display(Name = "Adres")]
    public string FakulteAdres { get; set; } = "";

    [Required(ErrorMessage = "Telefon zorunludur.")]
    [Phone(ErrorMessage = "Geçerli bir telefon numarası girin.")]
    [Display(Name = "Telefon")]
    public string FakulteTelefon { get; set; } = "";

    [Required(ErrorMessage = "E-posta zorunludur.")]
    [EmailAddress(ErrorMessage = "Geçerli bir e-posta adresi girin.")]
    [Display(Name = "E-posta")]
    public string FakulteEposta { get; set; } = "";

    public DateTime CreatedDate { get; set; }
    public DateTime? UpdatedDate { get; set; }
    public string IsActive { get; set; } = "1";
}