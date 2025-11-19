using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyAccounting.Data;
using MyAccounting.ViewModels;
using MyAccounting.Repository;

namespace MyAccounting.Controllers
{
    public class PeopleController : BaseAuthorizeController
    {
        private AccountPartyRepository _accountPartyRepository;
        private CurrencyUnitRepository _currencyUnitRepository;

        public PeopleController(SqlDBContext context) : base(context)
        {
            _accountPartyRepository = new AccountPartyRepository(context);
            _currencyUnitRepository = new CurrencyUnitRepository(context);
        }

        // GET: People
        [Authorize]
        public async Task<IActionResult> Index()
        {
            var user = await GetCurrentUser();
            if (user == null)
            {
                return RedirectToMainPage();
            }

            var list = await _accountPartyRepository.GetPeople(user.UserID);
            return View(list);
        }

        // GET: People/Create
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

        // POST: People/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name,CurrencyUnitID,PersonTell,PersonMobile,PersonEmail,PersonAddress,Description")] CreatePerson createPerson)
        {
            var user = await GetCurrentUser();
            if (user == null)
            {
                return RedirectToMainPage();
            }

            if (ModelState.IsValid)
            {
                if (await ControlData(personID: null, user.UserID, createPerson.Name))
                {
                    var accountParty = _accountPartyRepository.MapToAccountPartyAsync(createPerson);
                    if (await _accountPartyRepository.CreateAccountPartyAsync(accountParty, user.UserID, isPerson: true))
                    {
                        return RedirectToAction(nameof(Index));
                    }
                }
            }
            ViewData["CurrencyUnitID"] = _currencyUnitRepository.GetSelectList(createPerson.CurrencyUnitID);
            return View(createPerson);
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

        // GET: People/Edit/5
        [Authorize]
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var currentUser = await GetCurrentUser();
            var person = await _accountPartyRepository.FindAsync(id);
            if (person == null || currentUser == null || currentUser.UserID != person.UserID)
            {
                return RedirectToMainPage();
            }
            ViewData["CurrencyUnitID"] = _currencyUnitRepository.GetSelectList(person.CurrencyUnitID);
            var editPerson = new EditPerson()
            {
                PersonID = person.PersonID,
                Name = person.Name,
                CurrencyUnitID = person.CurrencyUnitID,
                PersonEmail = person.PersonEmail,
                PersonTell = person.PersonTell,
                PersonMobile = person.PersonMobile,
                PersonAddress = person.PersonAddress,
                Description = person.Description
            };
            return View(editPerson);
        }

        // POST: People/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("PersonID,Name,CurrencyUnitID,PersonTell,PersonMobile,PersonEmail,PersonAddress,Description")] EditPerson editPerson)
        {
            if (id != editPerson.PersonID)
            {
                return NotFound();
            }

            var currencyUnitID = editPerson.CurrencyUnitID;

            if (ModelState.IsValid)
            {
                var person = await _accountPartyRepository.FindAsync(id);
                if (person == null)
                {
                    return NotFound();
                }

                currencyUnitID = person.CurrencyUnitID;

                if (await ControlData(person.PersonID, person.UserID, editPerson.Name, person.CurrencyUnitID, editPerson.CurrencyUnitID))
                {
                    var accountParty = _accountPartyRepository.MapToAccountPartyAsync(editPerson);

                    if (await _accountPartyRepository.EdiAsync(accountParty, isPerson: false))
                    {
                        return RedirectToAction(nameof(Index));
                    }
                }
            }
            ViewData["CurrencyUnitID"] = _currencyUnitRepository.GetSelectList(currencyUnitID);
            return View(editPerson);
        }

        // GET: People/Delete/5
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

            var person = await _accountPartyRepository.GetByIdAsync(id.Value);
            if (person == null)
            {
                return NotFound();
            }

            if (currentUser.UserID != person.UserID)
            {
                return RedirectToMainPage();
            }

            var personDTO = new PersonDTO()
            {
                PersonID = person.PersonID,
                Name = person.Name,
            };

            return View(personDTO);
        }

        // POST: People/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            _ = await _accountPartyRepository.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
