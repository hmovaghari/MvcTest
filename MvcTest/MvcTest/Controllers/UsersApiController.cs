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
        public async Task<ActionResult<IEnumerable<UserDTO>>> GetUsers(string apiKey, Guid currentUserID)
        {
            if (!await _apiKeyRepository.IsValidAdminApiKeyAsync(apiKey))
            {
                return Forbid();
            }

            var currentUser = await _userRepository.GetById(currentUserID);
            if (currentUser == null)
            {
                return BadRequest();
            }
            if (!currentUser.IsAdmin)
            {
                return Forbid();
            }

            return await _userRepository.GetAllUsersAsync(); //_context.Users.ToListAsync();
        }

        // GET: api/UsersApi/5
        [HttpPost]
        [Route("GetUser")]
        public async Task<ActionResult<UserDTO>> GetUser(string apiKey, Guid currentUserID, GUID id)
        {
            if (!await _apiKeyRepository.IsValidAdminApiKeyAsync(apiKey))
            {
                return Forbid();
            }

            var currentUser = await _userRepository.GetById(currentUserID);
            if (currentUser == null)
            {
                return BadRequest();
            }
            if (!currentUser.IsAdmin)
            {
                return Forbid();
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
        public async Task<ActionResult<ChangeUser>> BeforChange(string apiKey, Guid currentUserID, GUID id)
        {
            if (!await _apiKeyRepository.IsValidAdminApiKeyAsync(apiKey))
            {
                return Forbid();
            }

            var currentUser = await _userRepository.GetById(currentUserID);
            if (currentUser == null)
            {
                return BadRequest();
            }
            if (!currentUser.IsAdmin)
            {
                return Forbid();
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
        public async Task<IActionResult> changeUser(string apiKey, Guid currentUserID, ChangeUser changeUser)
        {
            if (!await _apiKeyRepository.IsValidAdminApiKeyAsync(apiKey))
            {
                return Forbid();
            }

            var currentUser = await _userRepository.GetById(currentUserID);
            if (currentUser == null)
            {
                return BadRequest();
            }
            if (!currentUser.IsAdmin)
            {
                return Forbid();
            }

            if (changeUser == null)
            {
                return BadRequest();
            }

            if (await _userRepository.ChangeAsync(changeUser, apiKey))
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
        public async Task<ActionResult<EditUser>> BeforEdit(string apiKey, Guid currentUserID, GUID id)
        {
            if (id == null)
            {
                return BadRequest();
            }

            if(currentUserID != id.ID)
            {
                return Forbid();
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
        public async Task<IActionResult> EditUser(string apiKey, Guid currentUserID, EditUser editUser)
        {
            if (editUser == null)
            {
                return BadRequest();
            }

            if (currentUserID != editUser.UserID)
            {
                return Forbid();
            }

            if (await _userRepository.FindAsync(currentUserID) == null)
            {
                return NotFound();
            }

            if (await _userRepository.EditAsync(editUser, apiKey))
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
                return Forbid();
            }

            var currentUser = await _userRepository.GetById(currentUserID);
            if (currentUser == null)
            {
                return BadRequest();
            }
            if (!currentUser.IsAdmin)
            {
                return Forbid();
            }

            if (id == null)
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
