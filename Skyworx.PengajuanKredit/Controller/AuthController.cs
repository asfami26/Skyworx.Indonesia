using Microsoft.AspNetCore.Mvc;
using Skyworx.Common.Dto;
using Skyworx.Repository.Entity;
using Skyworx.Service.Interface;

namespace Skyworx.PengajuanKredit.Controller;
[ApiController]
[Route("[controller]")]
public class AuthController(IAuthService authService) : ControllerBase
{
    [HttpPost("login")]
    public async Task<ActionResult<ApiDataResponse<LoginDto>>> Login([FromBody] LoginCommand loginDto)
    {
        var response = await authService.AuthenticateAsync(loginDto);
        return new ApiDataResult<LoginDto>(response); 
    }
}