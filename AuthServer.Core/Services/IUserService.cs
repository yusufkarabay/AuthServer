using AuthServer.Core.Dto;
using AuthServer.Core.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthServer.Core.Services
{


    //identty kütüphanesini kullandığımız için ıuser repository yapmadık her metot zaten orda var
    //gerekli işlemleri çağırıp service tarafında işleyeceğiz.
    public interface IUserService
    {

        Task<CustomResponseDto<UserAppDto>> CreateUserAsync(CreateUserDto createUserDto);

        Task<CustomResponseDto<UserAppDto>> GetUserByUserNameAsync(string userName);

    }
}
