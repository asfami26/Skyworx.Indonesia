using Skyworx.Common.Dto;
using Skyworx.Repository.Entity;

namespace Skyworx.Service.Interface;

public interface IAuthService
{
    Task<ApiDataResponse<LoginDto>> AuthenticateAsync(LoginCommand command);
}