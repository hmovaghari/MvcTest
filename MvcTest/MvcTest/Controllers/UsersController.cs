using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MyAccounting.Data;
using MyAccounting.Data.Model;
using MyAccounting.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace MvcTest.Controllers
{
    public class UsersController : Controller
    {
        private readonly SqlDBContext _context;

        public UsersController(SqlDBContext context)
        {
            _context = context;
        }

        // GET: Users
        public async Task<IActionResult> Index()
        {
            return View(await _context.Users.ToListAsync());
        }

        // GET: Users/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.Users
                .FirstOrDefaultAsync(m => m.UserID == id);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        // GET: Users/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Users/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("UserID,Username,Email,Password,ConfirmPassword")] CreateUser createUser)
        {
            var user = new User();
            if (ModelState.IsValid)
            {
                if (await ControllData(userID: null, createUser.Username, createUser.Email))
                {
                    user.UserID = Guid.NewGuid();
                    user.Username = createUser.Username;
                    user.Email = createUser.Email;
                    user.Salt1 = Guid.NewGuid().ToString();
                    user.Salt2 = Guid.NewGuid().ToString();
                    user.Password = GenerateHashedPassword(createUser.Password, user.Salt1, user.Salt2);
                    user.IsActive = true;
                    user.IsAdmin = false;
                    _context.Add(user);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
            }
            return View(createUser);
        }

        private string GenerateHashedPassword(string password, string salt1, string salt2)
        {
            var text = salt1.Replace("-", "") + password + salt2.Replace("-", "");
            using (var sha256 = SHA256.Create())
            {

                var data = Encoding.UTF8.GetBytes(text ?? string.Empty);
                var hashBytes = sha256.ComputeHash(data);
                var hashString = BitConverter.ToString(hashBytes).Replace("-", "").ToLowerInvariant();
                return hashString;
            }
        }

        private async Task<bool> ControllData(Guid? userID, string username, string email)
        {
            // بررسی تکراری بودن نام کاربری (به جز خود کاربر)
            if (await _context.Users.AnyAsync(u => (userID == null || u.UserID != userID) && u.Username == username))
            {
                ModelState.AddModelError("Username", "این نام کاربری قبلاً استفاده شده است");
                return false;
            }

            // بررسی تکراری بودن ایمیل (به جز خود کاربر)
            if (await _context.Users.AnyAsync(u => (userID == null || u.UserID != userID) && u.Email == email))
            {
                ModelState.AddModelError("Email", "این ایمیل قبلاً استفاده شده است");
                return false;
            }

            return true;
        }

        // GET: Users/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            var model = new EditUser
            {
                UserID = user.UserID,
                Username = user.Username,
                Email = user.Email
            };

            return View(model);
        }

        // POST: Users/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("UserID,Username,Email,NewPassword,ConfirmNewPassword")] EditUser editUser)
        {
            if (id != editUser.UserID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var user = await _context.Users.FindAsync(id);
                    if (user == null)
                    {
                        return NotFound();
                    }

                    if (await ControllData(editUser.UserID, editUser.Username, editUser.Email))
                    {
                        user.Username = editUser.Username;
                        user.Email = editUser.Email;
                        if (!string.IsNullOrEmpty(editUser.NewPassword))
                        {
                            user.Salt1 = Guid.NewGuid().ToString();
                            user.Salt2 = Guid.NewGuid().ToString();
                            user.Password = GenerateHashedPassword(editUser.NewPassword, user.Salt1, user.Salt2);
                        }
                        _context.Update(user);
                        await _context.SaveChangesAsync();
                        return RedirectToAction(nameof(Index));
                    }
                    else
                    {
                        return View(editUser);
                    }
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UserExists(editUser.UserID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }
            return View(editUser);
        }

        // GET: Users/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.Users
                .FirstOrDefaultAsync(m => m.UserID == id);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        // POST: Users/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user != null)
            {
                _context.Users.Remove(user);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool UserExists(Guid id)
        {
            return _context.Users.Any(e => e.UserID == id);
        }
    }
}
