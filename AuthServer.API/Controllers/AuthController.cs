using AuthServer.Core.DTOs;
using AuthServer.Core.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace AuthServer.API.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class AuthController : CustomBaseController
    {
        private readonly IAuthentService _authService;

        public AuthController(IAuthentService authService)
        {
            _authService = authService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateToken(LoginDto loginDto)
        {
            var result = await _authService.CreateTokenAsync(loginDto);

            return ActionResultInstance(result);

        }

        [HttpPost]
        public  IActionResult CreateTokenByClient(ClientLoginDto clientLoginDto )
        {
            var result =  _authService.CreateTokenByClient(clientLoginDto);

            return ActionResultInstance(result);

        }

        [HttpPost]
        public async Task<IActionResult> RevokeRefrehToken(RefreshTokenDto refreshTokenDto)
        {
            var result = await _authService.RevokeRefreshToken(refreshTokenDto.Token);
            return ActionResultInstance(result);
        }

        [HttpPost]
        public async Task<IActionResult> CreateTokenByRefrehToken(RefreshTokenDto refreshTokenDto)
        {
            var result = await _authService.CreateTokenByRefreshToken(refreshTokenDto.Token);
            return ActionResultInstance(result);
        }


    }
}
