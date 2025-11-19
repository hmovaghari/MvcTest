using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using MvcTest.ViewModels;
using MyAccounting.Data;
using MyAccounting.Data.Model;
using MyAccounting.ViewModels;
using System;
using System.Security.Principal;

namespace MyAccounting.Repository
{
    public class AccountPartyRepository
    {
        private SqlDBContext _context;

        public AccountPartyRepository(SqlDBContext context)
        {
            _context = context;
        }

        private async Task<List<Person>> GetAllAccountParties(bool? isPerson = null)
        {
            return await _context.People
                .Include(p => p.CurrencyUnit)
                .Include(p => p.User)
                .Where(p => isPerson == null || p.IsPerson == isPerson)
                .ToListAsync();
        }

        public async Task<List<PersonDTO>> GetPeople()
        {
            try
            {
                var people = await GetAllAccountParties(isPerson: true);

                return people.Select(p => new PersonDTO
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
            }
            catch (Exception ex)
            {
                ErrorLogRepository.SaveErrorLog(_context, ex, nameof(AccountPartyRepository), nameof(GetPeople), null);
            }

            return new List<PersonDTO>();
        }

        public async Task<List<AccountDTO>> GetAccounts()
        {
            try
            {
                var people = await GetAllAccountParties(isPerson: false);

                return people.Select(p => new AccountDTO()
                {
                    PersonID = p.PersonID,
                    UserID = p.UserID,
                    Name = p.Name,
                    CurrencyUnitID = p.CurrencyUnitID,
                    CurrencyUnitName = p.CurrencyUnit != null ? p.CurrencyUnit.Name : string.Empty,
                    BankAccountNumber = p.BankAccountNumber,
                    BankCard = p.BankCard,
                    BankShaba = p.BankShaba,
                    Description = p.Description
                }).ToList();
            }
            catch (Exception ex)
            {
                ErrorLogRepository.SaveErrorLog(_context, ex, nameof(AccountPartyRepository), nameof(GetAccounts), null);
            }

            return new List<AccountDTO>();
        }

        public async Task<(string, string)?> ControlData(Guid? personID, Guid userID, string name, Guid? oldCurrencyUnitID = null,
            Guid? newCurrencyUnitID = null)
        {
            try
            {
                if (await _context.People.AnyAsync(p =>
                        (personID == null || p.PersonID != personID) && p.UserID == userID && p.Name == name))
                {
                    return ("Name", "تکراری است");
                }

                if (personID != null && oldCurrencyUnitID != null && newCurrencyUnitID != null && oldCurrencyUnitID != newCurrencyUnitID &&
                    await _context.Transactions.AnyAsync(t => t.PayerPersonID == personID || t.ReceiverPersonID == personID))
                {
                    return ("CurrencyUnitID", "به دلیل استفاده در تراکنش‌ها امکان ویرایش نیست");
                }

                return null;
            }
            catch (Exception ex)
            {
                var input = (personID, userID, name, oldCurrencyUnitID, newCurrencyUnitID);
                ErrorLogRepository.SaveErrorLog(_context, ex, nameof(AccountPartyRepository), nameof(ControlData), input);
                return ("Name", "خطایی در بررسی داده‌ها رخ داده است");
            }

        }

        public async Task<bool> CreatePersonAsync(CreatePerson createPerson, Guid userID)
        {
            try
            {
                var person = new Person()
                {
                    PersonID = Guid.NewGuid(),
                    UserID = userID,
                    Name = createPerson.Name,
                    IsPerson = true,
                    CurrencyUnitID = createPerson.CurrencyUnitID,
                    PersonTell = createPerson.PersonTell,
                    PersonMobile = createPerson.PersonMobile,
                    PersonEmail = createPerson.PersonEmail,
                    PersonAddress = createPerson.PersonAddress,
                    Description = createPerson.Description
                };
                _context.Add(person);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                ErrorLogRepository.SaveErrorLog(_context, ex, nameof(AccountPartyRepository), nameof(CreatePersonAsync), (createPerson, userID));
            }

            return false;
        }

        public async Task<bool> CreateAccountAsync(CreateAccount createAccount, Guid userID)
        {
            try
            {
                var person = new Person()
                {
                    PersonID = Guid.NewGuid(),
                    UserID = userID,
                    Name = createAccount.Name,
                    IsPerson = false,
                    CurrencyUnitID = createAccount.CurrencyUnitID,
                    BankAccountNumber = createAccount.BankAccountNumber,
                    BankShaba = createAccount.BankShaba,
                    BankCard = createAccount.BankCard,
                    Description = createAccount.Description
                };
                _context.Add(person);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                ErrorLogRepository.SaveErrorLog(_context, ex, nameof(AccountPartyRepository), nameof(CreateAccountAsync), (createAccount, userID));
            }

            return false;
        }

        public async Task<AccountPartyDTO> FindAsync(Guid? id)
        {
            try
            {
                var accountParty = await _context.People.FindAsync(id);
                if (accountParty != null)
                {
                    return new AccountPartyDTO()
                    {
                        PersonID = accountParty.PersonID,
                        UserID = accountParty.UserID,
                        Name = accountParty.Name,
                        CurrencyUnitID = accountParty.CurrencyUnitID,
                        CurrencyUnitName = accountParty.CurrencyUnit?.Name,
                        PersonTell = accountParty.PersonTell,
                        PersonMobile = accountParty.PersonMobile,
                        PersonEmail = accountParty.PersonEmail,
                        PersonAddress = accountParty.PersonAddress,
                        BankAccountNumber = accountParty.BankAccountNumber,
                        BankShaba = accountParty.BankShaba,
                        BankCard = accountParty.BankCard,
                        Description = accountParty.Description
                    };
                }
            }
            catch (Exception ex)
            {
                ErrorLogRepository.SaveErrorLog(_context, ex, nameof(AccountPartyRepository), nameof(FindAsync), id);
            }

            return null;
        }

        public async Task<AccountPartyDTO> MapToAsync(EditPerson editPerson)
        {
            if (editPerson == null)
            {
                return null;
            }

            return new AccountPartyDTO()
            {
                PersonID = editPerson.PersonID,
                Name = editPerson.Name,
                CurrencyUnitID = editPerson.CurrencyUnitID,
                PersonTell = editPerson.PersonTell,
                PersonMobile = editPerson.PersonMobile,
                PersonEmail = editPerson.PersonEmail,
                PersonAddress = editPerson.PersonAddress,
                Description = editPerson.Description
            };
        }

        public async Task<AccountPartyDTO> MapToAsync(EditAccount editAccount)
        {
            if (editAccount == null)
            {
                return null;
            }

            return new AccountPartyDTO()
            {
                PersonID = editAccount.PersonID,
                Name = editAccount.Name,
                CurrencyUnitID = editAccount.CurrencyUnitID,
                BankAccountNumber = editAccount.BankAccountNumber,
                BankShaba = editAccount.BankShaba,
                BankCard = editAccount.BankCard,
                Description = editAccount.Description,
            };
        }

        public async Task<bool> EdiAsync(AccountPartyDTO accountPartyDto, bool isPerson)
        {
            try
            {
                var dbData = await _context.People.FindAsync(accountPartyDto.PersonID);

                dbData.Name = accountPartyDto.Name;
                dbData.CurrencyUnitID = accountPartyDto.CurrencyUnitID;
                dbData.Description = accountPartyDto.Description;
                if (isPerson)
                {
                    dbData.PersonTell = accountPartyDto.PersonTell;
                    dbData.PersonMobile = accountPartyDto.PersonMobile;
                    dbData.PersonAddress = accountPartyDto.PersonAddress;
                }
                else
                {
                    dbData.BankAccountNumber = accountPartyDto.BankAccountNumber;
                    dbData.BankShaba = accountPartyDto.BankShaba;
                    dbData.BankCard = accountPartyDto.BankCard;
                }
                _context.Update(dbData);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                ErrorLogRepository.SaveErrorLog(_context, ex, nameof(AccountPartyRepository), nameof(EdiAsync), (accountPartyDto, isPerson));
            }

            return false;
        }
    }
}
