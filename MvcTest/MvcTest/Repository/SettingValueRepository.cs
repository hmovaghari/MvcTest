using Microsoft.EntityFrameworkCore;
using MyAccounting.Data;
using MyAccounting.Data.Model;
using MyAccounting.ViewModels;

namespace MyAccounting.Repository
{
    public class SettingValueRepository
    {
        private SqlDBContext _context;

        public SettingValueRepository(SqlDBContext contex)
        {
            _context = contex;
        }

        public async Task<List<SettingValueDTO>?> GetsettingsValuesDTO(UserDTO user)
        {
            try
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
            catch (Exception ex)
            {
                ErrorLogRepository.SaveErrorLog(_context, ex, nameof(SettingValueRepository), nameof(GetsettingsValuesDTO), user);
            }

            return null;
        }

        public async Task<bool?> AddOrUpdate(List<SettingValueDTO> settingValueDTOs)
        {
            try
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
                        var newSettingValue = new SettingValue
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
                return true;
            }
            catch (Exception ex)
            {
                ErrorLogRepository.SaveErrorLog(_context, ex, nameof(SettingValueRepository), nameof(AddOrUpdate), settingValueDTOs);
            }

            return null;
        }
    }
}
