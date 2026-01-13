using AutoMapper;
using UptimeChangeMonitor.Application.DTOs;
using MonitorEntity = UptimeChangeMonitor.Domain.Entities.Monitor;

namespace UptimeChangeMonitor.Application.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // Monitor mappings
        CreateMap<MonitorEntity, MonitorDto>().ReverseMap();
        CreateMap<CreateMonitorDto, MonitorEntity>();
        CreateMap<UpdateMonitorDto, MonitorEntity>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => DateTime.UtcNow));

        // UptimeCheck mappings
        CreateMap<UptimeChangeMonitor.Domain.Entities.UptimeCheck, UptimeCheckDto>().ReverseMap();

        // ChangeDetection mappings
        CreateMap<UptimeChangeMonitor.Domain.Entities.ChangeDetection, ChangeDetectionDto>().ReverseMap();
    }
}
