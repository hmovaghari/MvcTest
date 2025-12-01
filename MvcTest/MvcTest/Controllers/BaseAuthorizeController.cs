using Microsoft.AspNetCore.Mvc;
using MyAccounting.Data;
using MyAccounting.Repository;
using MyAccounting.ViewModels;

namespace MyAccounting.Controllers
{
    public class BaseAuthorizeController : Controller
    {
        protected readonly SqlDBContext _context;

        public BaseAuthorizeController(SqlDBContext context)
        {
            _context = context;
        }

        protected async Task<UserDTO?> GetCurrentUser()
        {
            if (!User.Identity.IsAuthenticated)
                return null;

            var username = User.Identity.Name;
            var _userRepository = new UserRepository(_context);
            return await _userRepository.GetUserByUsername(username);
        }

        protected IActionResult RedirectToMainPage()
        {
            return RedirectToAction("Index", "Home");
        }
    }
}