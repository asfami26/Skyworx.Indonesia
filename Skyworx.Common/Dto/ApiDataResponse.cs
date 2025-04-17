namespace Skyworx.Common.Dto;

public class ApiDataResponse<T> : ApiResponse
{
    public List<T>? Data { get; set; }
}