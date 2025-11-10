using System.ComponentModel.DataAnnotations;

namespace MyAccounting.ViewModels
{
    public class UserDTO
    {
        public Guid UserID { get; set; }

        [Display(Name = "نام کاربری")]
        public string Username { get; set; } = string.Empty;

        [Display(Name = "ایمیل")]
        public string Email { get; set; } = string.Empty;

        [Display(Name = "فعال")]
        public bool IsActive { get; set; }

        [Display(Name = "مدیر")]
        public bool IsAdmin { get; set; } = false;
    }
}
