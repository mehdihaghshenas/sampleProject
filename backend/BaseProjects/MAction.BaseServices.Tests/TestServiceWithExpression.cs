using System.Linq.Expressions;
using AutoMapper;
using MAction.BaseClasses;
using MAction.BaseClasses.Exceptions;
using MAction.BaseServices;

namespace MAction.BaseProject.Tests;

public class TestServiceWithExpression : BaseServiceWithKey<int, DoctorTest, DoctorTestInputModel, DoctorTestOutputModel>,
    ISelectWithModelExpression<DoctorTest, DoctorTestOutputModel>
{
    private readonly IBaseRepository<DoctorTest, int> _baseRepository;
    private readonly IMapper _mapper;
    private readonly IBaseServiceDependencyProvider _dependencyProvider;

    public TestServiceWithExpression(IBaseRepository<DoctorTest, int> baseRepository, IMapper mapper, IBaseServiceDependencyProvider dependencyProvider) : base(baseRepository,
        mapper, dependencyProvider)
    {
        _baseRepository = baseRepository;
        _mapper = mapper;
        _dependencyProvider = dependencyProvider;
    }

    public Expression<Func<DoctorTest, DoctorTestOutputModel>> SelectExpression()
    {
        return x => new DoctorTestOutputModel()
        {
            Language = x.Language,
        };
    }
}

public class TestServiceWithCustomMapping : BaseServiceWithKey<int, DoctorTest, DoctorTestInputModel, DoctorTestOutputModel>,
    ISelectWithModelMapper<DoctorTest, DoctorTestOutputModel>
{
    public TestServiceWithCustomMapping(IBaseRepository<DoctorTest, int> repository, IMapper mapper, IBaseServiceDependencyProvider dependencyProvider) : base(repository,
        mapper, dependencyProvider)
    {
    }

    public DoctorTestOutputModel MapEntityToOutput(DoctorTest entitiy)
    {
        return new DoctorTestOutputModel()
        {
            Language = entitiy.Language,
        };
    }
}

public class TestServiceWithPureMapper : BaseServiceWithKey<int, DoctorTest, DoctorTestInputModel, DoctorTestOutputModelPure>
{
    public TestServiceWithPureMapper(IBaseRepository<DoctorTest, int> repository, IMapper mapper,
        IBaseServiceDependencyProvider dependencyProvider) : base(repository, mapper, dependencyProvider)
    {
    }
}

public class TestServiceWithPolicy : BaseServiceWithKey<int, DoctorTest, DoctorTestInputModel, DoctorTestOutputModelPure>
{
    private readonly IBaseServiceDependencyProvider _dependencyProvider;
    public TestServiceWithPolicy(IBaseRepository<DoctorTest, int> repository, IMapper mapper,
        IBaseServiceDependencyProvider baseServiceDependencyProvider) : base(repository, mapper,
        baseServiceDependencyProvider)
    {
        _dependencyProvider = baseServiceDependencyProvider;
    }

    public override Expression<Func<DoctorTest, bool>> GetSelectPermissionExpression()
    {
        if (_dependencyProvider.IsCurrentUserAuthorize("TestGetPolicy"))
            return x => true;
        return x => false;
    }

    public override void CheckInsertPermission(DoctorTest entity)
    {
        if (!_dependencyProvider.IsCurrentUserAuthorize("TestInsertPolicy"))
            throw new ForbiddenExpection($"You need TestInsertPolicy");
    }

    public override void CheckUpdatePermission(DoctorTest entity)
    {
        if (!_dependencyProvider.IsCurrentUserAuthorize("TestUpdatePolicy"))
            throw new ForbiddenExpection($"You need TestUpdatePolicy");
    }

    public override void CheckDeletePermission(DoctorTest entity)
    {
        if (!_dependencyProvider.IsCurrentUserAuthorize("TestDeletePolicy"))
            throw new ForbiddenExpection($"You need TestDeletePolicy");
    }
}

public class DoctorTestInputModel : BaseDTO<DoctorTest, DoctorTestInputModel>
{
    public int DoctorId { get; set; }
    public string LastName { get; set; } = "";
    public string Speciality { get; set; } = "";
    public DateTime BirthDate { get; set; }
    public string Language { get; set; } = "EN";
    public int YearStartPractice { get; set; }
    public int Rate { get; set; }
}

public class DoctorTestOutputModel
{
    public string LastName { get; set; } = "";
    public string Speciality { get; set; } = "";
    public DateTime BirthDate { get; set; }
    public string Language { get; set; } = "EN";
    public int YearStartPractice { get; set; }
    public int Rate { get; set; }
}

public class DoctorTestOutputModelPure : BaseDTO<DoctorTest, DoctorTestOutputModelPure>
{
    public string MyLastName { get; set; } = "";
    public string Speciality { get; set; } = "";
    public DateTime BirthDate { get; set; }
    public string Language { get; set; } = "EN";
    public int YearStartPractice { get; set; }
    public int Rate { get; set; }

    public override void CustomReverseMapping(IMappingExpression<DoctorTest, DoctorTestOutputModelPure> mapping)
    {
        mapping.ForMember(x => x.MyLastName, m => m.MapFrom(d => d.LastName));
    }
}