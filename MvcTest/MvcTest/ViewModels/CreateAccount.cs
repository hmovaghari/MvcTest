using System.ComponentModel.DataAnnotations;

namespace MyAccounting.ViewModels
{
    public class CreateAccount
    {
        [Required(ErrorMessage = "نام حساب الزامی است")]
        [Display(Name = "نام حساب")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "واحد ارزی الزامی است")]
        [Display(Name = "واحد ارزی")]
        public Guid? CurrencyUnitID { get; set; }

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
