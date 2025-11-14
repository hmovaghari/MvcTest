using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyAccounting.Data;
using MyAccounting.ViewModels;

namespace MvcTest.Controllers
{
    public class SettingValuesController : BaseController
    {
        public SettingValuesController(SqlDBContext context) : base(context)
        {
        }

        // GET: Users
        [Authorize]
        public async Task<IActionResult> Index()
        {
            var user = await GetCurrentUser();
            if (user == null)
            {
                return RedirectToAction("Index", "Home");
            }
            var settingsValuesDTO = await GetsettingsValuesDTO(user);
            if (settingsValuesDTO == null)
            {
                return NotFound();
            }
            return View(settingsValuesDTO);
        }

        private async Task<List<SettingValueDTO>> GetsettingsValuesDTO(MyAccounting.Data.Model.User user)
        {
            var settingsKeys = await _context.SettingKeys.ToListAsync();
            var settingsValues = await _context.SettingValues.Where(sv => sv.UserID == user.UserID).ToListAsync();
            var settingsValuesDTO = new List<SettingValueDTO>();
            settingsKeys.ForEach(key =>
            {
                var existingValue = settingsValues.FirstOrDefault(sv => sv.SettingKeyID == key.SettingKeyID);
                settingsValuesDTO.Add(new SettingValueDTO
                {
                    SettingValueID = existingValue != null ? existingValue.SettingValueID : Guid.NewGuid(),
                    SettingKeyID = key.SettingKeyID,
                    SettingKeyTitle = key.Key,
                    SettingValueDescription = key.Description,
                    UserID = user.UserID,
                    SettingValue = existingValue != null ? existingValue.Value : string.Empty
                });
            });
            return settingsValuesDTO;
        }

        [Authorize]
        public async Task<IActionResult> AddOrUpdate()
        {
            var user = await GetCurrentUser();
            if (user == null)
            {
                return RedirectToAction("Index", "Home");
            }

            var settingValueDTOs = await GetsettingsValuesDTO(user);

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
                var userID = settingValueDTOs?.First().UserID;

                // استفاده از Task.WhenAll به جای ForEach با async
                // settingValueDTOs?.ForEach(async dto =>
                var tasks = settingValueDTOs?.Select(async dto =>
                {
                    var existingSettingValue = await _context.SettingValues
                        .FirstOrDefaultAsync(sv => sv.UserID == userID && sv.SettingKeyID == dto.SettingKeyID);
                    
                    if (existingSettingValue != null)
                    {
                        existingSettingValue.Value = dto.SettingValue;
                        _context.Update(existingSettingValue);
                    }
                    else
                    {
                        var newSettingValue = new MyAccounting.Data.Model.SettingValue
                        {
                            SettingValueID = dto.SettingValueID,
                            SettingKeyID = dto.SettingKeyID,
                            UserID = dto.UserID,
                            Value = dto.SettingValue
                        };
                        _context.Add(newSettingValue);
                    }
                }) ?? new List<Task>();

                await Task.WhenAll(tasks);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            return View(settingValueDTOs);
        }
    }
}
