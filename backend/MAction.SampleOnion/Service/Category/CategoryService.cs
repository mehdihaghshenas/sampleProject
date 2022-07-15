using System;
using System.Linq.Expressions;
using AutoMapper;
using MAction.BaseClasses;
using MAction.BaseMongoRepository;
using MAction.BaseServices;
using MAction.SampleOnion.Repository;
using MAction.SampleOnion.Service.ViewModel.Input;
using MAction.SampleOnion.Service.ViewModel.Output;
using MAction.SipOnline.Service;

namespace MAction.SampleOnion.Service.Category;

public class CategoryService : BaseServiceInt<Domain.Entity.SAL.Category, CategoryInputModel, CategoryOutputModel>,
    ICategoryService
{
    public CategoryService(IBaseRepositoryInt<Domain.Entity.SAL.Category> repository, IMapper mapper, IServiceDependencyProvider baseServiceDependencyProvider) :
     base(repository, mapper, baseServiceDependencyProvider)
    {
    }
}