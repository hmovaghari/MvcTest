using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MyAccounting.Data;
using MyAccounting.Data.Model;
using MyAccounting.ViewModels;
using MyAccounting.Repository;

namespace MyAccounting.Controllers
{
    public class PeopleController : BaseAuthorizeController
    {
        AccountPartyRepository _accountPartyRepository;

        public PeopleController(SqlDBContext context) : base(context)
        {
            _accountPartyRepository = new AccountPartyRepository(context);
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

            var list = (
                await _context.People
                .Include(p => p.CurrencyUnit)
                .Include(p => p.User)
                .Where(p => p.UserID == user.UserID && p.IsPerson).ToListAsync()
                ).Select(p => new PersonDTO()
            {
                    PersonID = p.PersonID,
                    UserID = p.UserID,
                    Name = p.Name,
                    CurrencyUnitID = p.CurrencyUnitID,
                    CurrencyUnitName = p.CurrencyUnit != null ? p.CurrencyUnit.Name : string.Empty,
                    PersonTell = p.PersonTell,
                    PersonMobile = p.PersonMobile,
                    PersonEmail = p.PersonEmail,
                    PersonAddress = p.PersonAddress,
                    Description = p.Description
                }).ToList();
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
            ViewData["CurrencyUnitID"] = new SelectList(_context.CurrencyUnits, "CurrencyUnitID", "Name");
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

            var person = new Person();
            person.UserID = user.UserID;
            if (ModelState.IsValid)
            {
                if (await ControlData(personID: null, user.UserID, createPerson.Name))
                {
                    person.PersonID = Guid.NewGuid();
                    person.Name = createPerson.Name;
                    person.IsPerson = true;
                    person.CurrencyUnitID = createPerson.CurrencyUnitID;
                    person.PersonTell = createPerson.PersonTell;
                    person.PersonMobile = createPerson.PersonMobile;
                    person.PersonEmail = createPerson.PersonEmail;
                    person.PersonAddress = createPerson.PersonAddress;
                    person.Description = createPerson.Description;
                    _context.Add(person);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
            }
            ViewData["CurrencyUnitID"] = new SelectList(_context.CurrencyUnits, "CurrencyUnitID", "Name", createPerson.CurrencyUnitID);
            return View(createPerson);
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

        // GET: People/Edit/5
        [Authorize]
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var currentUser = await GetCurrentUser();
            var person = await _context.People.FindAsync(id);
            if (person == null || currentUser == null || currentUser.UserID != person.UserID)
            {
                return RedirectToMainPage();
            }
            ViewData["CurrencyUnitID"] = new SelectList(_context.CurrencyUnits, "CurrencyUnitID", "Name", person.CurrencyUnitID);
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
                try
                {
                    var person = await _context.People.FindAsync(id);
                    if (person == null)
                    {
                        return NotFound();
                    }

                    currencyUnitID = person.CurrencyUnitID;

                    if (await ControlData(person.PersonID, person.UserID, editPerson.Name, person.CurrencyUnitID, editPerson.CurrencyUnitID))
                    {
                        person.Name = editPerson.Name;
                        person.CurrencyUnitID = editPerson.CurrencyUnitID;
                        person.PersonTell = editPerson.PersonTell;
                        person.PersonMobile = editPerson.PersonMobile;
                        person.PersonAddress = editPerson.PersonAddress;
                        person.Description = editPerson.Description;
                        _context.Update(person);
                        await _context.SaveChangesAsync();
                    }
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PersonExists(editPerson.PersonID))
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

            var person = await _context.People
                .Include(p => p.CurrencyUnit)
                .Include(p => p.User)
                .FirstOrDefaultAsync(m => m.PersonID == id);
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
            var person = await _context.People.FindAsync(id);
            if (person != null)
            {
                _context.People.Remove(person);
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
