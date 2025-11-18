using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MvcTest.Repository;
using MyAccounting.Data;
using MyAccounting.ViewModels;

namespace MvcTest.Controllers
{
    public class SettingValuesController : BaseAuthorizeController
    {
        private SettingValueRepository _settingValueRepository;

        public SettingValuesController(SqlDBContext context) : base(context)
        {
            _settingValueRepository = new SettingValueRepository(context);
        }

        // GET: Users
        [Authorize]
        public async Task<IActionResult> Index()
        {
            var user = await GetCurrentUser();
            if (user == null)
            {
                return RedirectToMainPage();
            }
            var settingsValuesDTO = await _settingValueRepository.GetsettingsValuesDTO(user);
            if (settingsValuesDTO == null)
            {
                return NotFound();
            }
            return View(settingsValuesDTO);
        }

        

        [Authorize]
        public async Task<IActionResult> AddOrUpdate()
        {
            var user = await GetCurrentUser();
            if (user == null)
            {
                return RedirectToMainPage();
            }

            var settingValueDTOs = await _settingValueRepository.GetsettingsValuesDTO(user);

            if (settingValueDTOs == null)
            {
                return NotFound();
            }

            return View(settingValueDTOs);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddOrUpdate(List<SettingValueDTO> settingValueDTOs)
        {
            if ((settingValueDTOs?.Count ?? 0) == 0)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                _ = _settingValueRepository.AddOrUpdate(settingValueDTOs);

                return RedirectToAction(nameof(Index));
            }

            return View(settingValueDTOs);
        }
    }
}
