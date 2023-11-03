using AutoMapper;
using EntityLayer.DTOs;
using EntityLayer.Entities;

namespace Capitol.Utulities
{
    public class DtoMapper : Profile
    {
        public DtoMapper()
        {
            CreateMap<AddVisitorDTO, Visitor>();
            CreateMap<Visitor, AddVisitorDTO>();
            CreateMap<UpdateVisitorDTO, Visitor>();
            CreateMap<Visitor, UpdateVisitorDTO>();
            CreateMap<Visitor, AllVisitorDTO>();
            CreateMap<AllVisitorDTO, Visitor>();
            CreateMap<Payment, PaymentDTO>();
            CreateMap<PaymentDTO, Payment>();
            CreateMap<ReportsDTO, Report>();
            CreateMap<Report, ReportsDTO>();
            CreateMap<VisitorDetailsDTO, Visitor>();
            CreateMap<Visitor, VisitorDetailsDTO>().ForMember(x => x.Payments, opt => opt.Ignore());

        }
    }
}
