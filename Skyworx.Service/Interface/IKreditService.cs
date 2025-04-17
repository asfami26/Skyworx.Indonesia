using Skyworx.Common.Command;
using Skyworx.Common.Dto;
using Skyworx.Repository.Entity;

namespace Skyworx.Service.Interface;

public interface IKreditService
{
    Task<ApiResponse> CreateAsync(CreatePengajuanKreditCommand command);
    Task<ApiDataResponse<PengajuanKreditDto>> GetAllAsync();
    Task<ApiDataResponse<PengajuanKreditDto>> GetByIdAsync(Guid id);
    Task<ApiResponse> UpdateAsync(Guid id, CreatePengajuanKreditCommand command);
    Task<ApiResponse> DeleteAsync(Guid id);
    Task<ApiDataResponse<AngsuranDto>> HitungAngsuranAsync(CalculateAngsuranCommand command);

}