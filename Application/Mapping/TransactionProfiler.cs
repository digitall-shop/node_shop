using Application.DomainEvents.Events;
using AutoMapper;
using Domain.DTOs.Transaction;
using Domain.Entities;
using Domain.Enumes.Transaction;
using Domain.Events.DomainEvents.Events;

namespace Application.Mapping;

public class TransactionProfiler : Profile
{
    public TransactionProfiler()
    {
        CreateMap<TransactionCreateDto, Transaction>();
        
        CreateMap<Transaction, TransactionDto>()
            .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src.Type.ToString()))
            .ForMember(dest => dest.Reason, opt => opt.MapFrom(src => src.Reason.ToString()));
        
        CreateMap<TransactionCreateDto, UserBalanceChangedEvent>()
            .ForMember(dest => dest.NewBalance
                , opt
                    => opt.MapFrom(src => src.BalanceAfter));

        CreateMap<AdminManualAdjustDto, TransactionCreateDto>();
        

    }
}