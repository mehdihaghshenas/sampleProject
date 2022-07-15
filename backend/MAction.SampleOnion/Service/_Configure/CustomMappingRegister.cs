using AutoMapper;
using MAction.SampleOnion.Domain.Entity.SAL;
using MAction.SampleOnion.Service.ViewModel.Input;
using MAction.SampleOnion.Service.ViewModel.Output;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MAction.SampleOnion.Service.Configure
{
    public class CustomMappingRegister : Profile
    {
        public CustomMappingRegister()
        {
            //We use this only if Model not Inherit From BaseDTO

            //CreateMap<SaleCompanyInputModel, SaleCompany>().ReverseMap().ForMember(x => x.Name, m => m.MapFrom(x => x.CompanyName));
            //CreateMap<SaleCompanyOutputModel, SaleCompany>().ReverseMap().ForMember(x => x.Name, m => m.MapFrom(x => x.CompanyName));

        }

    }
}
