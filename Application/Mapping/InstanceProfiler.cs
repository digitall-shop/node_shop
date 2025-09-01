using Application.Models.NodeManager;
using AutoMapper;
using Domain.DTOs.Instance;
using Domain.Entities;

namespace Application.Mapping;

public class InstanceProfiler : Profile
{
    public InstanceProfiler()
    {
        CreateMap<InstanceCreateDto, Instance>();

        CreateMap<Instance, InstanceDto>()
            .ForMember(dest => dest.NodeName, opt => opt.MapFrom(src => src.Node.NodeName))
            .ForMember(dest => dest.Username, opt => opt.MapFrom(src => src.User.UserName))
            .ForMember(dest => dest.PanelName, opt => opt.MapFrom(src => src.Panel.Name));

        CreateMap<ProvisionResponseDto, InstanceCreateDto>()
            .ForMember(dest => dest.ContainerDockerId, opt => opt.MapFrom(src => src.ContainerDockerId));

    }
    
}