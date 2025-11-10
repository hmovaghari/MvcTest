using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MyAccounting.Data;
using MyAccounting.Data.Model;

namespace MvcTest.Controllers
{
    public class AccountPartiesController : Controller
    {
        private readonly SqlDBContext _context;

        public AccountPartiesController(SqlDBContext context)
        {
            _context = context;
        }

        // GET: AccountParties
        public async Task<IActionResult> Index()
        {
            var sqlDBContext = _context.People.Include(p => p.CurrencyUnit).Include(p => p.User);
            return View(await sqlDBContext.ToListAsync());
        }

        // GET: AccountParties/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
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

            return View(person);
        }

        // GET: AccountParties/Create
        public IActionResult Create()
        {
            ViewData["CurrencyUnitID"] = new SelectList(_context.CurrencyUnits, "CurrencyUnitID", "CurrencyUnitID");
            ViewData["UserID"] = new SelectList(_context.Users, "UserID", "Username");
            return View();
        }

        // POST: AccountParties/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("PersonID,UserID,Name,IsPerson,PersonTell,PersonMobile,PersonEmail,PersonAddress,BankAccountNumber,BankShaba,BankCard,Description,CurrencyUnitID")] Person person)
        {
            if (ModelState.IsValid)
            {
                person.PersonID = Guid.NewGuid();
                _context.Add(person);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CurrencyUnitID"] = new SelectList(_context.CurrencyUnits, "CurrencyUnitID", "CurrencyUnitID", person.CurrencyUnitID);
            ViewData["UserID"] = new SelectList(_context.Users, "UserID", "Username", person.UserID);
            return View(person);
        }

        // GET: AccountParties/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var person = await _context.People.FindAsync(id);
            if (person == null)
            {
                return NotFound();
            }
            ViewData["CurrencyUnitID"] = new SelectList(_context.CurrencyUnits, "CurrencyUnitID", "CurrencyUnitID", person.CurrencyUnitID);
            ViewData["UserID"] = new SelectList(_context.Users, "UserID", "Username", person.UserID);
            return View(person);
        }

        // POST: AccountParties/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("PersonID,UserID,Name,IsPerson,PersonTell,PersonMobile,PersonEmail,PersonAddress,BankAccountNumber,BankShaba,BankCard,Description,CurrencyUnitID")] Person person)
        {
            if (id != person.PersonID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(person);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PersonExists(person.PersonID))
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
            ViewData["CurrencyUnitID"] = new SelectList(_context.CurrencyUnits, "CurrencyUnitID", "CurrencyUnitID", person.CurrencyUnitID);
            ViewData["UserID"] = new SelectList(_context.Users, "UserID", "Username", person.UserID);
            return View(person);
        }

        // GET: AccountParties/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
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

            return View(person);
        }

        // POST: AccountParties/Delete/5
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
