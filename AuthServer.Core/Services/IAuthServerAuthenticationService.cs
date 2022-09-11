using AuthServer.Core.Dto;
using AuthServer.Core.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthServer.Core.Services
{
    public interface IAuthServerAuthenticationService
    {

        // token oluşturur
        Task<CustomResponseDto<TokenDto>> CreateTokenAsync(LoginDto loginDto);

        //refresh token oluşturur
        Task<CustomResponseDto<TokenDto>> CreateTokenByRefreshTokenAsync(string refreshToken);

        //bu kullanıcı için refresh token varsa null yap şeklinde kodlayacağız
        Task<CustomResponseDto<NoDataDto>> RevokeRefreshTokenAsync(string refreshToken); 

        //client tipi az olduğu için appjsonda tutacağız. ordan gelen id ve secret kontrolünü burada işleyeceğiz
        CustomResponseDto<ClientTokenDto> CreateTokenByClient(ClientLoginDto clientLoginDto);
    }
}
