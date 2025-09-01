using Application.Models.Marzban;
using Application.Models.NodeManager;
using AutoMapper;
using Domain.DTOs.Node;
using Domain.Entities;

namespace Application.Mapping;

public class NodeProfiler : Profile
{
    public NodeProfiler()
    {
        CreateMap<NodeDto, Node>().ReverseMap();

        CreateMap<NodeDto, MarzbanNodeCreateRequest>()
            .ForMember(dest => dest.AddAsNewHost, opt => opt.MapFrom(src => true))
            .ForMember(dest => dest.Address, opt => opt.MapFrom(src => src.SshHost))
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.NodeName))
            .ForMember(dest => dest.Port, opt => opt.MapFrom(src => src.ServerPort))
            .ForMember(dest => dest.UsageCoefficient, opt => opt.MapFrom(src => 1.0m));

        CreateMap<NodeDto, ProvisionRequestDto>()
            .ForMember(dest => dest.CustomerId, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.XrayContainerImage, opt => opt.MapFrom(src => src.XrayContainerImage))
            .ForMember(dest => dest.NodeId, opt => opt.MapFrom(src => src.Id));

        CreateMap<Node,NodeCreateDto>().ReverseMap();
        
    }
}