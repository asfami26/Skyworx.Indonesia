using Asf.Messaging.MessageBroker;
using AutoMapper;
using MassTransit;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Skyworx.Common.Command;
using Skyworx.Common.Constants;
using Skyworx.Common.Dto;
using Skyworx.Common.Exception;
using Skyworx.Repository.DataContext;
using Skyworx.Repository.Entity;
using Skyworx.Service.Interface;

namespace Skyworx.Service.Impl;

public class KreditService(DataContext context, LocalContext local, IMapper mapper,IBus bus) : IKreditService
{
    public async Task<ApiResponse> CreateAsync(CreatePengajuanKreditCommand command)
    {
        var response = new ApiResponse();
        if (command.Plafon <= 0 || command.Bunga <= 0 || command.Bunga > 100 || command.Tenor <= 0)
            throw new BadRequestException(ResponseConstant.InvalidInput);

        var entity = mapper.Map<PengajuanKredit>(command);  
        entity.CreatedAt = DateTime.Now;
        entity.UpdatedAt = DateTime.MaxValue;
        entity.Angsuran = HitungAngsuran(command.Plafon, command.Bunga, command.Tenor);
        
        local.PengajuanKredits.Add(entity);
        await local.SaveChangesAsync();
        
        entity.CreatedAt = DateTime.UtcNow;
        
        await bus.Publish(new KreditPengajuanCreatedEvent
        {
            Id = entity.Id,
            Plafon = entity.Plafon,
            Bunga = entity.Bunga,
            Tenor = entity.Tenor,
            Angsuran = entity.Angsuran,
            CreatedAt = entity.CreatedAt
        });
        response.Message = ResponseConstant.SubmitSuccess;
        return response; 
    }
    
    public async Task<ApiDataResponse<PengajuanKreditDto>> GetAllAsync()
    {
        var response = new ApiDataResponse<PengajuanKreditDto>();
        var result = await context.PengajuanKredits.ToListAsync();
        var data = mapper.Map<List<PengajuanKreditDto>>(result);
        
        response.Message = data.Count > 0 ? ResponseConstant.GetDataSuccess : ResponseConstant.MesNotFound; 
        response.Data = data;
        return response;
    }
    
    public async Task<ApiDataResponse<PengajuanKreditDto>> GetByIdAsync(Guid id)
    {
        var response = new ApiDataResponse<PengajuanKreditDto>();
        var result = await context.PengajuanKredits.FindAsync(id)
                     ?? throw new NotFoundException(ResponseConstant.MesNotFound);

        
        var data = mapper.Map<PengajuanKreditDto>(result);
        response.Message = ResponseConstant.GetDataSuccess;
        response.Data = [data];
        
        return response;
    }
    
    public async Task<ApiResponse> UpdateAsync(Guid id, CreatePengajuanKreditCommand command)
    {
        var response = new ApiResponse();
        var existing = await context.PengajuanKredits.FindAsync(id)
                       ?? throw new NotFoundException(ResponseConstant.MesNotFound);

        mapper.Map(command, existing);
        existing.UpdatedAt = DateTime.UtcNow.Date;
        await context.SaveChangesAsync();
        response.Message = ResponseConstant.UpdateSuccess;
        return response;
    }
    
    public async Task<ApiResponse> DeleteAsync(Guid id)
    {
        var response = new ApiResponse();
        var data = await context.PengajuanKredits.FindAsync(id)
                   ?? throw new NotFoundException(ResponseConstant.MesNotFound);
        
        context.PengajuanKredits.Remove(data);
        await context.SaveChangesAsync();
        
        response.Message = ResponseConstant.DeleteSuccess;
        return response;
    }

    public async Task<ApiResponse> SaveMsg(CreatePengajuanKreditCommand command)
    {
        var entity = mapper.Map<PengajuanKredit>(command);
        entity.Id = Guid.NewGuid();
        entity.CreatedAt = DateTime.UtcNow;
        entity.UpdatedAt = DateTime.MaxValue;
        entity.Angsuran = HitungAngsuran(command.Plafon, command.Bunga, command.Tenor);

        
        
        return new ApiResponse { Message = ResponseConstant.SubmitSuccess };
    }

    public Task<ApiDataResponse<AngsuranDto>> HitungAngsuranAsync(CalculateAngsuranCommand command)
    {
        if (command.Plafon <= 0 || command.Bunga <= 0 || command.Bunga > 100 || command.Tenor <= 0)
            throw new BadRequestException(ResponseConstant.InvalidInput);

        var bungaBulanan = command.Bunga / 12 / 100;

        var angsuran = HitungAngsuran(command.Plafon, command.Bunga, command.Tenor);

        return Task.FromResult(new ApiDataResponse<AngsuranDto>
        {
            Message = "Perhitungan angsuran berhasil",
            Data = [new() { Angsuran = angsuran }]
        });
    }
    
    protected virtual decimal HitungAngsuran(decimal plafon, decimal bunga, int tenor)
    {
        var bungaBulanan = bunga / 12 / 100;
        return Math.Round((plafon * bungaBulanan) / (decimal)(1 - Math.Pow(1 + (double)bungaBulanan, -tenor)), 2);
    }
    
    
}