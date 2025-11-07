using System.ComponentModel.DataAnnotations.Schema;

namespace MyAccounting.Data.Model
{
    [Table("TransactionType")]
    public class TransactionType
    {
        public Guid TransactionTypeID { get; set; }
        public Guid? ParentTransactionTypeID { get; set; }
        public Guid UserID { get; set; }
        public bool IsCost { get; set; }
        public string Name { get; set; } = string.Empty;
        public User User { get; set; } = null!;
        public TransactionType? ParentTransactionType { get; set; }
        public ICollection<Transaction>? Transactions { get; }
        public ICollection<TransactionType>? TransactionTypes { get; }
    }
}
