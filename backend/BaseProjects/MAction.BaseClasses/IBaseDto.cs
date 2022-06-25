using AutoMapper;

namespace MAction.BaseClasses;

public interface IBaseDto
{
    void ConfigureMapping(Profile profile );
}