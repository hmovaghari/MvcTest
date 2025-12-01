using System.ComponentModel.DataAnnotations;

namespace MyAccounting.ViewModels
{
    public class CreatePerson
    {
        [Required(ErrorMessage = "نام و نام خانوادگی الزامی است")]
        [Display(Name = "نام و نام خانوادگی")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "واحد ارزی الزامی است")]
        [Display(Name = "واحد ارزی")]
        public Guid? CurrencyUnitID { get; set; }

        [Display(Name = "تلفن")]
        [Phone(ErrorMessage = "فرمت تلفن صحیح نیست")]
        public string? PersonTell { get; set; }

        [Display(Name = "موبایل")]
        [Phone(ErrorMessage = "فرمت موبایل صحیح نیست")]
        public string? PersonMobile { get; set; }

        [Display(Name = "ایمیل")]
        [EmailAddress(ErrorMessage = "فرمت ایمیل صحیح نیست")]
        public string? PersonEmail { get; set; }

        [Display(Name = "آدرس")]
        public string? PersonAddress { get; set; }

        [Display(Name = "توضیحات")]
        public string? Description { get; set; }
    }
}
