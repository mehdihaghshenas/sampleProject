using System;
using System.Linq.Expressions;
using AutoMapper;
using MAction.BaseClasses;
using MAction.BaseMongoRepository;
using MAction.BaseServices;
using MAction.SampleOnion.Service.Company;
using MAction.SampleOnion.Service.ViewModel.Input;
using MAction.SampleOnion.Service.ViewModel.Output;

namespace MAction.SampleOnion.Service.Category;

public class CategoryService : BaseService<Domain.Entity.SAL.Category, CategoryInputModel, CategoryOutputModel>,
    ICategoryService
{
    public CategoryService(IMongoRepository<Domain.Entity.SAL.Category> repository, IMapper mapper) : base(repository,
        mapper)
    {
    }
}