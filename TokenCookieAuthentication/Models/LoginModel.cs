using System.ComponentModel.DataAnnotations;

namespace CookieAuthentication.Models
{
    public class LoginModel
    {
        [Display(Name = "Username")]
        [Required(ErrorMessage = "Username Field Required")]
        [DataType(DataType.Text)]
        [MaxLength(8, ErrorMessage = "Username Shouldn`t Exceed 8 Letters")]
        public string UserName { get; set; }
        [Display(Name = "Password")]
        [Required(ErrorMessage = "Password Field required")]
        [DataType(DataType.Password)]
        [MaxLength(50, ErrorMessage = "Password Shouldn`t Exceed 50 Letters")]
        [MinLength(8, ErrorMessage = "Password Should be Longer than 8 Letters")]

        public string Password { get; set; }

        [Display(Name = "Remember Me")]
        public bool RememberMe { get; set; }

        public string ReturnUrl { get; set; }
    }
}
