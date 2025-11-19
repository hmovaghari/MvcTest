using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyAccounting.Data;
using MyAccounting.Repository;
using MyAccounting.ViewModels;

namespace MyAccounting.Controllers
{
    public class AccountsController : BaseAuthorizeController
    {
        private AccountPartyRepository _accountPartyRepository;
        private CurrencyUnitRepository _currencyUnitRepository;

        public AccountsController(SqlDBContext context) : base(context)
        {
            _accountPartyRepository = new AccountPartyRepository(context);
            _currencyUnitRepository = new CurrencyUnitRepository(context);
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

            var list = await _accountPartyRepository.GetAccounts(user.UserID);
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

            ViewData["CurrencyUnitID"] = _currencyUnitRepository.GetSelectList();
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

            if (ModelState.IsValid)
            {
                if (await ControlData(personID: null, user.UserID, createAccount.Name))
                {
                    var accountParty = _accountPartyRepository.MapToAccountPartyAsync(createAccount);
                    if (await _accountPartyRepository.CreateAccountPartyAsync(accountParty, user.UserID, isPerson: false))
                    {
                        return RedirectToAction(nameof(Index));
                    }
                }
            }
            ViewData["CurrencyUnitID"] = _currencyUnitRepository.GetSelectList(createAccount.CurrencyUnitID);
            return View(createAccount);
        }

        private async Task<bool> ControlData(Guid? personID, Guid userID, string name, Guid? oldCurrencyUnitID = null, Guid? newCurrencyUnitID = null)
        {
            var controlData = await _accountPartyRepository.ControlData(personID, userID, name, oldCurrencyUnitID, newCurrencyUnitID);
            if (controlData != null)
            {
                ModelState.AddModelError(controlData.Value.Item1, controlData.Value.Item2);
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
            var account = await _accountPartyRepository.FindAsync(id);
            if (account == null || currentUser == null || currentUser.UserID != account.UserID)
            {
                return RedirectToMainPage();
            }
            ViewData["CurrencyUnitID"] = _currencyUnitRepository.GetSelectList(account.CurrencyUnitID);
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
                var account = await _accountPartyRepository.FindAsync(id);
                if (account == null)
                {
                    return NotFound();
                }

                currencyUnitID = account.CurrencyUnitID;

                if (await ControlData(account.PersonID, account.UserID, account.Name, account.CurrencyUnitID, editAccount.CurrencyUnitID))
                {
                    var accountParty = _accountPartyRepository.MapToAccountPartyAsync(editAccount);

                    if (await _accountPartyRepository.EdiAsync(accountParty, isPerson: false))
                    {
                        return RedirectToAction(nameof(Index));
                    }
                }
            }
            ViewData["CurrencyUnitID"] = _currencyUnitRepository.GetSelectList(currencyUnitID);
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

            var account = await _accountPartyRepository.GetByIdAsync(id.Value);
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
            _ = _accountPartyRepository.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
