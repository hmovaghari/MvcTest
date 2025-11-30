namespace MyAccounting.ViewModels
{
    public class ChangeUser
    {
        public Guid UserID { get; set; }

        public bool IsActive { get; set; }

        public bool IsAdmin { get; set; }
    }
}
