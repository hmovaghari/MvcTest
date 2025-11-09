using System.ComponentModel.DataAnnotations;

namespace MyAccounting.ViewModels
{
  public class LoginViewModel
    {
        [Required(ErrorMessage = "نام کاربری وارد نشده")]
        [Display(Name = "نام کاربری")]
        public string Username { get; set; }

        [Required(ErrorMessage = "رمز عبور وارد نشده")]
        [Display(Name = "رمز عبور")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}