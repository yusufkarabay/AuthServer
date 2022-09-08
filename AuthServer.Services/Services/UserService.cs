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
        private readonly UserManager<UserApp> _userManager;

        public UserService(UserManager<UserApp> userManager)
        {
            _userManager=userManager;
        }

        //diğer apiden gelen userların hesap oluşturma servisi


        //public string UserName { get; set; }
        //public string Password { get; set; }
        //public string Email { get; set; }
        //public string CityPlateCode { get; set; }



        //user oluşturma
        public async Task<CustomResponseDto<UserAppDto>> CreateUserAsync(CreateUserDto createUserDto)
        {
            //bu satıda kullancıdan username,maili,plaka kodunu aldık fakat passwor almadık. Bunu veritabanında tutmamak için hashleme işlemi gerçekleşecek
            var user = new UserApp { Email=createUserDto.Email, UserName=createUserDto.UserName, CityPlateCode=createUserDto.CityPlateCode };

            //user oluşturduk . yanına haslanmış  passwordu ekledik veritabanına öyle gönderdik
            var result = await _userManager.CreateAsync(user, createUserDto.Password);

            //başarılı oladıysa
            if (!result.Succeeded)
            {
                //başarılı olmama sebebini oluşturuyoruz
                var errors = result.Errors.Select(x => x.Description).ToList();
                
                //oluşan hataları dönderiyoruz
                return CustomResponseDto<UserAppDto>.Fail(new ErrorDto(errors, true), 400);

            }

            //başarılıysa userappdtoyuı dönüyoruz !!!!! userappdtodan usernamei kaldırdım!!!! bunu daha sonra incele
            ///username tc olacağı için
            return CustomResponseDto<UserAppDto>.Success(ObjectMapper.Mapper.Map<UserAppDto>(user), 200);
        }


        //TC numarasına göre kullancı çekme
        public async Task<CustomResponseDto<UserAppDto>> GetUserByUserNameAsync(string userName)
        {
           //gelen tcye göre userı sorguladık
            var user = await _userManager.FindByNameAsync(userName);

            if (user==null)
            {
                //yoksa hata ver. Kullanıcı bulunamadı 404 
                return CustomResponseDto<UserAppDto>.Fail("User Name Not Found", 404, true);
            }

            //varsa userı döner fakat tcyi dönmüyor. 


            //!!!!!!!!!!!!!!! eğer ihtiyaç varsa yeni bi dtto oluştur içerisinde tc olan.
            return CustomResponseDto<UserAppDto>.Success(ObjectMapper.Mapper.Map<UserAppDto>(user), 200);
        }
    }
}
