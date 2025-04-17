
using Microsoft.AspNetCore.Mvc;

namespace Skyworx.Common.Dto;

public class ApiDataResult<T> : ObjectResult
{
    public ApiDataResult(ApiDataResponse<T> response)
        : base(response)
    {
        StatusCode = 200;
        if (response.Data == null)
        {
            if (!string.IsNullOrEmpty(response.Message))
            {
                if (response.Message.Contains("unauthorized", StringComparison.OrdinalIgnoreCase))
                    StatusCode = 401;
                else if (response.Message.Contains("not found", StringComparison.OrdinalIgnoreCase))
                    StatusCode = 404;
                else if (response.Message.Contains("invalid", StringComparison.OrdinalIgnoreCase))
                    StatusCode = 400;
                else
                    StatusCode = 400; 
            }
            else
            {
                StatusCode = 404; 
            }
        }
    }
}
