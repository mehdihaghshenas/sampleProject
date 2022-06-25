using AutoMapper;

namespace MAction.BaseServices.Tests;

public class CustomMappingRegister : Profile
{
    public CustomMappingRegister()
    {

        CreateMap<DoctorTest, DoctorTest>().ReverseMap();

    }

}
