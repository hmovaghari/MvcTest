using Microsoft.EntityFrameworkCore;
using MyAccounting.Data;
using MyAccounting.Data.Model;
using MyAccounting.ViewModels;
using System.Runtime.CompilerServices;

namespace MyAccounting.Repository
{
    public class ApiKeyRepository
    {
        private SqlDBContext _context;
        private UserRepository _userRepository;

        public ApiKeyRepository(SqlDBContext context)
        {
            _context = context;
            _userRepository = new UserRepository(context);
        }

        public async Task<List<ApiKeyDTO>> GetAllApiKeysAsync([CallerMemberName] string callerName = "")
        {
            try
            {
                var list = await _context.ApiKeys.ToListAsync();
                return list.Select(x => MapToDto(x)).ToList();
            }
            catch (Exception ex)
            {
                ErrorLogRepository.SaveErrorLog(_context, ex, nameof(ApiKeyRepository), nameof(GetAllApiKeysAsync), callerName, null);
            }
            return new List<ApiKeyDTO>();
        }

        public async Task<bool> CreateApiKeyAsync(ApiKeyDTO apiKeyDto, [CallerMemberName] string callerName = "")
        {
            try
            {
                var apiKey = new ApiKey
                {
                    ApiKeyID = Guid.NewGuid(),
                    UserID = apiKeyDto.UserID,
                    IsActive = apiKeyDto.IsActive,
                    IsAdmin = apiKeyDto.IsAdmin
                };
                _context.Add(apiKey);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                ErrorLogRepository.SaveErrorLog(_context, ex, nameof(ApiKeyRepository), nameof(CreateApiKeyAsync), callerName, apiKeyDto);
            }
            return false;
        }

        public async Task<ApiKeyDTO> GetById(Guid id, [CallerMemberName] string callerName = "")
        {
            try
            {
                var apiKey = await _context.ApiKeys
                    .Include(a => a.User)
                    .FirstOrDefaultAsync(m => m.ApiKeyID == id);
                if (apiKey != null)
                {
                    return MapToDto(apiKey);
                }
            }
            catch (Exception ex)
            {
                ErrorLogRepository.SaveErrorLog(_context, ex, nameof(ApiKeyRepository), nameof(GetById), callerName, id);
            }

            return null;
        }

        public async Task<ApiKeyDTO> FindAsync(Guid? id, [CallerMemberName] string callerName = "")
        {
            try
            {
                var apiKey = await _context.ApiKeys.FindAsync(id);
                if (apiKey != null)
                {
                    return MapToDto(apiKey);
                }
            }
            catch (Exception ex)
            {
                ErrorLogRepository.SaveErrorLog(_context, ex, nameof(ApiKeyRepository), nameof(FindAsync), callerName, id);
            }
            return null!;
        }

        public async Task<bool> EditApiKeyAsync(ApiKeyDTO apiKeyDto, [CallerMemberName] string callerName = "")
        {
            try
            {
                var apiKey = await _context.ApiKeys.FindAsync(apiKeyDto.ApiKeyID);
                if (apiKey != null)
                {
                    apiKey.UserID = apiKeyDto.UserID;
                    apiKey.IsActive = apiKeyDto.IsActive;
                    apiKey.IsAdmin = apiKeyDto.IsAdmin;
                    _context.Update(apiKey);
                    await _context.SaveChangesAsync();
                    return true;
                }
            }
            catch (Exception ex)
            {
                ErrorLogRepository.SaveErrorLog(_context, ex, nameof(ApiKeyRepository), nameof(EditApiKeyAsync), callerName, apiKeyDto);
            }
            return false;
        }

        public async Task<bool> DeleteApiKeyAsync(Guid id, [CallerMemberName] string callerName = "")
        {
            try
            {
                var apiKey = await _context.ApiKeys.FindAsync(id);
                if (apiKey != null)
                {
                    _context.ApiKeys.Remove(apiKey);
                    await _context.SaveChangesAsync();
                    return true;
                }
            }
            catch (Exception ex)
            {
                ErrorLogRepository.SaveErrorLog(_context, ex, nameof(ApiKeyRepository), nameof(DeleteApiKeyAsync),
                    callerName, id);
            }
            return false;
        }

        private ApiKeyDTO MapToDto(ApiKey apiKey, [CallerMemberName] string callerName = "")
        {
            try
            {
                if (apiKey == null)
                {
                    return null;
                }
                return new ApiKeyDTO()
                {
                    ApiKeyID = apiKey.ApiKeyID,
                    UserID = apiKey.UserID,
                    IsActive = apiKey.IsActive,
                    IsAdmin = apiKey.IsAdmin,
                    UserDto = _userRepository.MapToDto(apiKey.User)
                };
            }
            catch (Exception ex)
            {
                ErrorLogRepository.SaveErrorLog(_context, ex, nameof(ApiKeyRepository), nameof(MapToDto),
                    callerName, null);
            }

            return null;
        }

        public async Task<bool> IsValidApiKeyAsync(string apiKey, [CallerMemberName] string callerName = "")
        {
            try
            {
                var isPars = Guid.TryParse(apiKey, out Guid _apiKey);
                if (isPars)
                {
                    return _context.ApiKeys.Any(ak => ak.ApiKeyID == _apiKey && ak.IsActive);
                }

                return false;
            }
            catch (Exception ex)
            {
                ErrorLogRepository.SaveErrorLog(_context, ex, nameof(ApiKeyRepository), nameof(IsValidApiKeyAsync),
                    callerName, apiKey);
            }

            return false;
        }

        public async Task<bool> IsValidAdminApiKeyAsync(string apiKey, [CallerMemberName] string callerName = "")
        {
            try
            {
                var isPars = Guid.TryParse(apiKey, out Guid _apiKey);
                if (isPars)
                {
                    return await _context.ApiKeys.AnyAsync(ak => ak.ApiKeyID == _apiKey && ak.IsActive && ak.IsAdmin);
                }

                return false;
            }
            catch (Exception ex)
            {
                ErrorLogRepository.SaveErrorLog(_context, ex, nameof(ApiKeyRepository), nameof(IsValidAdminApiKeyAsync),
                    callerName, apiKey);
            }

            return false;
        }
    }
}
