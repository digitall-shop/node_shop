using Application.Models.NodeManager;
using AutoMapper;
using Domain.DTOs.Instance;
using Domain.DTOs.Node;
using Domain.DTOs.Panel;
using Domain.Entities;

namespace Application.Mapping;

public class ProvisionProfiler : Profile
{
    public ProvisionProfiler()
    {
        CreateMap<NodeDto, ProvisionRequestDto>()
            .ForMember(dest => dest.NodeId, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.CertificateKey,
                opt => opt.MapFrom((src, dest, destMember, context) => ((PanelDto)context.Items["Panel"]).CertificateKey))
            .ForMember(dest => dest.XrayPort,
                opt => opt.MapFrom((src, dest, destMember, context) => ((PanelDto)context.Items["Panel"]).XrayPort))
            .ForMember(dest => dest.ApiPort,
                opt => opt.MapFrom((src, dest, destMember, context) => ((PanelDto)context.Items["Panel"]).ApiPort))
            .ForMember(dest => dest.InboundPort,
                opt => opt.MapFrom((src, dest, destMember, context) => ((PanelDto)context.Items["Panel"]).InboundPort));

        CreateMap<ProvisionResponseDto, InstanceCreateDto>()
            .ForMember(dest => dest.ContainerDockerId, opt => opt.MapFrom(src => src.ContainerDockerId))
            .ForMember(dest => dest.InstanceId, opt => opt.Ignore())
            .ForMember(dest => dest.UserId, opt => opt.Ignore())
            .ForMember(dest => dest.NodeId, opt => opt.Ignore()) 
            .ForMember(dest => dest.PanelId, opt => opt.Ignore())
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
        
        CreateMap<InstanceCreateDto,Instance>()
            .ForMember(dest => dest.ContainerDockerId, opt => opt.MapFrom(src => src.ContainerDockerId))
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
    }
}