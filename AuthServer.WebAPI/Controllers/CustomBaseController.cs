using AuthServer.Core.Dto;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AuthServer.WebAPI.Controllers
{
   
    public class CustomBaseController : ControllerBase
    {
        public IActionResult ActionResultInstance<T>(CustomResponseDto<T> customResponseDto)where T : class
        {
            return new ObjectResult(customResponseDto)
            {
                StatusCode=customResponseDto.StatusCode
            };
        }
    }
}
