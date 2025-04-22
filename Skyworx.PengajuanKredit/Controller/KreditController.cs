using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Skyworx.Common.Command;
using Skyworx.Common.Dto;
using Skyworx.Service.Interface;

namespace Skyworx.PengajuanKredit.Controller;

[ApiController]
[Route("[controller]")]
[Authorize] 
public class KreditController(IKreditService kreditService) : ControllerBase
{
    [HttpPost("CreateData")]
    public async Task<ApiResponse> Create([FromBody] CreatePengajuanKreditCommand command)
    {
        return await kreditService.CreateAsync(command);
    }
    
    [HttpGet]
    public async Task<ApiDataResponse<PengajuanKreditDto>> GetAll()
    {
        return await kreditService.GetAllAsync();
    }
    
    [HttpGet("GetDataByid")]
    public async Task<ApiDataResponse<PengajuanKreditDto>> GetById([FromQuery] Guid id)
    {
        return await kreditService.GetByIdAsync(id);
    }

    [HttpPut("UpdateData/{id:guid}")]
    public async Task<ApiResponse> Update(Guid id, [FromBody] CreatePengajuanKreditCommand command)
    {
        return await kreditService.UpdateAsync(id, command);
    }
    
    [HttpDelete("{id:guid}")]
    public async Task<ApiResponse> Delete(Guid id)
    {
        return  await kreditService.DeleteAsync(id);
    }
    
    [HttpPost("hitung-angsuran")]
    public async Task<ActionResult<ApiDataResponse<AngsuranDto>>> HitungAngsuran([FromBody] CalculateAngsuranCommand command)
    {
        return await kreditService.HitungAngsuranAsync(command);
    }
    
    [HttpPost("save-msg")]
    public async Task<ApiResponse> SaveMsg([FromBody] CreatePengajuanKreditCommand command)
    {
        return await kreditService.SaveMsg(command);
    }



}