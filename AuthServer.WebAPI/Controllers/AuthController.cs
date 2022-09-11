using AuthServer.Core.DTOs;
using AuthServer.Core.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AuthServer.WebAPI.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class AuthController : CustomBaseController
    {
        private readonly IAuthServerAuthenticationService _authServerAuthenticationService;

        public AuthController(IAuthServerAuthenticationService authServerAuthenticationService)
        {
            _authServerAuthenticationService=authServerAuthenticationService;
        }


        /// <summary>
        /// Create Token
        /// </summary>
        /// <param name="loginDto"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> CreateToken([FromBody] LoginDto loginDto)
        {
            var result = await _authServerAuthenticationService.CreateTokenAsync(loginDto);

            return ActionResultInstance(result);

        }



        [HttpPost]
        public async Task<IActionResult> CreateTokenByClient([FromBody] ClientLoginDto clientLoginDto)
        {
            var result = _authServerAuthenticationService.CreateTokenByClient(clientLoginDto);

            return ActionResultInstance(result);
        }


        [HttpPost]
        public async Task<IActionResult> RevokeRefreshToken([FromBody] RefreshTokenDto refreshTokenDto)
        {
            var result = await _authServerAuthenticationService.RevokeRefreshTokenAsync(refreshTokenDto.RefreshTokenCode);

            return ActionResultInstance(result);

        }

        [HttpPost]
        public async Task<IActionResult> CreateTokenByRefreshToken([FromBody] RefreshTokenDto refreshTokenDto)
        {
            var result = await _authServerAuthenticationService.CreateTokenByRefreshTokenAsync(refreshTokenDto.RefreshTokenCode);

            return ActionResultInstance(result);

        }


    }
}

