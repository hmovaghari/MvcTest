using Microsoft.AspNetCore.Mvc.Rendering;
using MyAccounting.Data;
using MyAccounting.Data.Model;

namespace MyAccounting.Repository
{
    public class CurrencyUnitRepository
    {
        private SqlDBContext _context;

        public CurrencyUnitRepository(SqlDBContext context)
        {
            _context = context;
        }

        public SelectList GetSelectList(Guid? currencyUnitID = null)
        {
            return new SelectList(_context.CurrencyUnits, "CurrencyUnitID", "Name", currencyUnitID);
        }
    }
}
