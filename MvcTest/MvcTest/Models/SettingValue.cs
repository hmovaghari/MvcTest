using System.ComponentModel.DataAnnotations.Schema;

namespace MyAccounting.Data.Model
{
    [Table("SettingValue")]
    public class SettingValue
    {
        public Guid SettingValueID { get; set; } = Guid.NewGuid();
        public Guid SettingKeyID { get; set; }
        public Guid UserID { get; set; }
        public string Value { get; set; } = string.Empty;
        public SettingKey SettingKey { get; set; } = null!;
        public User User { get; set; } = null!;
    }
}
