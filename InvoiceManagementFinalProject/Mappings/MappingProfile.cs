using AutoMapper;
using InvoiceManagementFinalProject.Models;
using InvoiceManagementFinalProject.DTOs.Customer_DTOs;
using InvoiceManagementFinalProject.DTOs.Invoice_DTOs;

namespace InvoiceManagementFinalProject.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<CreateCustomerRequest, Customer>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.DeletedAt, opt => opt.Ignore());

            CreateMap<UpdateCustomerRequest, Customer>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.DeletedAt, opt => opt.Ignore());

            CreateMap<Customer, CustomerResponseDto>();

            CreateMap<CreateInvoiceRequest, Invoice>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.Status, opt => opt.MapFrom(_ => InvoiceStatus.Created))
                .ForMember(dest => dest.TotalSum, opt => opt.Ignore())
                .ForMember(dest => dest.InvoiceRows, opt => opt.MapFrom(src => src.Rows))
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.DeletedAt, opt => opt.Ignore());


            CreateMap<UpdateInvoiceRequest, Invoice>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CustomerId, opt => opt.Ignore())
                .ForMember(dest => dest.Status, opt => opt.Ignore())
                .ForMember(dest => dest.TotalSum, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.DeletedAt, opt => opt.Ignore())
                .ForMember(dest => dest.InvoiceRows, opt => opt.Ignore());

            CreateMap<CreateInvoiceRowDto, InvoiceRow>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.Sum, opt => opt.Ignore())
                .ForMember(dest => dest.InvoiceId, opt => opt.Ignore());

            CreateMap<UpdateInvoiceRowRequest, InvoiceRow>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.InvoiceId, opt => opt.Ignore());

            CreateMap<Invoice, InvoiceResponseDto>()
                .ForMember(dest => dest.CustomerName, opt => opt.MapFrom(src => src.Customer.Name))
                .ForMember(dest => dest.Rows, opt => opt.MapFrom(src => src.InvoiceRows))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()));

            CreateMap<InvoiceRow, InvoiceRowResponseDto>();
        }
    }
}
