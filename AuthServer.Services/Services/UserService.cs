using AuthServer.Core.Dto;
using AuthServer.Core.DTOs;
using AuthServer.Core.Entities;
using AuthServer.Core.Services;
using AuthServer.Services.Mapping;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace AuthServer.Services.Services
{
    public class UserService : IUserService
    {
        private readonly UserManager<UserApp>_userManager;

        public UserService(UserManager<UserApp> userManager)
        {
            _userManager=userManager;
        }

        public async Task<CustomResponseDto<UserAppDto>> CreateUserAsync(CreateUserDto createUserDto)
        {
         var user=new UserApp { Email=createUserDto.Email,UserName=createUserDto.UserName};
            var result = await _userManager.CreateAsync(user, createUserDto.Password);
            if (!result.Succeeded)
            {
                var errors = result.Errors.Select(x => x.Description).ToList();
                return CustomResponseDto<UserAppDto>.Fail(new ErrorDto(errors, true), 400);
                
            }
            return CustomResponseDto<UserAppDto>.Success(ObjectMapper.Mapper.Map<UserAppDto>(user), 200);
        }

        public async Task<CustomResponseDto<UserAppDto>> GetUserByUserNameAsync(string userName)
        {
           var user = await _userManager.FindByNameAsync(userName);
            if (user==null)
            {
                return CustomResponseDto<UserAppDto>.Fail("User Name Not Found", 404, true);
            }
            return CustomResponseDto<UserAppDto>.Success(ObjectMapper.Mapper.Map<UserAppDto>(user), 200);
        }
    }
}
