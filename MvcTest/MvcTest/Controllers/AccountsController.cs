using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MyAccounting.Data;
using MyAccounting.Data.Model;
using MyAccounting.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MvcTest.Controllers
{
    public class AccountsController : BaseAuthorizeController
    {
        public AccountsController(SqlDBContext context) : base(context)
    {
            
        }

        // GET: Accounts
        [Authorize]
        public async Task<IActionResult> Index()
        {
            var user = await GetCurrentUser();
            if (user == null)
            {
                return RedirectToMainPage();
            }

            var list = (
                await _context.People
                .Include(a => a.CurrencyUnit)
                .Include(a => a.User)
                .Where(a => a.UserID == user.UserID && !a.IsPerson).ToListAsync()
                ).Select(a => new AccountDTO()
                {
                    PersonID = a.PersonID,
                    UserID = a.UserID,
                    Name = a.Name,
                    CurrencyUnitID = a.CurrencyUnitID,
                    CurrencyUnitName = a.CurrencyUnit != null ? a.CurrencyUnit.Name : string.Empty,
                    BankAccountNumber = a.BankAccountNumber,
                    BankCard = a.BankCard,
                    BankShaba = a.BankShaba,
                    Description = a.Description
                }).ToList();
            return View(list);
        }

        // GET: Accounts/Create
        [Authorize]
        public async Task<IActionResult> Create()
        {
            var user = await GetCurrentUser();
            if (user == null)
            {
                return RedirectToMainPage();
            }
            ViewData["CurrencyUnitID"] = new SelectList(_context.CurrencyUnits, "CurrencyUnitID", "Name");
            return View();
        }

        // POST: Accounts/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("PersonID,CurrencyUnitID,UserID,Name,BankAccountNumber,BankShaba,BankCard,Description")] CreateAccount createAccount)
        {
            var user = await GetCurrentUser();
            if (user == null)
            {
                return RedirectToMainPage();
            }

            var account = new Person();
            account.UserID = user.UserID;
            if (ModelState.IsValid)
            {
                if (await ControlData(personID: null, user.UserID, createAccount.Name))
                {
                    account.PersonID = Guid.NewGuid();
                    account.Name = createAccount.Name;
                    account.IsPerson = false;
                    account.CurrencyUnitID = createAccount.CurrencyUnitID;
                    account.BankAccountNumber = createAccount.BankAccountNumber;
                    account.BankShaba = createAccount.BankShaba;
                    account.BankCard = createAccount.BankCard;
                    account.Description = createAccount.Description;
                    _context.Add(account);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
            }
            ViewData["CurrencyUnitID"] = new SelectList(_context.CurrencyUnits, "CurrencyUnitID", "Name", createAccount.CurrencyUnitID);
            return View(account);
        }

        private async Task<bool> ControlData(Guid? personID, Guid userID, string name, Guid? oldCurrencyUnitID = null, Guid? newCurrencyUnitID = null)
        {
            // بررسی تکراری بودن نام طرف حساب (به جز خود طرف حساب)
            if (await _context.People.AnyAsync(p => (personID == null || p.PersonID != personID) && p.UserID == userID && p.Name == name))
            {
                ModelState.AddModelError("Name", "این نام قبلاً استفاده شده است");
                return false;
            }

            if (personID != null && oldCurrencyUnitID != null && newCurrencyUnitID != null && oldCurrencyUnitID != newCurrencyUnitID &&
                await _context.Transactions.AnyAsync(t => t.PayerPersonID == personID || t.ReceiverPersonID == personID))
            {
                ModelState.AddModelError("CurrencyUnitID", "به دلیل استفاده در تراکنش‌ها امکان ویرایش نیست");
                return false;
            }

            return true;
        }

        // GET: Accounts/Edit/5
        [Authorize]
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var currentUser = await GetCurrentUser();
            var account = await _context.People.FindAsync(id);
            if (account == null || currentUser == null || currentUser.UserID != account.UserID)
            {
                return RedirectToMainPage();
            }
            ViewData["CurrencyUnitID"] = new SelectList(_context.CurrencyUnits, "CurrencyUnitID", "Name", account.CurrencyUnitID);
            var editBank = new EditAccount()
            {
                PersonID = account.PersonID,
                Name = account.Name,
                CurrencyUnitID = account.CurrencyUnitID,
                BankAccountNumber = account.BankAccountNumber,
                BankShaba = account.BankShaba,
                BankCard = account.BankCard,
                Description = account.Description
            };
            return View(editBank);
        }

        // POST: Accounts/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("PersonID,UserID,Name,CurrencyUnitID,BankAccountNumber,BankShaba,BankCard,Description")] EditAccount editAccount)
        {
            if (id != editAccount.PersonID)
            {
                return NotFound();
            }

            var currencyUnitID = editAccount.CurrencyUnitID;

            if (ModelState.IsValid)
            {
                try
                {
                    var account = await _context.People.FindAsync(id);
                    if (account == null)
                    {
                        return NotFound();
                    }

                    currencyUnitID = account.CurrencyUnitID;

                    if (await ControlData(account.PersonID, account.UserID, account.Name, account.CurrencyUnitID, editAccount.CurrencyUnitID))
                    {
                        account.Name = editAccount.Name;
                        account.CurrencyUnitID = editAccount.CurrencyUnitID;
                        account.BankAccountNumber = editAccount.BankAccountNumber;
                        account.BankShaba = editAccount.BankShaba;
                        account.BankCard = editAccount.BankCard;
                        account.Description = editAccount.Description;
                        _context.Update(account);
                        await _context.SaveChangesAsync();
                    }
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PersonExists(editAccount.PersonID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["CurrencyUnitID"] = new SelectList(_context.CurrencyUnits, "CurrencyUnitID", "Name", currencyUnitID);
            return View(editAccount);
        }

        // GET: Accounts/Delete/5
        [Authorize]
        public async Task<IActionResult> Delete(Guid? id)
        {
            var currentUser = await GetCurrentUser();
            if (currentUser == null)
            {
                return RedirectToMainPage();
            }

            if (id == null)
            {
                return NotFound();
            }

            var account = await _context.People
                .Include(p => p.CurrencyUnit)
                .Include(p => p.User)
                .FirstOrDefaultAsync(m => m.PersonID == id);
            if (account == null)
            {
                return NotFound();
            }

            if (currentUser.UserID != account.UserID)
            {
                return RedirectToMainPage();
            }

            var accountDTO = new AccountDTO()
            {
                PersonID = account.PersonID,
                Name = account.Name,
            };

            return View(accountDTO);
        }

        // POST: Accounts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var account = await _context.People.FindAsync(id);
            if (account != null)
            {
                _context.People.Remove(account);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PersonExists(Guid id)
        {
            return _context.People.Any(e => e.PersonID == id);
        }
    }
}
