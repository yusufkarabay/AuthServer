using AuthServer.Core;
using AuthServer.Core.Configration;
using AuthServer.Core.Dto;
using AuthServer.Core.DTOs;
using AuthServer.Core.Entities;
using AuthServer.Core.Repositories;
using AuthServer.Core.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthServer.Services.Services
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly List<Client> _clients;
        private readonly ITokenService _tokenService;
        private readonly UserManager<UserApp> _userManager;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IGenericRepository<UserRefreshToken> _userRefreshTokenService;

        public AuthenticationService(IOptions<List<Client>> optionsClient, ITokenService tokenService, UserManager<UserApp> userManager, IUnitOfWork unitOfWork, IGenericRepository<UserRefreshToken> userRefreshTokenService)
        {
            _clients=optionsClient.Value;
            _tokenService=tokenService;
            _userManager=userManager;
            _unitOfWork=unitOfWork;
            _userRefreshTokenService=userRefreshTokenService;
        }
       
        
        //token service katmanınında token oluşturma metotlarını bu katmanda çağırıp kullanacağız



        //accces token oluşturma bu metod içerisinde yapılacak
        public async Task<CustomResponseDto<TokenDto>> CreateAccesTokenAsync(LoginDto loginDto)
        {
            //giriş işlemi başarılı değilse null dğer dönüyor.
            if (loginDto==null) throw new ArgumentNullException(nameof(loginDto));

            //gelen usernamden bu kullancııyı sorguluyoruz
            var user = await _userManager.FindByNameAsync(loginDto.UserName);
            
            //eğer bir kullanıcı bulamadıysak(login olmaya çalışan kullancı yanlış username girmiş)
            //bu işlem bizden token isteme işlemi olduğu için.Tokendto dönecek.
            //Fakat fail dönecek.
            //kullanıcı adı veya şifre hatalı dememizin sebebi. yalnzıca username deseydik. şifrenin doğru olduğu bilgisi gider kötü niyetli kullanıcılar için böyle yazdık
            //400 cönme sebibimiz client hatası olduğu için. Bu durum gösterilsin diye ishow için ture gönderdik
            if (user==null) return CustomResponseDto<TokenDto>.Fail("User Name or Password is wrong", 400, true);

            //username doğru ise bu aşamaya geldik. aynı şekilde bu sefer passwordu kontrol ediyoruz.
            if (!await _userManager.CheckPasswordAsync(user, loginDto.Password))
            {
                if (user==null) return CustomResponseDto<TokenDto>.Fail("User Name or Password is wrong", 400, true);
            }

            //kullanıcı oluştuğu için. token oluşturma metoduna user'ı gönderiyoruz
            var token = _tokenService.CreateToken(user);

            //refresh tokenı veritabanına kaydedeceğim. fakat daha önceden var mı onu kontrol ediyorum.
            var userRefreshToken = await _userRefreshTokenService.Where(x => x.UserId==user.Id).SingleOrDefaultAsync();

            //refreshtoken yoksa 
            if (userRefreshToken==null)
            {
                //refreshtoken oluşturacağız
                //user elimizde var idsini kullanıyoruz.
                //5 satır yukarıda token da oluşturmuştuk. Bu tokenden gelen refreshtokenve süresini de kullanıyoruz
                await _userRefreshTokenService.AddAsync(new UserRefreshToken { UserId=user.Id, RefreshTokenCode=token.RefreshToken, ExpirationTime=token.RefreshTokenExpiration });
            }
            else
            {
                //userrefresh token varsa güncelleme işlemi yapıyoruz
                //yeni kodu ve yeni süreyi göncelliyoruz
                userRefreshToken.RefreshTokenCode=token.RefreshToken;
                userRefreshToken.ExpirationTime=token.RefreshTokenExpiration;
            }

            //tokenı veritabanını yansıttık
            await _unitOfWork.SaveChangesAsync();
            //tokenı clienta döndük
            return CustomResponseDto<TokenDto>.Success(token, 200);


        }


        //yalnızca clientlar için token oluşturuyoruz
        public CustomResponseDto<ClientTokenDto> CreateTokenByClient(ClientLoginDto clientLoginDto)
        {
            //gelen clientı sorguluyoruz
            var client = _clients.SingleOrDefault(x => x.Id==clientLoginDto.ClientId&&x.Secret==clientLoginDto.ClientSecret);

            //böyle bir client yoksa
            if (client==null)
            {
                //client id yok veya şifresi doğru değil. Is Show'a true gönderiyoruz
                return CustomResponseDto<ClientTokenDto>.Fail("ClientId or ClientSecret Not Found", 404, true);
            }

            //client , şifre uyumlu ve varsa token oluştur.
            var token = _tokenService.CreateTokenByClient(client);

            //başarılı ve oluşan token'ı dön
            return CustomResponseDto<ClientTokenDto>.Success(token, 200);
        }


        //refresh token ile token  oluşturma
        public async Task<CustomResponseDto<TokenDto>> CreateTokenByRefreshTokenAsync(string refreshToken)
        {
            //geln refresh tokenı veritabandından kontrol ediyoruz
            var existRefreshToken = await _userRefreshTokenService.Where(x => x.RefreshTokenCode==refreshToken).SingleOrDefaultAsync();
            //yoksa
            if (existRefreshToken==null)
            {
                //refresh token yok . 404 döndük. is show'a true gönderdik
                return CustomResponseDto<TokenDto>.Fail("Refresh Token Not Found", 404, true);
            }

            //eğer refresh token varsa user'da vardır. burda var olan refreshtoken'a ait userid ile. o user'ı buluyoruz
            var user = await _userManager.FindByIdAsync(existRefreshToken.UserId);
            if (user==null)
            {
                //user yoksa  bu durumu dönüyoruz
                return CustomResponseDto<TokenDto>.Fail("User Id Not Found", 404, true);
            }

            //user var ise bu user için refresh token oluşturuyoruz
            var tokenDto = _tokenService.CreateToken(user);


            //yeni token oluşunca refreshtoken da yenileniyor
            //bu satırda refresh tokenıı güncelliyoruz
            existRefreshToken.RefreshTokenCode=tokenDto.RefreshToken;

            //bu satırda refreshtokenın süresini güncelliyoruz
            existRefreshToken.ExpirationTime=tokenDto.RefreshTokenExpiration;

            //veritabanına yansıtıyoruz
            await _unitOfWork.SaveChangesAsync();


            //!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!    Veritabanına refresh token kaydedilir. Accesstoken kaydedilmez
            //!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!! Accestoken 1 kez dönülür.

            //tüm token'ı dönüyoruz
            return CustomResponseDto<TokenDto>.Success(tokenDto, 200);

        }


        //refreshtokenı null yapma metodu
        public async Task<CustomResponseDto<NoDataDto>> RevokeRefreshToken(string refreshToken)
        {
            //refreshtokenın var olup olmadığını sorguluyoruz
            var existRefreshToken = await _userRefreshTokenService.Where(x => x.RefreshTokenCode==refreshToken).SingleOrDefaultAsync();
            if (existRefreshToken==null)
            {
                //yoksa 404  dönüyoruz
                return CustomResponseDto<NoDataDto>.Fail("Refresh Token Not Found", 404, true);
            }
            //refresh token varsa null yapıyoruz
            _userRefreshTokenService.Delete(existRefreshToken);

            //veritabanına yansıtıyoruz
            await _unitOfWork.SaveChangesAsync();

            //kullanıcya bişey dönmüyoruz.
            return CustomResponseDto<NoDataDto>.Success(200);
        }
    }
}
