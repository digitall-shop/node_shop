using AutoMapper;
using Domain.DTOs.Transaction;
using Domain.Entities;


namespace Application.Mapping;

public class PaymentProfiler : Profile
{
    public PaymentProfiler()
    {
        CreateMap<PaymentRequest, PaymentRequestDto>().ReverseMap();
        
        CreateMap<PaymentRequestDto, PaymentRequest>()
            .ForMember(dest => dest.User, opt => opt.Ignore())
            .ForMember(dest => dest.BankAccount, opt => opt.Ignore());
    }
}