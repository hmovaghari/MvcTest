using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyAccounting.Data.Model
{
    [Table("User")]
    public class User
    {
        public Guid UserID { get; set; }
        
        [Required]
        public string Username { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string Salt1 { get; set; } = string.Empty;
        public string Salt2 { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;
        public bool IsAdmin { get; set; } = false;

        public ICollection<SettingKey>? SettingKeys { get; }

        public User()
        {
            UserID = Guid.NewGuid();
        }
    }
}
