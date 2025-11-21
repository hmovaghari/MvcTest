using System.ComponentModel.DataAnnotations.Schema;

namespace MyAccounting.Data.Model
{
    [Table("Transaction")]
    public class Transaction
    {
        public Guid TransactionID { get; set; } = Guid.NewGuid();
        public Guid TransactionTypeID { get; set; }
        public Guid UserID { get; set; }
        public Guid? ReceiverPersonID { get; set; }
        public Guid? PayerPersonID { get; set; }
        public DateTime Date { get; set; }
        public decimal Amount { get; set; }
        public string Description { get; set; } = string.Empty;
        public Guid? ApiKeyID { get; set; }
        public TransactionType TransactionType { get; set; } = null!;
        public User User { get; set; } = null!;
        public Person? ReceiverPerson { get; set; }
        public Person? PayerPerson { get; set; }
        public ApiKey? ApiKey { get; set; }
    }
}
