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

        /// <summary>
        /// Is allow to log in
        /// </summary>
        /// <remarks>
        /// sample request body :
        /// 
        ///     {
        ///         "username": "username",
        ///         "password": "password"
        ///     }
        /// 
        /// sample response body :
        /// 
        ///     true
        /// 
        /// </remarks>
        /// <param name="apiKey">API key</param>
        /// <param name="loginViewModel">Login view model</param>
        /// <returns>Is allow to log in</returns>
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
        /// <summary>
        /// Get all users
        /// </summary>
        /// <remarks>
        /// sample request body :
        ///
        ///
        /// 
        /// sample response body :
        ///
        ///
        ///     [
        ///       {
        ///         "userID": "782e5041-ffe3-401e-abed-28183243a72d",
        ///         "username": "user1",
        ///         "email": "user1@weapps.ir",
        ///         "isActive": true,
        ///         "isAdmin": true
        ///       },
        ///       {
        ///         "userID": "fb020ce9-af44-477e-9033-44adf6d48114",
        ///         "username": "user2",
        ///         "email": "user2@gmail.com",
        ///         "isActive": true,
        ///         "isAdmin": false
        ///       }
        ///     ]
        /// 
        ///    
        /// </remarks>
        /// <param name="apiKey">API key</param>
        /// <param name="currentUserID">Current User Id</param>
        /// <returns>users list</returns>
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
        /// <summary>
        /// Get user by id
        /// </summary>
        /// <remarks>
        ///     
        /// 
        /// sample request body :
        /// 
        ///     {
        ///        "id": "304bd50e-83b4-40b4-a8eb-5e62c80cf1b0"
        ///     }
        /// 
        /// sample response body :
        ///
        ///
        ///     {
        ///        "userID": "304bd50e-83b4-40b4-a8eb-5e62c80cf1b0",
        ///        "username": "user3",
        ///        "email": "user3@ymail.com",
        ///        "isActive": true,
        ///        "isAdmin": false
        ///     }
        /// 
        ///    
        /// </remarks>
        /// <param name="apiKey">API key</param>
        /// <param name="currentUserID">Current User Id</param>
        /// <param name="id">User ID</param>
        /// <returns>User info</returns>
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

        /// <summary>
        /// Show user info before change
        /// </summary>
        ///
        /// <remarks>
        /// 
        /// sample request body :
        ///
        ///
        ///     {
        ///       "id": "304bd50e-83b4-40b4-a8eb-5e62c80cf1b0"
        ///     }
        /// 
        /// 
        /// sample response body :
        ///
        ///
        ///     {
        ///        "userID": "304bd50e-83b4-40b4-a8eb-5e62c80cf1b0",
        ///        "isActive": true,
        ///        "isAdmin": false
        ///     }
        /// 
        ///    
        /// </remarks>
        /// <param name="apiKey">API key</param>
        /// <param name="currentUserID">Current User Id</param>
        /// <param name="id">User ID</param>
        /// <returns>info of user</returns>
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
        /// <summary>
        /// Change User
        /// </summary>
        /// <remarks>
        /// 
        /// sample request body :
        ///
        ///
        ///     {
        ///       "userID": "304bd50e-83b4-40b4-a8eb-5e62c80cf1b0",
        ///       "isActive": true,
        ///       "isAdmin": true
        ///     }
        /// 
        /// 
        /// sample response body :
        ///     
        ///     
        ///    
        /// </remarks>
        /// <param name="apiKey">API key</param>
        /// <param name="currentUserID">Current User Id</param>
        /// <param name="changeUser">Change User Info</param>
        /// <returns></returns>
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

        /// <summary>
        /// Show user info before edit
        /// </summary>
        /// <remarks>
        /// 
        /// sample request body :
        /// 
        ///
        ///     {
        ///       "id": "2a4fb175-f3c0-4de8-b589-91d39118dba0"
        ///     }
        /// 
        /// sample response body :
        ///     
        ///
        ///     {
        ///        "userID": "2a4fb175-f3c0-4de8-b589-91d39118dba0",
        ///        "username": "user4",
        ///        "email": "user4@gmail.com",
        ///        "newPassword": null,
        ///        "confirmNewPassword": null
        ///     }
        ///    
        /// </remarks>
        /// <param name="apiKey">API key</param>
        /// <param name="currentUserID">Current User Id</param>
        /// <param name="id">User ID</param>
        /// <returns>info of user</returns>
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
        /// <summary>
        /// Edit user
        /// </summary>
        /// <remarks>
        /// 
        /// sample request body :
        /// 
        ///     {
        ///        "userID": "35fdf87d-239d-4bd5-979f-bf20bbf6d25a",
        ///        "username": "new_user_name",
        ///        "email": "new_email@mail.com",
        ///        "newPassword": null,
        ///        "confirmNewPassword": null
        ///     }
        /// 
        /// sample response body :
        ///     
        ///     
        ///    
        /// </remarks>
        /// <param name="apiKey">API key</param>
        /// <param name="currentUserID">Current User Id</param>
        /// <param name="editUser">Edit User Info</param>
        /// <returns></returns>
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
        /// <summary>
        /// Create a new user
        /// </summary>
        ///
        /// <remarks>
        /// 
        /// sample request body :
        /// 
        ///     {
        ///        "userID": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
        ///        "username": "new_user",
        ///        "email": "new_user@example.com",
        ///        "password": "asdf123456789qwerty",
        ///        "confirmPassword": "asdf123456789qwerty"
        ///     }
        /// 
        /// sample response body :
        ///
        ///     {
        ///       "userID": "00a10035-b6bb-4d79-88b0-202252765f93",
        ///       "username": "new_user",
        ///       "email": "new_user@example.com",
        ///       "password": "asdf123456789qwerty",
        ///       "confirmPassword": "asdf123456789qwerty"
        ///     }
        ///    
        /// </remarks>
        /// <param name="apiKey">API key</param>
        /// <param name="createUser">Create User Info</param>
        /// <returns>User Info</returns>
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
        /// <summary>
        /// Delete User
        /// </summary>
        ///
        /// <remarks>
        ///
        /// sample request body :
        /// 
        ///
        ///     {
        ///        "id": "00a10035-b6bb-4d79-88b0-202252765f93"
        ///     }
        /// 
        /// sample response body :
        ///     
        /// 
        ///    
        /// </remarks>
        /// <param name="apiKey">API key</param>
        /// <param name="currentUserID">Current User Id</param>
        /// <param name="id">User ID</param>
        /// <returns></returns>
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
