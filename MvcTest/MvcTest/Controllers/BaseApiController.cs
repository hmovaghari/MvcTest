using Microsoft.AspNetCore.Mvc;
using MyAccounting.Data;
using MyAccounting.Repository;

namespace MvcTest.Controllers
{
    public class BaseApiController
    {
        private SqlDBContext _context;
        private ApiKeyRepository _apiKeyRepository;
        public BaseApiController(SqlDBContext context)
        {
            _context = context;
            _apiKeyRepository = new ApiKeyRepository(context);
        }

        public async Task<bool> IsValidApiKeyAsync(string apiKey)
        {
            return await _apiKeyRepository.IsValidApiKeyAsync(apiKey);
        }

        public async Task<bool> IsValidAdminApiKeyAsync(string apiKey)
        {
            return await _apiKeyRepository.IsValidAdminApiKeyAsync(apiKey);
        }
    }
}
