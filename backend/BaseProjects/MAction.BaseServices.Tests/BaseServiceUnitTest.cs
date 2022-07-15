using AutoMapper;
using MAction.BaseClasses;
using MAction.BaseClasses.Exceptions;
using MAction.BaseClasses.InputModels;
using MAction.BaseClasses.OutpuModels;
using MAction.BaseServices;
using MAction.BaseServices.Configure;
using MAction.TestHelpers;
using FluentAssertions;
using Moq;

namespace MAction.BaseProject.Tests;

public partial class BaseServiceUnitTest : IDisposable
{
    private readonly BaseServiceWithKey<int, DoctorTest, DoctorTest, DoctorTest> _baseService;
    private readonly TestServiceWithExpression _baseServiceWithExpression;
    private readonly TestServiceWithCustomMapping _baseServiceWithCustomMapping;
    private readonly TestServiceWithPureMapper _baseServiceWithPureMapper;

    public BaseServiceUnitTest()
    {
        var repository = new Mock<IBaseRepository<DoctorTest, int>>();
        var dependencyProvider = new FakeBaseServiceDependencyProvider();
        dependencyProvider.SetInternalMode(true);
#pragma warning disable CS8603 // Possible null reference return.
        repository.Setup(x => x.GetAll()).Returns(() => Doctors.AsQueryable());
        repository.Setup(x => x.Get(It.IsAny<int>()))
            .Returns<int>(x => Doctors.FirstOrDefault(d => d.DoctorId == (int)x));

        repository.Setup(x => x.InsertWithSaveChange(It.IsAny<DoctorTest>()))
            .Returns<DoctorTest>(x =>
            {
                Doctors.Add(x);
                x.SetPrimaryKeyValue(x.DoctorId == 0 ? Doctors.Max(doctorTest => doctorTest.DoctorId) + 1 : x.DoctorId);
                return x;
            });

        repository.Setup(x => x.InsertWithSaveChangeAsync(It.IsAny<DoctorTest>(), CancellationToken.None))
            .Returns<DoctorTest, CancellationToken>((x, _) =>
            {
                Doctors.Add(x);
                x.SetPrimaryKeyValue(x.DoctorId == 0 ? Doctors.Max(doctorTest => doctorTest.DoctorId) + 1 : x.DoctorId);
                return Task.FromResult(x);
            });

        repository.Setup(x => x.RemoveWithSaveChangeAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .Returns<int, CancellationToken>((x, _) =>
            {
                var entity = Doctors.First(y => y.DoctorId == (int)x);
                Doctors.Remove(entity);
                return Task.FromResult(entity.DoctorId);
            });

        repository.Setup(x => x.UpdateWithSaveChange(It.IsAny<DoctorTest>())).Callback<DoctorTest>(x =>
        {
            Doctors.Remove(Doctors.First(y => y.DoctorId == x.DoctorId));
            Doctors.Add(x);
        });

        repository.Setup(x => x.UpdateWithSaveChangeAsync(It.IsAny<DoctorTest>(), CancellationToken.None))
            .Returns<DoctorTest, CancellationToken>((x, _) =>
            {
                Doctors.Remove(Doctors.First(y => y.DoctorId == x.DoctorId));
                Doctors.Add(x);
                return Task.FromResult(x.DoctorId);
            });

#pragma warning restore CS8603 // Possible null reference return.
        //should configure automapper future for Automapper Tests
        //IMapper mapper = Mock.Of<IMapper>();
        var mockMapper = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile(new CustomMappingRegister());
            cfg.AddProfile(new CustomMappingProfile(this.GetType().Assembly));
        });
        var mapper = mockMapper.CreateMapper();
        _baseService =
            new BaseServiceWithKey<int, DoctorTest, DoctorTest, DoctorTest>(repository.Object, mapper, dependencyProvider);
        _baseServiceWithExpression = new TestServiceWithExpression(repository.Object, Mock.Of<IMapper>(), dependencyProvider);
        _baseServiceWithCustomMapping = new TestServiceWithCustomMapping(repository.Object, Mock.Of<IMapper>(), dependencyProvider);
        _baseServiceWithPureMapper = new TestServiceWithPureMapper(repository.Object, mapper, dependencyProvider);
    }

    public void Dispose()
    {
    }

    [Fact]
    public void TestGet_WithAutoMapping_WhenEntityDoesNotFound_MustThrowException()
    {
        var action = new Action(() => _baseService.Get(int.MaxValue));

        action.Should().ThrowExactly<InvalidEntityException>();
    }

    [Fact]
    public void TestGet_WithAutoMapping_WhenEntityExists_MustReturnResult()
    {
        var doctor = _baseService.Get(Doctors.First().DoctorId);

        doctor.Should().NotBeNull();
    }

    [Fact]
    public void TestGet_WithUseExpressionMapping_MustThrowNotImplementException()
    {
        var action = new Action(() =>
            _baseService.Get(Doctors.First().DoctorId, OutputModelMappingTypeEnum.UseExpression));

        action.Should().ThrowExactly<NotImplementedException>();
    }

    [Fact]
    public void TestGet_WithUseExpressionMapping_WhenEntityExists_MustReturnResult()
    {
        var doctor = _baseServiceWithExpression.Get(Doctors.First().DoctorId, OutputModelMappingTypeEnum.UseExpression);

        doctor.Should().NotBeNull();
    }

    [Fact]
    public void TestGet_WithUseMappingModelMapping_MustThrowNotImplementException()
    {
        var action = new Action(() =>
            _baseService.Get(Doctors.First().DoctorId, OutputModelMappingTypeEnum.UseMappingModel));

        action.Should().ThrowExactly<NotImplementedException>();
    }


    [Fact]
    public void TestGet_WithUseMappingModelMapping_WhenEntityExists_MustReturnResult()
    {
        var doctor =
            _baseServiceWithCustomMapping.Get(Doctors.First().DoctorId, OutputModelMappingTypeEnum.UseMappingModel);

        doctor.Should().NotBeNull();
    }


    [Fact]
    public void TestGet_WithPureAutoMapperMapping_WhenEntityExists_MustReturnResult()
    {
        var doctor =
            _baseServiceWithPureMapper.Get(Doctors.First().DoctorId, OutputModelMappingTypeEnum.PureAutoMapper);

        doctor.Should().NotBeNull();
    }

    [Fact]
    public void TestGetAsync_WithAutoMapping_WhenEntityDoesNotFound_MustThrowException()
    {
        var action = new Func<Task<DoctorTest>>(async () => await _baseService.GetAsync(int.MaxValue));

        action.Should().ThrowExactlyAsync<InvalidEntityException>();
    }

    [Fact]
    public void TestGetAsync_WithAutoMapping_WhenEntityExists_MustReturnResult()
    {
        var doctor = _baseService.GetAsync(Doctors.First().DoctorId);

        doctor.Should().NotBeNull();
    }

    [Fact]
    public void TestGetAsync_WithUseExpressionMapping_MustThrowNotImplementException()
    {
        var action = new Func<Task<DoctorTest>>(async () =>
            await _baseService.GetAsync(Doctors.First().DoctorId, OutputModelMappingTypeEnum.UseExpression));

        action.Should().ThrowExactlyAsync<NotImplementedException>();
    }

    [Fact]
    public void TestGetAsync_WithUseExpressionMapping_WhenEntityExists_MustReturnResult()
    {
        var doctor =
            _baseServiceWithExpression.GetAsync(Doctors.First().DoctorId, OutputModelMappingTypeEnum.UseExpression);

        doctor.Should().NotBeNull();
    }

    [Fact]
    public void TestGetAsync_WithUseMappingModelMapping_MustThrowNotImplementException()
    {
        var action = new Func<Task<DoctorTest>>(async () =>
            await _baseService.GetAsync(Doctors.First().DoctorId, OutputModelMappingTypeEnum.UseMappingModel));

        action.Should().ThrowExactlyAsync<NotImplementedException>();
    }


    [Fact]
    public void TestGetAsync_WithUseMappingModelMapping_WhenEntityExists_MustReturnResult()
    {
        var doctor =
            _baseServiceWithCustomMapping.GetAsync(Doctors.First().DoctorId,
                OutputModelMappingTypeEnum.UseMappingModel);

        doctor.Should().NotBeNull();
    }


    [Fact]
    public void TestGetAsync_WithPureAutoMapperMapping_WhenEntityExists_MustReturnResult()
    {
        var doctor =
            _baseServiceWithPureMapper.GetAsync(Doctors.First().DoctorId, OutputModelMappingTypeEnum.PureAutoMapper);

        doctor.Should().NotBeNull();
    }

    [Fact]
    public void TestGetItemByFilter_WithoutPaging_WhenEntityExists_MustReturnResult()
    {
        var filterAndSortCondition = new FilterAndSortConditions
        {
            DisablePaging = true
        };

        var doctors = _baseService.GetItemByFilter(filterAndSortCondition);

        doctors.Data.Count.Should().Be(4);
    }

    [Fact]
    public void TestGetItemByFilter_WithPaging_WhenEntityExists_MustReturnResult()
    {
        ResetList();
        var filterAndSortCondition = new FilterAndSortConditions
        {
            DisablePaging = false,
            PageSize = 3,
            PageNumber = 2
        };

        var doctors = _baseService.GetItemByFilter(filterAndSortCondition);

        doctors.Data.Count.Should().Be(1);
    }

    [Fact]
    public void TestGetItemByFilter_WithTwoPaging_WhenEntityExists_MustReturnResult()
    {
        var filterAndSortCondition = new FilterAndSortConditions
        {
            DisablePaging = false,
            PageSize = 2,
            PageNumber = 2
        };

        var doctors = _baseService.GetItemByFilter(filterAndSortCondition);

        doctors.Data.Count.Should().Be(2);
    }

    [Fact]
    public void TestGetItemByFilter_WithCondition_WithAutoMapping_WhenEntityExists_MustReturnResult()
    {
        ResetList();
        var filterAndSortCondition = new FilterAndSortConditions
        {
            DisablePaging = true
        };

        var doctors = _baseService.GetItemByFilter(filterAndSortCondition, x => x.Language == "English");

        doctors.Data.Count.Should().Be(2);
    }

    [Fact]
    public void
        TestGetItemByFilter_WithCondition_WithUseExpressionMapping_WhenEntityExists_MustThrowNotImplementException()
    {
        var filterAndSortCondition = new FilterAndSortConditions
        {
            DisablePaging = true
        };

        var action = new Action(() =>
            _baseService.GetItemByFilter(filterAndSortCondition, x => x.Language == "English",
                OutputModelMappingTypeEnum.UseExpression));

        action.Should().ThrowExactly<NotImplementedException>();
    }

    [Fact]
    public void TestGetItemByFilter_WithCondition_WithUseExpressionMapping_WhenEntityExists_MustReturnResult()
    {
        ResetList();
        var filterAndSortCondition = new FilterAndSortConditions
        {
            DisablePaging = true
        };

        var doctors = _baseServiceWithExpression.GetItemByFilter(filterAndSortCondition, x => x.Language == "English",
            OutputModelMappingTypeEnum.UseExpression);

        doctors.Data.Count.Should().Be(2);
    }

    [Fact]
    public void
        TestGetItemByFilter_WithCondition_WithUseMappingModelMapping_WhenEntityExists_MustThrowNotImplementException()
    {
        var filterAndSortCondition = new FilterAndSortConditions
        {
            DisablePaging = true
        };

        var action = new Action(() =>
            _baseService.GetItemByFilter(filterAndSortCondition, x => x.Language == "English",
                OutputModelMappingTypeEnum.UseMappingModel));

        action.Should().ThrowExactly<NotImplementedException>();
    }

    [Fact]
    public void TestGetItemByFilter_WithCondition_WithUseMappingModelMapping_WhenEntityExists_MustReturnResult()
    {
        ResetList();
        var filterAndSortCondition = new FilterAndSortConditions
        {
            DisablePaging = true
        };

        var doctors = _baseServiceWithCustomMapping.GetItemByFilter(filterAndSortCondition,
            x => x.Language == "English",
            OutputModelMappingTypeEnum.UseMappingModel);

        doctors.Data.Count.Should().Be(2);
    }


    [Fact]
    public void TestGetItemByFilter_WithCondition_WithPureAutoMapperMapping_WhenEntityExists_MustReturnResult()
    {
        ResetList();
        var filterAndSortCondition = new FilterAndSortConditions
        {
            DisablePaging = true
        };

        var doctors = _baseServiceWithPureMapper.GetItemByFilter(filterAndSortCondition, x => x.Language == "English",
            OutputModelMappingTypeEnum.PureAutoMapper);

        doctors.Data.Count.Should().Be(2);
    }

    [Fact]
    public async Task TestGetItemByFilterAsync_WithoutPaging_WhenEntityExists_MustReturnResult()
    {
        ResetList();
        var filterAndSortCondition = new FilterAndSortConditions
        {
            DisablePaging = true
        };

        var doctors = await _baseService.GetItemByFilterAsync(filterAndSortCondition);

        doctors.Data.Count.Should().Be(4);
    }

    [Fact]
    public async Task TestGetItemByFilterAsync_WithPaging_WhenEntityExists_MustReturnResult()
    {
        ResetList();
        var filterAndSortCondition = new FilterAndSortConditions
        {
            DisablePaging = false,
            PageSize = 3,
            PageNumber = 2
        };

        var doctors = await _baseService.GetItemByFilterAsync(filterAndSortCondition);

        doctors.Data.Count.Should().Be(1);
    }

    [Fact]
    public async Task TestGetItemByFilterAsync_WithTwoPaging_WhenEntityExists_MustReturnResult()
    {
        var filterAndSortCondition = new FilterAndSortConditions
        {
            DisablePaging = false,
            PageSize = 2,
            PageNumber = 2
        };

        var doctors = await _baseService.GetItemByFilterAsync(filterAndSortCondition);

        doctors.Data.Count.Should().Be(2);
    }

    [Fact]
    public async Task TestGetItemByFilterAsync_WithCondition_WithAutoMapping_WhenEntityExists_MustReturnResult()
    {
        ResetList();
        var filterAndSortCondition = new FilterAndSortConditions
        {
            DisablePaging = true
        };

        var doctors = await _baseService.GetItemByFilterAsync(filterAndSortCondition, x => x.Language == "English");

        doctors.Data.Count.Should().Be(2);
    }

    [Fact]
    public void
        TestGetItemByFilterAsync_WithCondition_WithUseExpressionMapping_WhenEntityExists_MustThrowNotImplementException()
    {
        var filterAndSortCondition = new FilterAndSortConditions
        {
            DisablePaging = true
        };

        var action = new Func<Task<DynamicQueryFilterResult<DoctorTest>>>(async () =>
            await _baseService.GetItemByFilterAsync(filterAndSortCondition, x => x.Language == "English",
                OutputModelMappingTypeEnum.UseExpression));

        action.Should().ThrowExactlyAsync<NotImplementedException>();
    }

    [Fact]
    public async Task TestGetItemByFilterAsync_WithCondition_WithUseExpressionMapping_WhenEntityExists_MustReturnResult()
    {
        ResetList();
        var filterAndSortCondition = new FilterAndSortConditions
        {
            DisablePaging = true
        };

        var doctors = await _baseServiceWithExpression.GetItemByFilterAsync(filterAndSortCondition,
            x => x.Language == "English",
            OutputModelMappingTypeEnum.UseExpression);

        doctors.Data.Count.Should().Be(2);
    }

    [Fact]
    public void
        TestGetItemByFilterAsync_WithCondition_WithUseMappingModelMapping_WhenEntityExists_MustThrowNotImplementException()
    {
        var filterAndSortCondition = new FilterAndSortConditions
        {
            DisablePaging = true
        };

        var action = new Func<Task<DynamicQueryFilterResult<DoctorTest>>>(async () =>
            await _baseService.GetItemByFilterAsync(filterAndSortCondition, x => x.Language == "English",
                OutputModelMappingTypeEnum.UseMappingModel));

        action.Should().ThrowExactlyAsync<NotImplementedException>();
    }

    [Fact]
    public async Task TestGetItemByFilterAsync_WithCondition_WithUseMappingModelMapping_WhenEntityExists_MustReturnResult()
    {
        ResetList();
        var filterAndSortCondition = new FilterAndSortConditions
        {
            DisablePaging = true
        };

        var doctors = await _baseServiceWithCustomMapping.GetItemByFilterAsync(filterAndSortCondition,
            x => x.Language == "English",
            OutputModelMappingTypeEnum.UseMappingModel);

        doctors.Data.Count.Should().Be(2);
    }


    [Fact]
    public async Task TestGetItemByFilterAsync_WithCondition_WithPureAutoMapperMapping_WhenEntityExists_MustReturnResult()
    {
        ResetList();
        var filterAndSortCondition = new FilterAndSortConditions
        {
            DisablePaging = true
        };

        var doctors = await _baseServiceWithPureMapper.GetItemByFilterAsync(filterAndSortCondition,
            x => x.Language == "English",
            OutputModelMappingTypeEnum.PureAutoMapper);

        doctors.Data.Count.Should().Be(2);
    }

    [Fact]
    public void TestGetItemsByFilter_WithoutPaging_WhenEntityExists_MustReturnResult()
    {
        ResetList();
        var filterAndSortCondition = new FilterAndSortConditions
        {
            DisablePaging = true
        };
        var doctors = _baseService.GetItemsByFilter(filterAndSortCondition);

        doctors.Data.Count.Should().Be(Doctors.Count);
    }

    [Fact]
    public void TestGetItemsByFilter_WithPaging_WhenEntityExists_MustReturnResult()
    {
        ResetList();
        var filterAndSortCondition = new FilterAndSortConditions
        {
            DisablePaging = false,
            PageSize = 3,
            PageNumber = 2
        };

        var doctors = _baseService.GetItemsByFilter(filterAndSortCondition);

        doctors.Data.Count.Should().Be(1);
    }

    [Fact]
    public void TestGetItemsByFilter_WithTwoPaging_WhenEntityExists_MustReturnResult()
    {
        var filterAndSortCondition = new FilterAndSortConditions
        {
            DisablePaging = false,
            PageSize = 2,
            PageNumber = 2
        };

        var doctors = _baseService.GetItemsByFilter(filterAndSortCondition);

        doctors.Data.Count.Should().Be(2);
    }

    [Fact]
    public void TestGetItemsByFilter_WithCondition_WithAutoMapping_WhenEntityExists_MustReturnResult()
    {
        ResetList();
        var filterAndSortCondition = new FilterAndSortConditions
        {
            DisablePaging = true
        };

        var doctors = _baseService.GetItemsByFilter(filterAndSortCondition, x => x.Language == "English");

        doctors.Data.Count.Should().Be(2);
    }

    [Fact]
    public void
        TestGetItemsByFilter_WithCondition_WithUseExpressionMapping_WhenEntityExists_MustThrowNotImplementException()
    {
        var filterAndSortCondition = new FilterAndSortConditions
        {
            DisablePaging = true
        };

        var action = new Action(() =>
            _baseService.GetItemsByFilter<DoctorTestOutputModel>(filterAndSortCondition, x => x.Language == "English",
                OutputModelMappingTypeEnum.UseExpression));

        action.Should().ThrowExactly<NotImplementedException>();
    }

    [Fact]
    public void TestGetItemsByFilter_WithCondition_WithUseExpressionMapping_WhenEntityExists_MustReturnResult()
    {
        ResetList();
        var filterAndSortCondition = new FilterAndSortConditions
        {
            DisablePaging = true
        };

        var doctors = _baseServiceWithExpression.GetItemsByFilter<DoctorTestOutputModel>(filterAndSortCondition,
            x => x.Language == "English",
            OutputModelMappingTypeEnum.UseExpression);

        doctors.Data.Count.Should().Be(2);
    }

    [Fact]
    public void
        TestGetItemsByFilter_WithCondition_WithUseMappingModelMapping_WhenEntityExists_MustThrowNotImplementException()
    {
        var filterAndSortCondition = new FilterAndSortConditions
        {
            DisablePaging = true
        };

        var action = new Action(() =>
            _baseService.GetItemsByFilter<DoctorTestOutputModel>(filterAndSortCondition, x => x.Language == "English",
                OutputModelMappingTypeEnum.UseMappingModel));

        action.Should().ThrowExactly<NotImplementedException>();
    }

    [Fact]
    public void TestGetItemsByFilter_WithCondition_WithUseMappingModelMapping_WhenEntityExists_MustReturnResult()
    {
        var filterAndSortCondition = new FilterAndSortConditions
        {
            DisablePaging = true
        };

        var doctors = _baseServiceWithCustomMapping.GetItemsByFilter<DoctorTestOutputModel>(filterAndSortCondition,
            x => x.Language == "English",
            OutputModelMappingTypeEnum.UseMappingModel);

        doctors.Data.Count.Should().Be(2);
    }


    [Fact]
    public void TestGetItemsByFilter_WithCondition_WithPureAutoMapperMapping_WhenEntityExists_MustReturnResult()
    {
        ResetList();
        var filterAndSortCondition = new FilterAndSortConditions
        {
            DisablePaging = true
        };

        var doctors = _baseServiceWithPureMapper.GetItemsByFilter<DoctorTest>(filterAndSortCondition,
            x => x.Language == "English",
            OutputModelMappingTypeEnum.PureAutoMapper);

        doctors.Data.Count.Should().Be(2);
    }

    [Fact]
    public void TestGetItemsByFilter_WithoutPaging_WithExtraCondition_WhenEntityExists_MustReturnResult()
    {
        ResetList();
        var filterAndSortCondition = new FilterAndSortConditions
        {
            DisablePaging = true
        };
        var doctors = _baseService.GetItemsByFilter(filterAndSortCondition, x => x.Language == "Spanish");

        doctors.Data.Count.Should().Be(1);
        doctors.Data.All(x => x.Language == "Spanish").Should().BeTrue();
    }

    [Fact]
    public void TestGetItemsByFilter_WithoutPaging_WithFilter_WhenEntityExists_MustReturnResult()
    {
        ResetList();
        var filterAndSortCondition = new FilterAndSortConditions
        {
            DisablePaging = true,
            WhereConditionText = "Language = \"Spanish\""
        };
        var doctors = _baseService.GetItemsByFilter(filterAndSortCondition);

        doctors.Data.Count.Should().Be(1);
        doctors.Data.All(x => x.Language == "Spanish").Should().BeTrue();
    }

    [Fact]
    public void TestGetItemsByFilter_WithoutPaging_WithSort_WhenEntityExists_MustReturnResult()
    {
        ResetList();
        var filterAndSortCondition = new FilterAndSortConditions
        {
            DisablePaging = true,
            SortText = "Rate"
        };
        var doctors = _baseService.GetItemsByFilter(filterAndSortCondition);

        doctors.Data.First().Rate.Should().Be(2);
        doctors.Data.Last().Rate.Should().Be(4);
    }

    [Fact]
    public void TestGetItemsByFilter_WithoutPaging_WithFilterAndSort_WhenEntityExists_MustReturnResult()
    {
        ResetList();
        var filterAndSortCondition = new FilterAndSortConditions
        {
            DisablePaging = true,
            SortText = "Rate",
            WhereConditionText = "Language = \"English\""
        };
        var doctors = _baseService.GetItemsByFilter(filterAndSortCondition);

        doctors.Data.Count.Should().Be(2);
        doctors.Data.All(x => x.Language == "English").Should().BeTrue();
        doctors.Data.First().Rate.Should().Be(2);
        doctors.Data.Last().Rate.Should().Be(3);
    }

    [Fact]
    public async Task TestGetItemsByFilterAsync_WithoutPaging_WhenEntityExists_MustReturnResult()
    {
        ResetList();
        var filterAndSortCondition = new FilterAndSortConditions
        {
            DisablePaging = true
        };

        var doctors = await _baseService.GetItemsByFilterAsync(filterAndSortCondition);

        doctors.Data.Count.Should().Be(4);
    }

    [Fact]
    public async Task TestGetItemsByFilterAsync_WithPaging_WhenEntityExists_MustReturnResult()
    {
        ResetList();
        var filterAndSortCondition = new FilterAndSortConditions
        {
            DisablePaging = false,
            PageSize = 3,
            PageNumber = 2
        };

        var doctors = await _baseService.GetItemsByFilterAsync(filterAndSortCondition);

        doctors.Data.Count.Should().Be(1);
    }

    [Fact]
    public async Task TestGetItemsByFilterAsync_WithTwoPaging_WhenEntityExists_MustReturnResult()
    {
        var filterAndSortCondition = new FilterAndSortConditions
        {
            DisablePaging = false,
            PageSize = 2,
            PageNumber = 2
        };

        var doctors = await _baseService.GetItemsByFilterAsync(filterAndSortCondition);

        doctors.Data.Count.Should().Be(2);
    }

    [Fact]
    public async Task TestGetItemsByFilterAsync_WithCondition_WithAutoMapping_WhenEntityExists_MustReturnResult()
    {
        ResetList();
        var filterAndSortCondition = new FilterAndSortConditions
        {
            DisablePaging = true
        };

        var doctors = await _baseService.GetItemsByFilterAsync(filterAndSortCondition, x => x.Language == "English");

        doctors.Data.Count.Should().Be(2);
    }

    [Fact]
    public void
        TestGetItemsByFilterAsync_WithCondition_WithUseExpressionMapping_WhenEntityExists_MustThrowNotImplementException()
    {
        var filterAndSortCondition = new FilterAndSortConditions
        {
            DisablePaging = true
        };

        var action = new Func<Task<DynamicQueryFilterResult<DoctorTest>>>(async () =>
            await _baseService.GetItemsByFilterAsync<DoctorTest>(filterAndSortCondition, x => x.Language == "English",
                OutputModelMappingTypeEnum.UseExpression, cancellationToken: CancellationToken.None));

        action.Should().ThrowExactlyAsync<NotImplementedException>();
    }

    [Fact]
    public async Task TestGetItemsByFilterAsync_WithCondition_WithUseExpressionMapping_WhenEntityExists_MustReturnResult()
    {
        ResetList();
        var filterAndSortCondition = new FilterAndSortConditions
        {
            DisablePaging = true
        };

        var doctors = await _baseServiceWithExpression.GetItemsByFilterAsync<DoctorTestOutputModel>(filterAndSortCondition,
            x => x.Language == "English",
            OutputModelMappingTypeEnum.UseExpression, CancellationToken.None);

        doctors.Data.Count.Should().Be(2);
    }

    [Fact]
    public void
        TestGetItemsByFilterAsync_WithCondition_WithUseMappingModelMapping_WhenEntityExists_MustThrowNotImplementException()
    {
        var filterAndSortCondition = new FilterAndSortConditions
        {
            DisablePaging = true
        };

        var action = new Func<Task<DynamicQueryFilterResult<DoctorTest>>>(async () =>
            await _baseService.GetItemsByFilterAsync<DoctorTest>(filterAndSortCondition, x => x.Language == "English",
                OutputModelMappingTypeEnum.UseMappingModel, CancellationToken.None));

        action.Should().ThrowExactlyAsync<NotImplementedException>();
    }

    [Fact]
    public async Task TestGetItemsByFilterAsync_WithCondition_WithUseMappingModelMapping_WhenEntityExists_MustReturnResult()
    {
        ResetList();
        var filterAndSortCondition = new FilterAndSortConditions
        {
            DisablePaging = true
        };

        var doctors = await _baseServiceWithCustomMapping.GetItemsByFilterAsync<DoctorTestOutputModel>(filterAndSortCondition,
            x => x.Language == "English",
            OutputModelMappingTypeEnum.UseMappingModel, CancellationToken.None);

        doctors.Data.Count.Should().Be(Doctors.Count(x => x.Language == "English"));
    }


    [Fact]
    public async Task TestGetItemsByFilterAsync_WithCondition_WithPureAutoMapperMapping_WhenEntityExists_MustReturnResult()
    {
        ResetList();
        var filterAndSortCondition = new FilterAndSortConditions
        {
            DisablePaging = true
        };

        var doctors = await _baseServiceWithPureMapper.GetItemsByFilterAsync<DoctorTest>(filterAndSortCondition,
            x => x.Language == "English",
            OutputModelMappingTypeEnum.PureAutoMapper, CancellationToken.None);

        doctors.Data.Count.Should().Be(2);
    }

    [Fact]
    public void TestInsert_WithAutoMapping_MustInsertSuccessfully()
    {
        var oldCount = Doctors.Count;

        _baseService.Insert(new DoctorTest
        {
            LastName = "Basim"
        });

        var doctor = _baseService.GetItemByFilter(new FilterAndSortConditions
        {
            DisablePaging = true
        }, x => x.LastName == "Basim");

        doctor.Should().NotBeNull();
        (Doctors.Count - oldCount).Should().Be(1);
    }

    [Fact]
    public void TestInsert_WithUseAutoMapperMapping_MustInsertSuccessfully()
    {
        var oldCount = Doctors.Count;
        _baseService.Insert(new DoctorTest
        {
            LastName = "Basim"
        }, OutputModelMappingTypeEnum.UseAutoMapper);

        var doctor = _baseService.GetItemByFilter(new FilterAndSortConditions
        {
            DisablePaging = true
        }, x => x.LastName == "Basim", OutputModelMappingTypeEnum.UseAutoMapper);

        doctor.Should().NotBeNull();
        (Doctors.Count - oldCount).Should().Be(1);
    }

    [Fact]
    public void TestInsert_WithPureAutoMapperMapping_MustInsertSuccessfully()
    {
        var oldCount = Doctors.Count;
        _baseServiceWithPureMapper.Insert(new DoctorTestInputModel
        {
            LastName = "Basim"
        }, OutputModelMappingTypeEnum.PureAutoMapper);

        var doctor = _baseServiceWithPureMapper.GetItemByFilter(new FilterAndSortConditions
        {
            DisablePaging = true
        }, x => x.LastName == "Basim", OutputModelMappingTypeEnum.PureAutoMapper);

        doctor.Should().NotBeNull();
        (Doctors.Count - oldCount).Should().Be(1);
    }

    [Fact]
    public void TestInsert_WithUseExpressionMapping_MustInsertSuccessfully()
    {
        var action = new Action(() => _baseServiceWithExpression.Insert(new DoctorTestInputModel
        {
            LastName = "Basim"
        }, OutputModelMappingTypeEnum.UseExpression));

        action.Should().ThrowExactly<InvalidOperationException>();
    }

    [Fact]
    public void TestInsert_WithUseMappingModelMapping_MustInsertSuccessfully()
    {
        var action = new Action(() => _baseServiceWithCustomMapping.Insert(new DoctorTestInputModel
        {
            LastName = "Basim"
        }, OutputModelMappingTypeEnum.UseMappingModel));

        action.Should().ThrowExactly<InvalidOperationException>();
    }

    [Fact]
    public async Task TestInsertAsync_WithAutoMapping_MustInsertSuccessfully()
    {
        var oldCount = Doctors.Count;
        await _baseService.InsertAsync(new DoctorTest
        {
            LastName = "Basim"
        });

        var doctor = _baseService.GetItemByFilterAsync(new FilterAndSortConditions
        {
            DisablePaging = true
        }, x => x.LastName == "Basim");

        doctor.Should().NotBeNull();
        (Doctors.Count - oldCount).Should().Be(1);
    }

    [Fact]
    public async Task TestInsertAsync_WithUseAutoMapperMapping_MustInsertSuccessfully()
    {
        var oldCount = Doctors.Count;
        await _baseService.InsertAsync(new DoctorTest
        {
            LastName = "Basim"
        }, OutputModelMappingTypeEnum.UseAutoMapper);

        var doctor = _baseService.GetItemByFilterAsync(new FilterAndSortConditions
        {
            DisablePaging = true
        }, x => x.LastName == "Basim", OutputModelMappingTypeEnum.UseAutoMapper);

        doctor.Should().NotBeNull();
        (Doctors.Count - oldCount).Should().Be(1);
    }

    [Fact]
    public async Task TestInsertAsync_WithPureAutoMapperMapping_MustInsertSuccessfully()
    {
        var oldCount = Doctors.Count;
        await _baseServiceWithPureMapper.InsertAsync(new DoctorTestInputModel
        {
            LastName = "Basim"
        }, OutputModelMappingTypeEnum.PureAutoMapper);

        var doctor = _baseServiceWithPureMapper.GetItemByFilterAsync(new FilterAndSortConditions
        {
            DisablePaging = true
        }, x => x.LastName == "Basim", OutputModelMappingTypeEnum.PureAutoMapper);

        doctor.Should().NotBeNull();
        (Doctors.Count - oldCount).Should().Be(1);
    }

    [Fact]
    public void TestInsertAsync_WithUseExpressionMapping_MustInsertSuccessfully()
    {
        var action = new Func<Task>(async () => await _baseServiceWithExpression.InsertAsync(new DoctorTestInputModel
        {
            LastName = "Basim"
        }, OutputModelMappingTypeEnum.UseExpression));

        action.Should().ThrowExactlyAsync<InvalidOperationException>();
    }

    [Fact]
    public void TestInsertAsync_WithUseMappingModelMapping_MustInsertSuccessfully()
    {
        var action = new Func<Task>(async () => await _baseServiceWithCustomMapping.InsertAsync(new DoctorTestInputModel
        {
            LastName = "Basim"
        }, OutputModelMappingTypeEnum.UseMappingModel));

        action.Should().ThrowExactlyAsync<InvalidOperationException>();
    }

    [Fact]
    public void TestRemoveAsync_WhenEntityDoesNotFound_MustThrowException()
    {
        var action = new Func<Task>(async () => await _baseService.RemoveAsync(int.MaxValue));

        action.Should().ThrowExactlyAsync<InvalidEntityException>();
    }

    [Fact]
    public async Task TestRemoveAsync_WhenEntityExists_MustRemoved()
    {
        var oldCount = Doctors.Count;
        var doctorId = Doctors.First().DoctorId;

        await _baseService.RemoveAsync(doctorId);

        var doctor = await _baseService.GetItemByFilterAsync(new FilterAndSortConditions
        {
            DisablePaging = true
        }, x => x.DoctorId == doctorId);

        doctor.Data.Count.Should().Be(0);
        (oldCount - Doctors.Count).Should().Be(1);
    }

    [Fact]
    public void TestUpdate_WithAutoMapping_MustInsertSuccessfully()
    {
        _baseService.Update(new DoctorTest
        {
            DoctorId = 1,
            LastName = "Basim"
        });

        var doctor = _baseService.GetItemByFilter(new FilterAndSortConditions
        {
            DisablePaging = true
        }, x => x.DoctorId == 1);

        doctor.Data.First().LastName.Should().Be("Basim");
    }

    [Fact]
    public void TestUpdate_WithUseAutoMapperMapping_MustInsertSuccessfully()
    {
        _baseService.Update(new DoctorTest
        {
            DoctorId = 1,
            LastName = "Basim"
        }, OutputModelMappingTypeEnum.UseAutoMapper);

        var doctor = _baseService.GetItemByFilter(new FilterAndSortConditions
        {
            DisablePaging = true
        }, x => x.DoctorId == 1, OutputModelMappingTypeEnum.UseAutoMapper);

        doctor.Data.First().LastName.Should().Be("Basim");
    }

    [Fact]
    public void TestUpdate_WithPureAutoMapperMapping_MustInsertSuccessfully()
    {
        _baseServiceWithPureMapper.Update(new DoctorTestInputModel
        {
            DoctorId = 1,
            LastName = "Basim"
        }, OutputModelMappingTypeEnum.PureAutoMapper);

        var doctor = _baseServiceWithPureMapper.GetItemByFilter(new FilterAndSortConditions
        {
            DisablePaging = true
        }, x => x.DoctorId == 1, OutputModelMappingTypeEnum.PureAutoMapper);

        doctor.Data.First().MyLastName.Should().Be("Basim");
    }

    [Fact]
    public void TestUpdate_WithUseExpressionMapping_MustInsertSuccessfully()
    {
        var action = new Action(() => _baseServiceWithExpression.Update(new DoctorTestInputModel
        {
            DoctorId = 1,
            LastName = "Basim"
        }, OutputModelMappingTypeEnum.UseExpression));

        action.Should().ThrowExactly<InvalidOperationException>();
    }

    [Fact]
    public void TestUpdate_WithUseMappingModelMapping_MustInsertSuccessfully()
    {
        var action = new Action(() => _baseServiceWithCustomMapping.Update(new DoctorTestInputModel
        {
            DoctorId = 1,
            LastName = "Basim"
        }, OutputModelMappingTypeEnum.UseMappingModel));

        action.Should().ThrowExactly<InvalidOperationException>();
    }

    [Fact]
    public async Task TestUpdateAsync_WithAutoMapping_MustInsertSuccessfully()
    {
        await _baseService.UpdateAsync(new DoctorTest
        {
            DoctorId = 1,
            LastName = "Basim"
        });

        var doctor = await _baseService.GetItemByFilterAsync(new FilterAndSortConditions
        {
            DisablePaging = true
        }, x => x.DoctorId == 1);

        doctor.Data.First().LastName.Should().Be("Basim");
    }

    [Fact]
    public async Task TestUpdateAsync_WithUseAutoMapperMapping_MustInsertSuccessfully()
    {
        await _baseService.UpdateAsync(new DoctorTest
        {
            DoctorId = 1,
            LastName = "Basim"
        }, OutputModelMappingTypeEnum.UseAutoMapper);

        var doctor = await _baseService.GetItemByFilterAsync(new FilterAndSortConditions
        {
            DisablePaging = true
        }, x => x.DoctorId == 1, OutputModelMappingTypeEnum.UseAutoMapper);

        doctor.Data.First().LastName.Should().Be("Basim");
    }

    [Fact]
    public async Task TestUpdateAsync_WithPureAutoMapperMapping_MustInsertSuccessfully()
    {
        await _baseServiceWithPureMapper.UpdateAsync(new DoctorTestInputModel
        {
            DoctorId = 1,
            LastName = "Basim"
        }, OutputModelMappingTypeEnum.PureAutoMapper);

        var doctor = await _baseServiceWithPureMapper.GetItemByFilterAsync(new FilterAndSortConditions
        {
            DisablePaging = true
        }, x => x.DoctorId == 1, OutputModelMappingTypeEnum.PureAutoMapper);

        doctor.Data.First().MyLastName.Should().Be("Basim");
    }

    [Fact]
    public void TestUpdateAsync_WithUseExpressionMapping_MustInsertSuccessfully()
    {
        var action = new Func<Task>(async () => await _baseServiceWithExpression.UpdateAsync(new DoctorTestInputModel
        {
            DoctorId = 1,
            LastName = "Basim"
        }, OutputModelMappingTypeEnum.UseExpression));

        action.Should().ThrowExactlyAsync<InvalidOperationException>();
    }

    [Fact]
    public void TestUpdateAsync_WithUseMappingModelMapping_MustInsertSuccessfully()
    {
        var action = new Func<Task>(async () => await _baseServiceWithCustomMapping.UpdateAsync(new DoctorTestInputModel
        {
            DoctorId = 1,
            LastName = "Basim"
        }, OutputModelMappingTypeEnum.UseMappingModel));

        action.Should().ThrowExactlyAsync<InvalidOperationException>();
    }
}