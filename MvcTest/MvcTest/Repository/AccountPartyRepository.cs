using MyAccounting.Data;

namespace MyAccounting.Repository
{
    public class AccountPartyRepository
    {
        private SqlDBContext _context;

        public AccountPartyRepository(SqlDBContext context)
        {
            _context = context;
        }
    }
}
