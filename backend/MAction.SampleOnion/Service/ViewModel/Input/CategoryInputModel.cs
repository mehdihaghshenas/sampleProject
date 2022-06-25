using AutoMapper;
using MAction.BaseClasses;
using MAction.BaseClasses.Helpers;

namespace MAction.SampleOnion.Service.ViewModel.Input;

public class CategoryInputModel: BaseDTO<Domain.Entity.SAL.Category, CategoryInputModel>
{
    public string Id { get; set; }

    public string Key { get; set; }
    public Translation Translation { get; set; }
}