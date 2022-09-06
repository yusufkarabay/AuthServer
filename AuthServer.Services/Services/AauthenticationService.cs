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
    public class AauthenticationService : IAauthenticationService
    {
        private readonly List<Client> _clients;
        private readonly ITokenService _tokenService;
        private readonly UserManager<UserApp> _userManager;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IGenericRepository<UserRefreshToken> _userRefreshTokenService;

        public AauthenticationService(IOptions<List<Client>> optionsClient, ITokenService tokenService, UserManager<UserApp> userManager, IUnitOfWork unitOfWork, IGenericRepository<UserRefreshToken> userRefreshTokenService)
        {
            _clients=optionsClient.Value;
            _tokenService=tokenService;
            _userManager=userManager;
            _unitOfWork=unitOfWork;
            _userRefreshTokenService=userRefreshTokenService;
        }

        public async Task<CustomResponseDto<TokenDto>> CreateAccesTokenAsync(LoginDto loginDto)
        {
            if (loginDto==null) throw new ArgumentNullException(nameof(loginDto));
            var user = await _userManager.FindByNameAsync(loginDto.UserName);
            if (user==null) return CustomResponseDto<TokenDto>.Fail("User Name or Password is wrong", 400, true);
            if (!await _userManager.CheckPasswordAsync(user, loginDto.Password))
            {
                if (user==null) return CustomResponseDto<TokenDto>.Fail("User Name or Password is wrong", 400, true);
            }

            var token = _tokenService.CreateToken(user);
            var userRefreshToken = await _userRefreshTokenService.Where(x => x.UserId==user.Id).SingleOrDefaultAsync();
            if (userRefreshToken==null)
            {
                await _userRefreshTokenService.AddAsync(new UserRefreshToken { UserId=user.Id, RefreshTokenCode=token.RefreshToken, ExpirationTime=token.RefreshTokenExpiration });
            }
            else
            {
                userRefreshToken.RefreshTokenCode=token.RefreshToken;
                userRefreshToken.ExpirationTime=token.RefreshTokenExpiration;
            }

            await _unitOfWork.SaveChangesAsync();
            return CustomResponseDto<TokenDto>.Success(token, 200);


        }

        public CustomResponseDto<ClientTokenDto> CreateTokenByClient(ClientLoginDto clientLoginDto)
        {
            var client = _clients.SingleOrDefault(x => x.Id==clientLoginDto.ClientId&&x.Secret==clientLoginDto.ClientSecret);
            if (client==null)
            {
                return CustomResponseDto<ClientTokenDto>.Fail("ClientId or ClientSecret Not Found", 404, true);
            }
            var token = _tokenService.CreateTokenByClient(client);
            return CustomResponseDto<ClientTokenDto>.Success(token, 200);
        }

        public async Task<CustomResponseDto<TokenDto>> CreateTokenByRefreshTokenAsync(string refreshToken)
        {
            var existRefreshToken = await _userRefreshTokenService.Where(x => x.RefreshTokenCode==refreshToken).SingleOrDefaultAsync();
            if (existRefreshToken==null)
            {
                return CustomResponseDto<TokenDto>.Fail("Refresh Token Not Found", 404, true);
            }


            var user = await _userManager.FindByIdAsync(existRefreshToken.UserId);
            if (user==null)
            {
                return CustomResponseDto<TokenDto>.Fail("User Id Not Found", 404, true);
            }

            var tokenDto = _tokenService.CreateToken(user);

            existRefreshToken.RefreshTokenCode=tokenDto.RefreshToken;
            existRefreshToken.ExpirationTime=tokenDto.RefreshTokenExpiration;
            await _unitOfWork.SaveChangesAsync();

            return CustomResponseDto<TokenDto>.Success(tokenDto, 200);

        }

        public async Task<CustomResponseDto<NoDataDto>> RevokeRefreshToken(string refreshToken)
        {
            var existRefreshToken = await _userRefreshTokenService.Where(x => x.RefreshTokenCode==refreshToken).SingleOrDefaultAsync();
            if (existRefreshToken==null)
            {
                return CustomResponseDto<NoDataDto>.Fail("Refresh Token Not Found", 404, true);
            }
            _userRefreshTokenService.DeleteAsync(existRefreshToken);
            await _unitOfWork.SaveChangesAsync();
            return CustomResponseDto<NoDataDto>.Success(200);
        }
    }
}
