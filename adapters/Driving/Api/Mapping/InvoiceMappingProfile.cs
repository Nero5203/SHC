using AutoMapper;
using application.Dto.Purchases;
using Domain.Entities.Purchases;

namespace api.Mapping
{
    public class InvoiceMappingProfile : Profile
    {
        public InvoiceMappingProfile()
        {
            CreateMap<Invoice, InvoiceResponseDto>();
        }
    }
}
