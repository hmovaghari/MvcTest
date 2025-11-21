namespace MyAccounting.Data.Model
{
    public class ApiKey
    {
        public Guid ApiKeyID { get; set; } = Guid.NewGuid();
        public Guid UserID { get; set; }
        public bool IsActive { get; set; } = true;
        public bool IsAdmin { get; set; } = false;
        public User User { get; set; } = null!;
        public ICollection<CurrencyUnit>? CurrencyUnits { get; }
        public ICollection<Person>? People { get; }
        public ICollection<SettingKey>? SettingKeys { get; }
        public ICollection<SettingValue>? SettingValues { get; }
        public ICollection<Transaction>? Transactions { get; }
        public ICollection<TransactionType>? TransactionTypes { get; }
        public ICollection<User>? Users { get; }
    }
}
