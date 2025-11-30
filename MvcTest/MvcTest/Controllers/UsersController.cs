using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyAccounting.Data;
using MyAccounting.ViewModels;
using System.Security.Claims;
using MyAccounting.Repository;

namespace MyAccounting.Controllers
{
    public class UsersController : BaseAuthorizeController
    {
        UserRepository _userRepository;

        public UsersController(SqlDBContext context) : base(context)
        {
            _userRepository = new UserRepository(context);
        }

        // GET: Users
        [Authorize]
        public async Task<IActionResult> Index()
        {
            var user = await GetCurrentUser();
            if (user == null || !user.IsAdmin)
            {
                return RedirectToMainPage();
            }

            var list = await _userRepository.GetAllUsersAsync();
            return View(list);
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
                var checkPassword = await _userRepository.CheckPassword(model);
                if (checkPassword.Item1 != null)
                {
                    if (checkPassword.Item1 == true)
                    {
                        var claims = new List<Claim>
                        {
                            new Claim(ClaimTypes.Name, checkPassword.Item2.Username),
                            new Claim("UserId", checkPassword.Item2.UserID.ToString()),
                            new Claim("IsAdmin", checkPassword.Item2.IsAdmin.ToString())
                        };

                        var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));

                        if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                        {
                            return Redirect(returnUrl);
                        }
                        return RedirectToMainPage();
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
            return RedirectToMainPage();
        }

        // GET: Users/Create
        public async Task<IActionResult> Create()
        {
            var user = await GetCurrentUser();
            if (user != null && !user.IsAdmin)
            {
                return RedirectToMainPage();
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
            
            if (ModelState.IsValid)
            {
                if (await ControllData(userID: null, createUser.Username, createUser.Email))
                {

                    if (await _userRepository.CreateUserAsync(createUser))
                    {
                        return RedirectToAction(nameof(Index));
                    }
                }
            }
            return View(createUser);
        }

        

        private async Task<bool> ControllData(Guid? userID, string username, string email)
        {
            var controllData = await _userRepository.ControllData(userID, username, email);
            if (controllData != null)
            {
                ModelState.AddModelError(controllData.Value.Item1, controllData.Value.Item2);
                return false;
            }

            return true;
        }

        // GET: Users/Change/5
        [Authorize]
        public async Task<IActionResult> Change(Guid? id)
        {
            if (id == null)
            {
                return RedirectToMainPage();
            }

            var user = await _userRepository.FindAsync(id);
            var currentUser = await GetCurrentUser();

            if (user == null || !(currentUser?.IsAdmin ?? false))
            {
                return RedirectToMainPage();
            }

            var changeUser = new ChangeUser()
            {
                UserID = user.UserID,
                IsActive = user.IsActive,
                IsAdmin = user.IsAdmin,
            };

            return View(changeUser);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Change(Guid id, [Bind("UserID,IsActive,IsAdmin")] ChangeUser changeUser)
        {
            if (id != changeUser.UserID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var user = await _userRepository.FindAsync(id);
                if (user == null)
                {
                    return NotFound();
                }

                if (await _userRepository.ChangeAsync(id, changeUser))
                {
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    return View(changeUser);
                }
            }
            return View(changeUser);
        }

        // GET: Users/Edit/5
        [Authorize]
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return RedirectToMainPage();
            }

            var user = await _userRepository.FindAsync(id);
            var currentUser = await GetCurrentUser();
            
            if (user == null || currentUser == null || (currentUser.UserID != user.UserID))
            {
                return RedirectToMainPage();
            }

            var editUser = new EditUser()
            {
                UserID = user.UserID,
                Username = user.Username,
                Email = user.Email
            };

            return View(editUser);
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
                var user = await _userRepository.FindAsync(id);
                if (user == null)
                {
                    return NotFound();
                }

                if (await ControllData(editUser.UserID, editUser.Username, editUser.Email))
                {
                    if (await _userRepository.EditAsync(id, editUser))
                    {
                        return RedirectToAction(nameof(Index));
                    }
                }
                else
                {
                    return View(editUser);
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
                return RedirectToMainPage();
            }

            if (id == null)
            {
                return NotFound();
            }

            var user = await _userRepository.GetById(id.Value);
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
            _ = await _userRepository.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
