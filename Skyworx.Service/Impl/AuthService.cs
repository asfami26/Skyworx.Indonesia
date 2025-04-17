using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Skyworx.Common.Config;
using Skyworx.Common.Dto;
using Skyworx.Repository.DataContext;
using Skyworx.Repository.Entity;
using Skyworx.Service.Interface;

namespace Skyworx.Service.Impl;

public class AuthService(ConfigExtension config, DataContext context) : IAuthService
{
    public async Task<ApiDataResponse<LoginDto>> AuthenticateAsync(LoginCommand command)
    {
        var user = await context.UserAccounts
            .FirstOrDefaultAsync(u => u.Username == command.Username && u.Password == command.Password);

        if (user == null)
        {
            return new ApiDataResponse<LoginDto>
            {
                Message = "Unauthorized: username or password incorrect",
                Data = null
            };
        }

        var key = Encoding.ASCII.GetBytes(config.Jwt.SecretKey);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Role, user.Role)
            }),
            Expires = DateTime.Now.AddHours(2),
            Issuer = config.Jwt.Issuer,
            Audience = config.Jwt.Audience,
            SigningCredentials =
                new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };

        var token = new JwtSecurityTokenHandler().CreateToken(tokenDescriptor);
        var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

        return new ApiDataResponse<LoginDto>
        {
            Message = "Login successful",
            Data = [new LoginDto { Token = tokenString }]
        };
    }
}
