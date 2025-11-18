using System.ComponentModel.DataAnnotations;

namespace MvcTest.ViewModels
{
    public class AccountDTO
    {
        public Guid PersonID { get; set; }
        public Guid UserID { get; set; }

        [Display(Name = "نام")]
        public string Name { get; set; } = string.Empty;

        public Guid? CurrencyUnitID { get; set; }

        [Display(Name = "واحد ارزی")]
        public string CurrencyUnitName { get; set; }

        [Display(Name = "شماره حساب")]
        public string? BankAccountNumber { get; set; }

        [Display(Name = "شماره شبا")]
        public string? BankShaba { get; set; }

        [Display(Name = "شماره کارت")]
        public string? BankCard { get; set; }

        [Display(Name = "توضیحات")]
        public string? Description { get; set; }
    }
}
