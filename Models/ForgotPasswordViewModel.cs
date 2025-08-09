using System.ComponentModel.DataAnnotations;

namespace CuzdanUygulamasi.Models
{
    public class ForgotPasswordViewModel
    {
        [Required(ErrorMessage = "Email alanı zorunludur")]
        [EmailAddress(ErrorMessage = "Geçersiz email formatı")]
        public string Email { get; set; }
    }
    
}
