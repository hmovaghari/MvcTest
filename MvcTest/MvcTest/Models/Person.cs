using System.ComponentModel.DataAnnotations.Schema;

namespace MyAccounting.Data.Model
{
    [Table("Person")]
    public class Person
    {
        public Guid PersonID { get; set; } = Guid.NewGuid();
        public Guid UserID { get; set; }
        public string Name { get; set; } = string.Empty;
        public bool IsPerson { get; set; }
        public string PersonTell { get; set; } = string.Empty;
        public string PersonMobile { get; set; } = string.Empty;
        public string PersonEmail { get; set; } = string.Empty;
        public string PersonAddress { get; set; } = string.Empty;
        public string BankAccountNumber { get; set; } = string.Empty;
        public string BankShaba { get; set; } = string.Empty;
        public string BankCard { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public Guid? CurrencyUnitID { get; set; }
        public User User { get; set; } = null!;
        public CurrencyUnit? CurrencyUnit { get; set; }
        public virtual ICollection<Transaction>? TransactionReceivers { get; set; }
        public virtual ICollection<Transaction>? TransactionPayers { get; set; }
    }
}
