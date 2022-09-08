using AuthServer.Core.Configration;
using AuthServer.Core.Dto;
using AuthServer.Core.DTOs;
using AuthServer.Core.Entities;
using AuthServer.Core.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace AuthServer.Services.Services
{
    public class TokenService : ITokenService
    {
        private readonly UserManager<UserApp> _userManager;
        private readonly CustomTokenOption _customTokenOption;

        public TokenService(UserManager<UserApp> userManager, IOptions<CustomTokenOption> options)
        {
            _userManager=userManager;
            _customTokenOption=options.Value;
        }


        //refresh token için bir string değer üreteceğiz
        private string CreateRefreshToken()
        {
            var numberByte = new byte[32];
            using var rnd = RandomNumberGenerator.Create();
            rnd.GetBytes(numberByte);
            return Convert.ToBase64String(numberByte);
        }


        //payload içerisinde kullanıcı hakkında tutulan her bir data claim olarak adlandırılıyor
        //kullancııların hangi apilere istek yapabileceğini işaretliyorurz
        //audineces apileri işaret ediyor

        //user için claim yapısı
        private IEnumerable<Claim> GetClaims(UserApp userApp, List<string> audiences)
        {
            var userList = new List<Claim>
            {

                new Claim(ClaimTypes.NameIdentifier, userApp.Id),
                new Claim(JwtRegisteredClaimNames.Email,userApp.Email),

                //username tc olacağı için test sırsında burayı kontrol et
                new Claim(ClaimTypes.Name,userApp.UserName),

                //plaka koduna göre ekranlar açılacak
                new Claim(ClaimTypes.PostalCode,userApp.CityPlateCode), 

                //her token için token idsi oluşturuyorum
                new Claim(JwtRegisteredClaimNames.Jti,Guid.NewGuid().ToString())
            };
            //tokenın ona uygun mu değil mi burada kontrol ediyor
            userList.AddRange(audiences.Select(x => new Claim(JwtRegisteredClaimNames.Aud, x)));
            return userList;
        }


        //clientlar için claim yapısı
        private IEnumerable<Claim> GetClaimsByClient(Client client)
        {
            var claims = new List<Claim>();
            //clientler hangi apilerde yetkili onları burada inceliyoruz.
            claims.AddRange(client.Audiences.Select(x => new Claim(JwtRegisteredClaimNames.Aud, x)));

            //token için id ürettik
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString());

            //bu token kimin için oluşturuldu onu oluşturuyoruz
            new Claim(JwtRegisteredClaimNames.Sub, client.Id.ToString());

            return claims;
        }


        //tokenı burada oluşturuyoruz
        public TokenDto CreateToken(UserApp userApp)
        {
            //access tokının süresi--şuan ki zamana custom token optinstan gelen dakikayı ekleyecek
            var accessTokenExpiration = DateTime.Now.AddMinutes(_customTokenOption.AccessTokenExpiration);

            //refresh tokının süresi--şuan ki zamana custom token optinstan gelen dakikayı ekleyecek
            var refreshTokenExpiration = DateTime.Now.AddMinutes(_customTokenOption.RefreshTokenExpiration);

            //custom token optins üzerindensign service  göndereceğiz
            var securityKey = SignService.GetSymmetricSecurityKey(_customTokenOption.SecurityKey);

            //imzayı burada oluşturuyoruz. imzayı istediğimiz algoritmayı seçerek oluşturuyoruz
            SigningCredentials signingCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256Signature);

            //tokenın parametrelerini dolduruyoruz
            JwtSecurityToken jwtSecurityToken = new JwtSecurityToken(
                
                //tokenı yayınlayan kim?
                issuer: _customTokenOption.Issuer,

                //tokının süresi
                expires: accessTokenExpiration,

                //benim verdiğim  aralıkta geçerli olsun diye. ezpires ve bu aralıktan
                notBefore: DateTime.Now,

                //bu token hangi apilere ulaşsın ve kimler ulaşsın  
                claims: GetClaims(userApp, _customTokenOption.Audience),

                //tokenın imzası
                signingCredentials: signingCredentials);

            //tokenı burada oluşturuyoruz
            var handler = new JwtSecurityTokenHandler();

            //oluşturan tokıunı buraya veriyoruz yazma işlemi için. tabi dto şeklinde döneceğiz
            var token = handler.WriteToken(jwtSecurityToken);


            //oluşan tokendan gelen parametreleri dto'da karşlıklarına veriyoruz
            var tokenDto = new TokenDto
            {
                AccessToken=token,
                RefreshToken=CreateRefreshToken(),
                AccessTokenExpiration=accessTokenExpiration,
                RefreshTokenExpiration=refreshTokenExpiration,
            };

            //token dtoyu dönüyoruz
            return tokenDto;

        }


        //yukarıda ki token oluşlturma yöntemi ile aynı.
        // tek farkı bu token üyekliksiz apilere istek gönderebilecek ulaşabilecek.
        //refresh token yok burada onu kaldırdık.
        // clientlar için ürettik.
        public ClientTokenDto CreateTokenByClient(Client client)
        {
            var accessTokenExpiration = DateTime.Now.AddMinutes(_customTokenOption.AccessTokenExpiration);
            var securityKey = SignService.GetSymmetricSecurityKey(_customTokenOption.SecurityKey);
            SigningCredentials signingCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256Signature);
            JwtSecurityToken jwtSecurityToken = new JwtSecurityToken(
                issuer: _customTokenOption.Issuer,
                expires: accessTokenExpiration,
                notBefore: DateTime.Now,

                //burada yalnızca client için olan metodu verdik.
                claims: GetClaimsByClient(client),
                signingCredentials: signingCredentials);

            var handler = new JwtSecurityTokenHandler();
            var token = handler.WriteToken(jwtSecurityToken);


            //dönüşü tokendto değil clientTokenDto
            var tokenDto = new ClientTokenDto
            {
                AccessToken=token,
                AccessTokenExpiration=accessTokenExpiration,

            };
            return tokenDto;
        }
    }
}
