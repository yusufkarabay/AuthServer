using AuthServer.Core.DTOs;
using AuthServer.Core.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AuthServer.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : CustomBaseController
    {

        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService=userService;
        }



        /// <summary>
        /// Create User
        /// </summary>
        /// <param name="createUserDto"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> CreateUser([FromBody] CreateUserDto createUserDto)
        {
            return ActionResultInstance(await _userService.CreateUserAsync(createUserDto));
        }

        /// <summary>
        /// Get User By TC
        /// </summary>
        /// <returns></returns>

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetUserByUserNameAsync()
        {
            return ActionResultInstance(await _userService.GetUserByUserNameAsync(HttpContext.User.Identity.Name));
        }


    }
}
