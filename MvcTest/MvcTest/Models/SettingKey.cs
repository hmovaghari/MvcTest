using System.ComponentModel.DataAnnotations.Schema;

namespace MyAccounting.Data.Model
{
    [Table("SettingKey")]
    public class SettingKey
    {
        public Guid SettingKeyID { get; set; }
        public string Key { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public Guid? ApiKeyID { get; set; }
        public ICollection<SettingValue>? SettingValues { get; }
        public ApiKey? ApiKey { get; set; }
    }
}
