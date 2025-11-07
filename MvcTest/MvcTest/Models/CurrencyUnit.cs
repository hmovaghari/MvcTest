using System.ComponentModel.DataAnnotations.Schema;

namespace MyAccounting.Data.Model
{
    [Table("CurrencyUnit")]
    public class CurrencyUnit
    {
        public Guid CurrencyUnitID { get; set; }
        public string Name { get; set; } = string.Empty;
        public bool IsDesimal { get; set; }
        public ICollection<Person>? Persons { get; set; }
    }
}
