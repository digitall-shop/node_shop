using AutoMapper;
using Domain.DTOs.CurrencyReta;
using Domain.Entities;

namespace Application.Mapping;

public class CurrencyRateProfiler : Profile
{
    public CurrencyRateProfiler()
    {
        CreateMap<CurrencyRate, CurrencyRateDto>().ReverseMap();
        CreateMap<CurrencyRateCreateDto, CurrencyRate>();
        CreateMap<CurrencyRateUpdateDto, CurrencyRate>(); 
    }
}