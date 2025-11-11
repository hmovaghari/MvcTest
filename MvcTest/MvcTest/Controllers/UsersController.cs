using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyAccounting.Data;
using MyAccounting.Data.Model;
using MyAccounting.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace MvcTest.Controllers
{
    public class UsersController : BaseController
    {
        public UsersController(SqlDBContext context) : base(context)
        {
        }

        // GET: Users
        [Authorize]
        public async Task<IActionResult> Index()
        {
            var user = await GetCurrentUser();
            if (user == null || !user.IsAdmin)
            {
                return RedirectToAction("Index", "Home");
            }
            var users = await _context.Users.ToListAsync();
            var userDTOs = users.Select(x => new UserDTO()
            {
                UserID = x.UserID,
                Username = x.Username,
                Email = x.Email,
                IsActive = x.IsActive,
                IsAdmin = x.IsAdmin,
            });
            return View(userDTOs);
        }

        // GET: Users/Login
        public IActionResult Login(string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        // POST: Users/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model, string returnUrl = null)
        {
            if (ModelState.IsValid)
            {
                var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == model.Username);
                if (user != null)
                {
                    var hashedPassword = GenerateHashedPassword(model.Password, user.Salt1, user.Salt2);
                    if (hashedPassword == user.Password)
                    {
                        var claims = new List<Claim>
                        {
                            new Claim(ClaimTypes.Name, user.Username),
                            new Claim("UserId", user.UserID.ToString()),
                            new Claim("IsAdmin", user.IsAdmin.ToString())
                        };

                        var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));

                        if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                        {
                            return Redirect(returnUrl);
                        }
                        return RedirectToAction("Index", "Home");
                    }
                }
                ModelState.AddModelError(string.Empty, "نام کاربری یا رمز عبور نادرست است");
            }
            return View(model);
        }

        // POST: Users/Logout
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }

        // GET: Users/Create
        public async Task<IActionResult> Create()
        {
            var user = await GetCurrentUser();
            if (user != null && !user.IsAdmin)
            {
                return RedirectToAction("Index", "Home");
            }
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
        [Authorize]
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return RedirectToAction("Index", "Home");
            }

            var user = await _context.Users.FindAsync(id);
            var currentUser = await GetCurrentUser();
            
            if (user == null || currentUser == null || (!currentUser.IsAdmin && currentUser.UserID != user.UserID))
            {
                return RedirectToAction("Index", "Home");
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
        [Authorize]
        public async Task<IActionResult> Delete(Guid? id)
        {
            var currentUser = await GetCurrentUser();
            if (currentUser == null || !currentUser.IsAdmin)
            {
                return RedirectToAction("Index", "Home");
            }

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

            var userDTO = new UserDTO()
            {
                UserID = user.UserID,
                Username = user.Username,
                Email = user.Email,
                IsActive = user.IsActive,
                IsAdmin = user.IsAdmin,
            };

            return View(userDTO);
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
