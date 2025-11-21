using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyAccounting.Data;
using MyAccounting.Repository;
using MyAccounting.ViewModels;

namespace MyAccounting.Controllers
{
    public class ApiKeysController : BaseAuthorizeController
    {
        private ApiKeyRepository _apiKeyRepository;
        private UserRepository _userRepository;

        public ApiKeysController(SqlDBContext context) : base(context)
        {
            _apiKeyRepository = new ApiKeyRepository(context);
            _userRepository = new UserRepository(context);
        }

        // GET: ApiKeys
        [Authorize]
        public async Task<IActionResult> Index()
        {
            var currentUser = await GetCurrentUser();
            if (currentUser?.IsAdmin == false)
            {
                return RedirectToMainPage();
            }

            var apiKeys = await _apiKeyRepository.GetAllApiKeysAsync();
            return View(apiKeys);
        }

        // GET: ApiKeys/Create
        [Authorize]
        public async Task<IActionResult> Create()
        {
            var currentUser = await GetCurrentUser();
            if (currentUser?.IsAdmin == false)
            {
                return RedirectToMainPage();
            }

            ViewData["UserID"] = _userRepository.GetSelectList();
            return View();
        }

        // POST: ApiKeys/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ApiKeyID,UserID,IsActive,IsAdmin")] ApiKeyDTO apiKeyDto)
        {
            //if (ModelState.IsValid)
            //{
            if (await _apiKeyRepository.CreateApiKeyAsync(apiKeyDto))
            {
                return RedirectToAction(nameof(Index));
            }
            //}

            ViewData["UserID"] = _userRepository.GetSelectList(apiKeyDto.UserID);
            return View(apiKeyDto);
        }

        // GET: ApiKeys/Edit/5
        [Authorize]
        public async Task<IActionResult> Edit(Guid? id)
        {
            var currentUser = await GetCurrentUser();
            if (currentUser?.IsAdmin == false)
            {
                return RedirectToMainPage();
            }

            if (id == null)
            {
                return NotFound();
            }

            var apiKeyDto = await _apiKeyRepository.FindAsync(id);
            if (apiKeyDto == null)
            {
                return NotFound();
            }
            ViewData["UserID"] = _userRepository.GetSelectList(apiKeyDto.UserID);
            return View(apiKeyDto);
        }

        // POST: ApiKeys/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("ApiKeyID,UserID,IsActive,IsAdmin")] ApiKeyDTO apiKeyDto)
        {
            if (id != apiKeyDto.ApiKeyID)
            {
                return NotFound();
            }

            //if (ModelState.IsValid)
            //{
            if (await _apiKeyRepository.EditApiKeyAsync(apiKeyDto))
            {
                return RedirectToAction(nameof(Index));
            }
            //}
            ViewData["UserID"] = _userRepository.GetSelectList(apiKeyDto.UserID);
            return View(apiKeyDto);
        }

        // GET: ApiKeys/Delete/5
        [Authorize]
        public async Task<IActionResult> Delete(Guid? id)
        {
            var currentUser = await GetCurrentUser();
            if (currentUser?.IsAdmin == false)
            {
                return RedirectToMainPage();
            }

            if (id == null)
            {
                return NotFound();
            }

            var apiKeyDto = await _apiKeyRepository.GetById(id.Value);
            if (apiKeyDto == null)
            {
                return NotFound();
            }

            return View(apiKeyDto);
        }

        // POST: ApiKeys/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            _ = await _apiKeyRepository.DeleteApiKeyAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
