using AutoMapper;
using MAction.BaseClasses;
using MAction.BaseClasses.Language;

namespace MAction.SampleOnion.Service.ViewModel.Output;

public class CategoryOutputModel : BaseDTO<Domain.Entity.SAL.Category, CategoryOutputModel>
{
    public string Key { get; set; }
    public Translation Translation { get; set; }
}