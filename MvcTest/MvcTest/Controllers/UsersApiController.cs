using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MyAccounting.Data;
using MyAccounting.Repository;
using MyAccounting.ViewModels;

namespace MvcTest.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersApiController : ControllerBase
    {
        private readonly UserRepository _userRepository;
        private readonly ApiKeyRepository _apiKeyRepository;

        public UsersApiController(SqlDBContext context) //: base(context)
        {
            _userRepository = new UserRepository(context);
            _apiKeyRepository = new ApiKeyRepository(context);
        }

        [HttpPost]
        [Route("Login")]
        public async Task<ActionResult<bool>> Login(string apiKey, LoginViewModel loginViewModel)
        {
            if (!await _apiKeyRepository.IsValidApiKeyAsync(apiKey))
            {
                return BadRequest();
            }

            if (loginViewModel == null)
            {
                return BadRequest();
            }

            var checkPassword = await _userRepository.CheckPassword(loginViewModel);
            if (checkPassword.Item1 != null)
            {
                return checkPassword.Item1 == true;
            }

            return false;
        }

        // GET: api/UsersApi
        [HttpGet]
        [Route("GetUsers")]
        public async Task<ActionResult<IEnumerable<UserDTO>>> GetUsers(string apiKey)
        {
            if (!await _apiKeyRepository.IsValidAdminApiKeyAsync(apiKey))
            {
                return BadRequest();
            }

            return await _userRepository.GetAllUsersAsync(); //_context.Users.ToListAsync();
        }

        // GET: api/UsersApi/5
        [HttpPost]
        [Route("GetUser")]
        public async Task<ActionResult<UserDTO>> GetUser(string apiKey, GUID id)
        {
            if (!await _apiKeyRepository.IsValidAdminApiKeyAsync(apiKey))
            {
                return BadRequest();
            }

            if (id == null)
            {
                return BadRequest();
            }

            var userDto = await _userRepository.FindAsync(id.ID);

            if (userDto == null)
            {
                return NotFound();
            }

            return userDto;
        }

        [HttpPost]
        [Route("BeforChange")]
        public async Task<ActionResult<ChangeUser>> BeforChange(string apiKey, GUID id)
        {
            if (!await _apiKeyRepository.IsValidAdminApiKeyAsync(apiKey))
            {
                return BadRequest();
            }

            if (id == null)
            {
                return BadRequest();
            }

            var userDto = await _userRepository.FindAsync(id.ID);

            if (userDto != null)
            {
                return new ChangeUser()
                {
                    UserID = id.ID,
                    IsActive = userDto.IsActive,
                    IsAdmin = userDto.IsAdmin,
                };
            }

            return null;
        }

        // PUT: api/UsersApi/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut]
        [Route("ChangeUser")]
        public async Task<IActionResult> changeUser(string apiKey, Guid id, ChangeUser changeUser)
        {
            if (!await _apiKeyRepository.IsValidAdminApiKeyAsync(apiKey))
            {
                return BadRequest();
            }

            var userDto = await _userRepository.GetById(id);

            if (changeUser == null || userDto == null || !userDto.IsAdmin)
            {
                return BadRequest();
            }

            if (await _userRepository.ChangeAsync(id, changeUser, apiKey))
            {
                return Ok();
            }
            else
            {
                return Problem();
            }
        }

        [HttpPost]
        [Route("BeforEdit")]
        public async Task<ActionResult<EditUser>> BeforEdit(string apiKey, GUID id)
        {
            if (!await _apiKeyRepository.IsValidApiKeyAsync(apiKey))
            {
                return BadRequest();
            }

            if (id == null)
            {
                return BadRequest();
            }

            var userDto = await _userRepository.FindAsync(id.ID);

            if (userDto != null)
            {
                return new EditUser()
                {
                    UserID = id.ID,
                    Username = userDto.Username,
                    Email = userDto.Email
                };
            }

            return null;
        }

        // PUT: api/UsersApi/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut]
        [Route("EditUser")]
        public async Task<IActionResult> EditUser(string apiKey, Guid id, EditUser editUser)
        {
            if (!await _apiKeyRepository.IsValidApiKeyAsync(apiKey))
            {
                return BadRequest();
            }

            if (editUser == null || id != editUser.UserID)
            {
                return BadRequest();
            }

            if (await _userRepository.FindAsync(id) == null)
            {
                return NotFound();
            }

            if (await _userRepository.EditAsync(id, editUser, apiKey))
            {
                return Ok();
            }
            else
            {
                return Problem();
            }
        }

        // POST: api/UsersApi
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        [Route("CreateUser")]
        public async Task<ActionResult<UserDTO>?> CreateUser(string apiKey, CreateUser createUser)
        {
            if (!await _apiKeyRepository.IsValidApiKeyAsync(apiKey))
            {
                return BadRequest();
            }

            if (await _userRepository.CreateUserAsync(createUser, apiKey))
            {
                return CreatedAtAction("GetUser", new { id = createUser.UserID }, createUser);
            }
            else
            {
                return null;
            }
        }

        // DELETE: api/UsersApi/5
        [HttpDelete]
        [Route("DeleteUser")]
        public async Task<IActionResult> DeleteUser(string apiKey, Guid currentUserID, GUID id)
        {
            if (!await _apiKeyRepository.IsValidAdminApiKeyAsync(apiKey))
            {
                return BadRequest();
            }

            if (id == null)
            {
                return BadRequest();
            }

            var currentUser = await _userRepository.GetById(currentUserID);

            if (currentUser == null || !currentUser.IsAdmin)
            {
                return BadRequest();
            }

            if (await _userRepository.FindAsync(id.ID) == null)
            {
                return NotFound();
            }

            if (await _userRepository.DeleteAsync(id.ID))
            {
                return Ok();
            }
            else
            {
                return Problem();
            }
        }
    }
}
