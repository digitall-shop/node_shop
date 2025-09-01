using AutoMapper;
using Domain.DTOs.Support;
using Domain.Entities;

namespace Application.Mapping;

public class SupportProfile : Profile
{
    public SupportProfile()
    {
        CreateMap<SupportTicket, SupportTicketDto>();
        CreateMap<SupportMessage, SupportMessageDto>()
            .ForMember(d=>d.AttachmentUrl, opt => opt.MapFrom(src => src.AttachmentPath));
    }
}