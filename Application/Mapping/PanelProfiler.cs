using AutoMapper;
using Domain.DTOs.Panel;
using Domain.Entities;

namespace Application.Mapping;

public class PanelProfiler : Profile
{
    public PanelProfiler()
    {
        CreateMap<PanelDto, Panel>().ReverseMap();
        
        CreateMap<PanelCreateDto, Panel>();
        
        CreateMap<PanelOverviewDto, Panel>().ReverseMap();

        CreateMap<PanelUpdateDto, Panel>().ReverseMap();
    }
}