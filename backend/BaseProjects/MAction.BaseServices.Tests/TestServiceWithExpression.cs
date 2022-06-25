using System.Linq.Expressions;
using AutoMapper;
using MAction.BaseClasses;
using MAction.BaseServices;

namespace MAction.BaseServices.Tests;

public class TestServiceWithExpression : BaseService<DoctorTest, DoctorTestInputModel, DoctorTestOutputModel>,
    ISelectWithModelExpression<DoctorTest, DoctorTestOutputModel>
{
    private readonly IBaseRepository<DoctorTest> _baseRepository;
    private readonly IMapper _mapper;

    public TestServiceWithExpression(IBaseRepository<DoctorTest> baseRepository, IMapper mapper) : base(baseRepository,
        mapper)
    {
        _baseRepository = baseRepository;
        _mapper = mapper;
    }

    public Expression<Func<DoctorTest, DoctorTestOutputModel>> SelectExpression()
    {
        return x => new DoctorTestOutputModel()
        {
            Language = x.Language,
        };
    }
}

public class TestServiceWithCustomMapping : BaseService<DoctorTest, DoctorTestInputModel, DoctorTestOutputModel>,
    ISelectWithModelMapper<DoctorTest, DoctorTestOutputModel>
{
    public TestServiceWithCustomMapping(IBaseRepository<DoctorTest> repository, IMapper mapper) : base(repository,
        mapper)
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

public class TestServiceWithPureMapper : BaseService<DoctorTest, DoctorTestInputModel, DoctorTestOutputModelPure>
{
    public TestServiceWithPureMapper(IBaseRepository<DoctorTest> repository, IMapper mapper) : base(repository, mapper)
    {
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