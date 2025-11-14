using Microsoft.EntityFrameworkCore;
using MyAccounting.Data.Model;

namespace MyAccounting.Data
{
    public class SqlDBContext : DbContext
    {
        public SqlDBContext(DbContextOptions<SqlDBContext> options) : base(options)
        {
            
        }

        public DbSet<User> Users { get; set; }
        public DbSet<SettingKey> SettingKeys { get; set; }
        public DbSet<SettingValue> SettingValues { get; set; }
        public DbSet<Person> People { get; set; }
        public DbSet<CurrencyUnit> CurrencyUnits { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<TransactionType> TransactionTypes { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // رابطه اول: Payer (پرداخت‌کننده)
            modelBuilder.Entity<Transaction>()
                .HasOne(t => t.PayerPerson)                    // هر تراکنش یک پرداخت‌کننده دارد
                .WithMany(p => p.TransactionPayers)            // هر شخص چند تراکنش پرداختی دارد
                .HasForeignKey(t => t.PayerPersonID)           // کلید خارجی
                .OnDelete(DeleteBehavior.Restrict);            // جلوگیری از حذف آبشاری

            // رابطه دوم: Receiver (دریافت‌کننده)
            modelBuilder.Entity<Transaction>()
                .HasOne(t => t.ReceiverPerson)                 // هر تراکنش یک دریافت‌کننده دارد
                .WithMany(p => p.TransactionReceivers)         // هر شخص چند تراکنش دریافتی دارد
                .HasForeignKey(t => t.ReceiverPersonID)        // کلید خارجی
                .OnDelete(DeleteBehavior.Restrict);            // جلوگیری از حذف آبشاری

            //کاربر مدیریت پیشفرض
            modelBuilder.Entity<User>().HasData(
                new User
                {
                    UserID = Guid.Parse("11111111-1111-1111-1111-111111111111"),
                    Username = "admin",
                    Salt1 = "bcfbf4a8-67fc-4db9-ba7e-908afb4de0f7",
                    Salt2 = "ddc46224-2fdc-4320-b821-f7f7c2e65bba",
                    Password = "0130fd5601a7addede0fb3dfac1657353497fe54d9a54a7a83b82ef77e5e6212",
                    IsActive = true,
                    IsAdmin = true
                }
            );

            // مقادیر پیشفرض تنظیمات
            modelBuilder.Entity<SettingKey>().HasData(
                new SettingKey
                {
                    SettingKeyID = Guid.Parse("11111111-1111-1111-1111-111111111111"),
                    Key = "NewTrasactionDateType",
                    Description = "تاریخ پیشفرض تراکنش‌های جدید"
                }
            );

            // مقادیر پیشفرض نوع ارز
            modelBuilder.Entity<CurrencyUnit>().HasData(
                new CurrencyUnit
                {
                    CurrencyUnitID = Guid.Parse("11111111-1111-1111-1111-111111111111"),
                    Name = "ریال",
                    IsDesimal = false
                },
                new CurrencyUnit
                {
                    CurrencyUnitID = Guid.Parse("22222222-2222-2222-2222-222222222222"),
                    Name = "تومان",
                    IsDesimal = false
                }
            );
        }
    }
}
