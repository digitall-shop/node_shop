using AutoMapper;
using Domain.DTOs.BankAccount;
using Domain.Entities;

namespace Application.Mapping;

public class BankAccountProfiler : Profile
{
    public BankAccountProfiler()
    {
        CreateMap<BankAccount,BankAccountDto>().ReverseMap();
        CreateMap<BankAccountCreateDto, BankAccount>();
        CreateMap<BankAccountUpdateDto, BankAccount>();
    }
}