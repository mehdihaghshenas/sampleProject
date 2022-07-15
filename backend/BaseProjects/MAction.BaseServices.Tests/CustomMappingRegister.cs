using AutoMapper;

namespace MAction.BaseProject.Tests;

public class CustomMappingRegister : Profile
{
    public CustomMappingRegister()
    {

        CreateMap<DoctorTest, DoctorTest>().ReverseMap();

    }

}
