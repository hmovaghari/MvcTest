using System.ComponentModel.DataAnnotations;

namespace MyAccounting.ViewModels
{
    public class ApiKeyDTO
    {
        [Display(Name = "کلید")]
        public Guid ApiKeyID { get; set; } = Guid.NewGuid();
        
        public Guid UserID { get; set; }

        [Display(Name = "فعال")]
        public bool IsActive { get; set; }

        [Display(Name = "مدیر")]
        public bool IsAdmin { get; set; }

        [Display(Name = "کاربر")]
        public UserDTO UserDto { get; set; }
    }
}
