using Microsoft.EntityFrameworkCore;
using MvcTest.ViewModels;
using MyAccounting.Data;
using MyAccounting.Data.Model;
using MyAccounting.ViewModels;

namespace MyAccounting.Repository
{
    public class AccountPartyRepository
    {
        private SqlDBContext _context;

        public AccountPartyRepository(SqlDBContext context)
        {
            _context = context;
        }

        private async Task<List<Person>> GetAllAccountParties(Guid userId, bool? isPerson = null)
        {
            return await _context.People
                .Include(p => p.CurrencyUnit)
                .Include(p => p.User)
                .Where(p => isPerson == null || p.IsPerson == isPerson)
                .Where(p => p.UserID == userId)
                .ToListAsync();
        }

        public async Task<List<PersonDTO>> GetPeople(Guid userId)
        {
            try
            {
                var people = await GetAllAccountParties(userId, isPerson: true);

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

        public async Task<List<AccountDTO>> GetAccounts(Guid userId)
        {
            try
            {
                var people = await GetAllAccountParties(userId, isPerson: false);

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

        public AccountPartyDTO MapToAccountPartyAsync(CreatePerson createPerson)
        {
            if (createPerson == null)
            {
                return null;
            }
            return new AccountPartyDTO()
            {
                PersonID = Guid.NewGuid(),
                Name = createPerson.Name,
                CurrencyUnitID = createPerson.CurrencyUnitID,
                PersonTell = createPerson.PersonTell,
                PersonMobile = createPerson.PersonMobile,
                PersonEmail = createPerson.PersonEmail,
                PersonAddress = createPerson.PersonAddress,
                Description = createPerson.Description
            };
        }

        public AccountPartyDTO MapToAccountPartyAsync(CreateAccount createAccount)
        {
            if (createAccount == null)
            {
                return null;
            }
            return new AccountPartyDTO()
            {
                PersonID = Guid.NewGuid(),
                Name = createAccount.Name,
                CurrencyUnitID = createAccount.CurrencyUnitID,
                BankAccountNumber = createAccount.BankAccountNumber,
                BankShaba = createAccount.BankShaba,
                BankCard = createAccount.BankCard,
                Description = createAccount.Description
            };
        }

        public async Task<bool> CreateAccountPartyAsync(AccountPartyDTO accountPartyDto, Guid userID, bool isPerson)
        {
            try
            {
                var person = new Person()
                {
                    PersonID = Guid.NewGuid(),
                    UserID = userID,
                    Name = accountPartyDto.Name,
                    IsPerson = isPerson,
                    CurrencyUnitID = accountPartyDto.CurrencyUnitID,
                    Description = accountPartyDto.Description
                };
                if (isPerson)
                {
                    person.PersonTell = accountPartyDto.PersonTell;
                    person.PersonMobile = accountPartyDto.PersonMobile;
                    person.PersonEmail = accountPartyDto.PersonEmail;
                    person.PersonAddress = accountPartyDto.PersonAddress;
                }
                else
                {
                    person.BankAccountNumber = accountPartyDto.BankAccountNumber;
                    person.BankShaba = accountPartyDto.BankShaba;
                    person.BankCard = accountPartyDto.BankCard;
                }
                _context.Add(person);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                ErrorLogRepository.SaveErrorLog(_context, ex, nameof(AccountPartyRepository), nameof(CreateAccountPartyAsync), (accountPartyDto, userID, isPerson));
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

        public AccountPartyDTO MapToAccountPartyAsync(EditPerson editPerson)
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

        public AccountPartyDTO MapToAccountPartyAsync(EditAccount editAccount)
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
                    dbData.PersonEmail = accountPartyDto.PersonEmail;
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

        public async Task<AccountPartyDTO> GetByIdAsync(Guid id)
        {
            try
            {
                var person = await _context.People
                    .Include(p => p.CurrencyUnit)
                    .Include(p => p.User)
                    .FirstOrDefaultAsync(m => m.PersonID == id);
                if (person != null)
                {
                    return new AccountPartyDTO()
                    {
                        PersonID = person.PersonID,
                        UserID = person.UserID,
                        Name = person.Name,
                        CurrencyUnitID = person.CurrencyUnitID,
                        CurrencyUnitName = person.CurrencyUnit != null ? person.CurrencyUnit.Name : string.Empty,
                        PersonTell = person.PersonTell,
                        PersonMobile = person.PersonMobile,
                        PersonEmail = person.PersonEmail,
                        PersonAddress = person.PersonAddress,
                        BankAccountNumber = person.BankAccountNumber,
                        BankShaba = person.BankShaba,
                        BankCard = person.BankCard,
                        Description = person.Description
                    };
                }

                return null;
            }
            catch (Exception ex)
            {
                ErrorLogRepository.SaveErrorLog(_context, ex, nameof(AccountPartyRepository), nameof(GetByIdAsync), id);
                return null;
            }
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            try
            {
                var person = await _context.People.FindAsync(id);
                if (person != null)
                {
                    _context.People.Remove(person);
                    await _context.SaveChangesAsync();
                    return true;
                }
            }
            catch (Exception ex)
            {
                ErrorLogRepository.SaveErrorLog(_context, ex, nameof(AccountPartyRepository), nameof(DeleteAsync), id);
            }
            return false;
        }
    }
}
