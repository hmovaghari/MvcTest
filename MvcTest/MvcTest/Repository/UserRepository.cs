using Microsoft.EntityFrameworkCore;
using MyAccounting.Data;
using MyAccounting.Data.Model;
using MyAccounting.ViewModels;
using System.Security.Cryptography;
using System.Text;

namespace MyAccounting.Repository
{
    public class UserRepository
    {
        private SqlDBContext _context;

        public UserRepository(SqlDBContext contex)
        {
            _context = contex;
        }

        public async Task<List<UserDTO>> GetAllUsersAsync()
        {
            try
            {
                var users = await _context.Users.ToListAsync();
                var userDTOs = users.Select(x => new UserDTO()
                {
                    UserID = x.UserID,
                    Username = x.Username,
                    Email = x.Email,
                    IsActive = x.IsActive,
                    IsAdmin = x.IsAdmin,
                }).ToList();
                return userDTOs;
            }
            catch (Exception ex)
            {
                ErrorLogRepository.SaveErrorLog(_context, ex, nameof(UserRepository), nameof(GetAllUsersAsync), null);
            }

            return new List<UserDTO>();
        }

        public async Task<bool> CreateUserAsync(CreateUser createUser)
        {
            try
            {
                var user = new User
                {
                    UserID = Guid.NewGuid(),
                    Username = createUser.Username,
                    Email = createUser.Email,
                    Salt1 = Guid.NewGuid().ToString(),
                    Salt2 = Guid.NewGuid().ToString(),
                    IsActive = true,
                    IsAdmin = false
                };
                user.Password = GenerateHashedPassword(createUser.Password, user.Salt1, user.Salt2);
                _context.Add(user);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                ErrorLogRepository.SaveErrorLog(_context, ex, nameof(UserRepository), nameof(CreateUserAsync),
                    createUser);
            }

            return false;
        }

        private string GenerateHashedPassword(string password, string salt1, string salt2)
        {
            var text = salt1.Replace("-", "") + password + salt2.Replace("-", "");
            using var sha256 = SHA256.Create();
            var data = Encoding.UTF8.GetBytes(text ?? string.Empty);
            var hashBytes = sha256.ComputeHash(data);
            var hashString = BitConverter.ToString(hashBytes).Replace("-", "").ToLowerInvariant();
            return hashString;
        }

        public async Task<(string, string)?> ControllData(Guid? userID, string username, string email)
        {
            try
            {
                // بررسی تکراری بودن نام کاربری (به جز خود کاربر)
                if (await _context.Users.AnyAsync(u =>
                        (userID == null || u.UserID != userID) && u.Username == username))
                {
                    return ("Username", "این نام کاربری قبلاً استفاده شده است");
                }

                // بررسی تکراری بودن ایمیل (به جز خود کاربر)
                if (await _context.Users.AnyAsync(u => (userID == null || u.UserID != userID) && u.Email == email))
                {
                    return ("Email", "این ایمیل قبلاً استفاده شده است");
                }

                return null;
            }
            catch (Exception ex)
            {
                var input = (userID, username, email);
                ErrorLogRepository.SaveErrorLog(_context, ex, nameof(UserRepository), nameof(ControllData), input);
                return ("Username", "خطایی در بررسی داده‌ها رخ داده است");
            }
        }

        public async Task<UserDTO?> FindAsync(Guid? id)
        {
            try
            {
                var user = await _context.Users.FindAsync(id);
                if (user != null)
                {
                    return new UserDTO()
                    {
                        UserID = user.UserID,
                        Username = user.Username,
                        Email = user.Email,
                        IsActive = user.IsActive,
                        IsAdmin = user.IsAdmin,
                    };
                }
            }
            catch (Exception ex)
            {
                ErrorLogRepository.SaveErrorLog(_context, ex, nameof(UserRepository), nameof(FindAsync), id);
            }

            return null;
        }

        public async Task<bool> Edit(Guid id, EditUser editUser)
        {
            try
            {
                var user = await _context.Users.FindAsync(id);
                if (user == null)
                {
                    return false;
                }

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
                return true;
            }
            catch (Exception ex)
            {
                ErrorLogRepository.SaveErrorLog(_context, ex, nameof(UserRepository), nameof(Edit), (id, editUser));
                return false;
            }
        }

        public async Task<UserDTO?> GetById(Guid id)
        {
            try
            {
                var user = await _context.Users.FirstOrDefaultAsync(m => m.UserID == id);
                if (user != null)
                {
                    return new UserDTO()
                    {
                        UserID = user.UserID,
                        Username = user.Username,
                        Email = user.Email,
                        IsActive = user.IsActive,
                        IsAdmin = user.IsAdmin,
                    };
                }

                return null;
            }
            catch (Exception ex)
            {
                ErrorLogRepository.SaveErrorLog(_context, ex, nameof(UserRepository), nameof(GetById), id);
                return null;
            }
        }

        public async Task<bool> Delete(Guid id)
        {
            try
            {
                var user = await _context.Users.FindAsync(id);
                if (user != null)
                {
                    _context.Users.Remove(user);
                    await _context.SaveChangesAsync();
                    return true;
                }
            }
            catch (Exception ex)
            {
                ErrorLogRepository.SaveErrorLog(_context, ex, nameof(UserRepository), nameof(Delete), id);
            }

            return false;
        }

        public async Task<(bool?, UserDTO?)> CheckPassword(LoginViewModel model)
        {
            try
            {
                var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == model.Username);
                if (user != null)
                {
                    var hashedPassword = GenerateHashedPassword(model.Password, user.Salt1, user.Salt2);
                    if (hashedPassword == user.Password)
                    {
                        var userDTO = new UserDTO()
                        {
                            UserID = user.UserID,
                            Username = user.Username,
                            Email = user.Email,
                            IsActive = user.IsActive,
                            IsAdmin = user.IsAdmin,
                        };
                        return (true, userDTO);
                    }
                    return (false, null);
                }
            }
            catch (Exception ex)
            {
                ErrorLogRepository.SaveErrorLog(_context, ex, nameof(CheckPassword), nameof(Delete), model);
            }

            return (null, null);
        }

        public async Task<UserDTO?> GetUserByUsername(string username)
        {
            try
            {
                var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == username);
                return new UserDTO() 
                {
                    UserID = user.UserID,
                    Username = user.Username,
                    Email = user.Email,
                    IsActive = user.IsActive,
                    IsAdmin = user.IsAdmin,
                };
            }
            catch (Exception ex)
            {
                ErrorLogRepository.SaveErrorLog(_context, ex, nameof(UserRepository), nameof(GetUserByUsername), username);
            }
            return null;
        }
    }
}
