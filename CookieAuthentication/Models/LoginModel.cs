using System.ComponentModel.DataAnnotations;

namespace CookieAuthentication.Models
{
    public class LoginModel
    {
        [Display(Name = "username")]
        [Required(ErrorMessage = "username required!")]
        [DataType(DataType.Text)]
        [MaxLength(8)]
        public string UserName { get; set; }
        [Display(Name = "password")]
        [Required(ErrorMessage = "password required!")]
        [DataType(DataType.Password)]
        [MaxLength(50), MinLength(8)]
        
        public string Password { get; set; }

        [Display(Name = "Remember Me")]
        public bool RememberMe { get; set; }

        public string ReturnUrl { get; set; }
    }
}
