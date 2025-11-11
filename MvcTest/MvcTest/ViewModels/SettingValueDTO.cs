namespace MyAccounting.ViewModels
{
    public class SettingValueDTO
    {
        public Guid SettingValueID { get; set; }
        public Guid SettingKeyID { get; set; }
        public string SettingKeyTitle { get; set; }
        public Guid UserID { get; set; }
        public string SettingValue { get; set; }
    }
}
