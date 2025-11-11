using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyAccounting.Data;
using MyAccounting.Data.Model;
using System.Threading.Tasks;

namespace MvcTest.Controllers
{
    public class BaseController : Controller
    {
        protected readonly SqlDBContext _context;

        public BaseController(SqlDBContext context)
        {
            _context = context;
        }

        protected async Task<User> GetCurrentUser()
        {
            if (!User.Identity.IsAuthenticated)
                return null;

            var username = User.Identity.Name;
            return await _context.Users.FirstOrDefaultAsync(u => u.Username == username);
        }
    }
}