using AutoMapper;
using Skyworx.Common.Command;
using Skyworx.Common.Dto;
using Skyworx.Repository.Entity;

namespace Skyworx.Service.Mapper;

public class PengajuanKreditProfile : Profile
{
    public PengajuanKreditProfile()
    {
        CreateMap<PengajuanKredit, PengajuanKreditDto>().ReverseMap();
        
        CreateMap<CreatePengajuanKreditCommand, PengajuanKredit>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(_ => Guid.NewGuid()))
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.Angsuran, opt => opt.Ignore()); 
        
        CreateMap<CreatePengajuanKreditCommand, PengajuanKredit>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.Angsuran, opt => opt.Ignore());

    }
}