using AutoMapper;
using MAction.BaseClasses;
using MAction.BaseClasses.Exceptions;
using MAction.BaseServices;
using MAction.BaseServices.Configure;
using MAction.TestHelpers;
using FluentAssertions;
using Moq;
using TestHelpers;

namespace MAction.BaseProject.Tests;

public class BaseServiceWithPolicyUnitTest : IDisposable
{
    private readonly TestServiceWithPolicy _baseService;
    private readonly FakeBaseServiceDependencyProvider _dependencyProvider;

    public BaseServiceWithPolicyUnitTest()
    {
        var mockMapper = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile(new CustomMappingRegister());
            cfg.AddProfile(new CustomMappingProfile(GetType().Assembly));
        });
        var mapper = mockMapper.CreateMapper();

        var repository = new IBaseRepositoryMock<DoctorTest, int, IBaseRepository<DoctorTest, int>>();
        _dependencyProvider = new FakeBaseServiceDependencyProvider();
        _baseService = new TestServiceWithPolicy(repository.MockRepository.Object, mapper, _dependencyProvider);

        repository.Entities.AddRange(BaseServiceUnitTest.Doctors);
    }

    public void Dispose()
    {
    }

    [Fact]
    public void Test_Get_WithOut_Policy()
    {
        _dependencyProvider.RemovePolicy("TestGetPolicy");

        var action = new Action(() => _baseService.Get(BaseServiceUnitTest.Doctors.First().DoctorId));

        action.Should().ThrowExactly<InvalidEntityException>();
    }

    [Fact]
    public void Test_Insert_WithOut_Policy()
    {
        _dependencyProvider.RemovePolicy("TestInsertPolicy");

        var action = new Action(() => _baseService.Insert(new DoctorTest()));

        action.Should().ThrowExactly<ForbiddenExpection>();
    }

    [Fact]
    public void Test_Update_WithOut_Policy()
    {
        _dependencyProvider.RemovePolicy("TestUpdatePolicy");

        var action = new Action(() => _baseService.Update(new DoctorTest()));

        action.Should().ThrowExactly<ForbiddenExpection>();
    }

    [Fact]
    public void Test_Remove_WithOut_Policy()
    {
        _dependencyProvider.RemovePolicy("TestDeletePolicy");

        var action = new Action(() => _baseService.RemoveAsync(BaseServiceUnitTest.Doctors.First().DoctorId));

        action.Should().ThrowExactly<ForbiddenExpection>();
    }

    [Fact]
    public void Test_Get_With_Policy()
    {
        _dependencyProvider.WithPolicy("TestGetPolicy");

        var doctor = _baseService.Get(BaseServiceUnitTest.Doctors.First().DoctorId);

        doctor.Should().NotBeNull();
        doctor.Rate.Should().Be(4);
    }

    [Fact]
    public void Test_Insert_With_Policy()
    {
        _dependencyProvider.WithPolicy("TestInsertPolicy");

        var doctor = _baseService.Insert(new DoctorTest
        {
            LastName = "Basim",
            BirthDate = DateTime.Parse("1989/05/08"),
            Language = "Persian",
            Speciality = "Nosal surgeon",
            YearStartPractice = 2013,
            Rate = 4,
            Offices = new List<Office>
            {
                new()
                {
                    OfficeId = 5,
                    AddressDoctor = new AddressDoctor {AddressId = 5, StateName = "tehran"}
                }
            },
            DoctorId = 5
        });

        doctor.Should().NotBeNull();
        doctor.Rate.Should().Be(4);
    }

    [Fact]
    public void Test_Update_With_Policy()
    {
        _dependencyProvider.WithPolicy("TestUpdatePolicy");
        _dependencyProvider.WithPolicy("TestGetPolicy");

        _baseService.Update(new DoctorTest
        {
            LastName = "Smith",
            BirthDate = DateTime.Parse("1980/05/08"),
            Language = "Spanish",
            Speciality = "Nosal surgeon",
            YearStartPractice = 2010,
            Rate = 3,
            Offices = new List<Office>
            {
                new()
                {
                    OfficeId = 1,
                    AddressDoctor = new AddressDoctor {AddressId = 1, StateName = "california"}
                }
            },
            DoctorId = 1
        });

        var doctor = _baseService.Get(BaseServiceUnitTest.Doctors.First(x => x.LastName == "Smith").DoctorId);

        doctor.Should().NotBeNull();
        doctor.Rate.Should().Be(3);
    }

    [Fact]
    public void Test_Remove_With_Policy()
    {
        _dependencyProvider.WithPolicy("TestDeletePolicy");
        _dependencyProvider.WithPolicy("TestGetPolicy");

        _baseService.RemoveAsync(BaseServiceUnitTest.Doctors.First(x => x.LastName == "Smith").DoctorId);

        var action = new Action(() =>
            _baseService.Get(BaseServiceUnitTest.Doctors.First(x => x.LastName == "Smith").DoctorId));

        action.Should().ThrowExactly<InvalidEntityException>();
    }

    [Fact]
    public void Test_All_With_Admin_Policy()
    {
        _dependencyProvider.WithRole("Admin");
        _dependencyProvider.RemoveAllPolicy();

        _baseService.RemoveAsync(BaseServiceUnitTest.Doctors.First(x => x.LastName == "Smith").DoctorId);

        var action = new Action(() =>
            _baseService.Get(BaseServiceUnitTest.Doctors.First(x => x.LastName == "Smith").DoctorId));

        action.Should().ThrowExactly<InvalidEntityException>();
    }
}