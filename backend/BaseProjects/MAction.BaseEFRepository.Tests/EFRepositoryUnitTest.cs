using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Faker;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Xunit;
using MAction.BaseClasses.Exceptions;
using MAction.BaseEFRepository;

namespace MAction.BaseEFRepository.Tests;

public partial class EFRepositoryUnitTest : IDisposable
{
    private readonly TestContext testContext;

    private readonly EFRepository<DoctorTest> baseRepository;
    public EFRepositoryUnitTest()
    {
        var options = new DbContextOptionsBuilder<TestContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning))
            .EnableSensitiveDataLogging()
            .Options;

        testContext = new TestContext(options);
        testContext.Doctors.AddRange(Doctors);
        testContext.SaveChanges();

        baseRepository = new EFRepository<DoctorTest>(testContext);
    }

    public void Dispose()
    {
        testContext?.Dispose();
    }

    [Fact]
    public async Task TestGetAll_WhenEverythingIsOkAndNoFilterAdded_MustReturnAllData()
    {
        var doctors = await baseRepository.GetAll().ToListAsync();

        doctors.Should().HaveCount(Doctors.Count);
    }

    [Fact]
    public async Task TestGetAll_WhenTestReturnType_MustBeSame()
    {
        var doctors = await baseRepository.GetAll().ToListAsync();

        doctors.Should().BeOfType<List<DoctorTest>>();
    }

    [Fact]
    public void TestGet_WhenEntityDoesNotFound_MustThrowException()
    {
        var action = baseRepository.Get(int.MaxValue);

        action.Should().BeNull();
    }

    [Fact]
    public void TestGet_WhenEntityExists_MustReturnResult()
    {
        var doctor = baseRepository.Get(Doctors.First().DoctorId);

        doctor.Should().NotBeNull();
    }

    [Fact]
    public void TestGet_WhenTestReturnType_MustBeSame()
    {
        var doctor = baseRepository.Get(Doctors.First().DoctorId);

        doctor.Should().BeOfType<DoctorTest>();
    }

    [Fact]
    public void TestGetById_WhenEntityExists_MustReturnResult()
    {
        var doctor = baseRepository.Get(Doctors.First().DoctorId);

        doctor.Should().NotBeNull();
    }

    [Fact]
    public void TestGetById_WhenTestReturnType_MustBeSame()
    {
        var doctor = baseRepository.Get(Doctors.First().DoctorId);

        doctor.Should().BeOfType<DoctorTest>();
    }

    [Fact]
    public async Task TestGetAsync_WhenEntityDoesNotFound_MustThrowException()
    {
        var doctor = await baseRepository.GetAsync(int.MaxValue);

        doctor.Should().BeNull();
    }

    [Fact]
    public async Task TestGetAsync_WhenEntityExists_MustReturnResult()
    {
        var doctor = await baseRepository.GetAsync(Doctors.First().DoctorId);

        doctor.Should().NotBeNull();
    }

    [Fact]
    public async Task TestGetAsync_WhenTestReturnType_MustBeSame()
    {
        var doctor = await baseRepository.GetAsync(Doctors.First().DoctorId);

        doctor.Should().BeOfType<DoctorTest>();
    }

    [Fact]
    public async Task TestGetByIdAsync_WhenEntityExists_MustReturnResult()
    {
        var doctor = await baseRepository.GetAsync(Doctors.First().DoctorId);

        doctor.Should().NotBeNull();
    }

    [Fact]
    public async Task TestGetByIdAsync_WhenTestReturnType_MustBeSame()
    {
        var doctor = await baseRepository.GetAsync(Doctors.First().DoctorId);

        doctor.Should().BeOfType<DoctorTest>();
    }

    [Fact]
    public void TestInsert_WhenEverythingIsOk_MustAddedToContext()
    {
        var doctor = new DoctorTest
        {
            LastName = Name.FullName(),
            BirthDate = Identification.DateOfBirth(),
            Language = "Spanish",
            Speciality = Lorem.GetFirstWord(),
            YearStartPractice = RandomNumber.Next(),
            Rate = RandomNumber.Next(),
            Offices = new List<Office>
            {
                new()
                {
                    OfficeId = RandomNumber.Next(),
                    AddressDoctor = new AddressDoctor {AddressId = 10, StateName = Country.Name()}
                }
            },
            DoctorId = 10
        };

        baseRepository.Insert(doctor);
        baseRepository.SaveChanges();

        var doctors = baseRepository.GetAll();
        doctors.Should().HaveCount(Doctors.Count + 1);
        var createdDoctor = doctors.Single(i => i.DoctorId == 10);

        createdDoctor.LastName.Should().Be(doctor.LastName);
        createdDoctor.BirthDate.Should().Be(doctor.BirthDate);
        createdDoctor.Language.Should().Be(doctor.Language);
        createdDoctor.Rate.Should().Be(doctor.Rate);
        createdDoctor.YearStartPractice.Should().Be(doctor.YearStartPractice);
        createdDoctor.Offices.Should().BeEquivalentTo(doctor.Offices);
    }

    [Fact]
    public void TestInsert_WhenEntityIsNull_MustThrowException()
    {
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
        var action = new Action(() => baseRepository.Insert(null));
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.

        action.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public async Task TestInsertAsync_WhenEverythingIsOk_MustAddedToContext()
    {
        var doctor = new DoctorTest
        {
            LastName = Name.FullName(),
            BirthDate = Identification.DateOfBirth(),
            Language = "Spanish",
            Speciality = Lorem.GetFirstWord(),
            YearStartPractice = RandomNumber.Next(),
            Rate = RandomNumber.Next(),
            Offices = new List<Office>
            {
                new()
                {
                    OfficeId = 10,
                    AddressDoctor = new AddressDoctor {AddressId = 10, StateName = Country.Name()}
                }
            },
            DoctorId = 10
        };

        await baseRepository.InsertAsync(doctor);
        await baseRepository.SaveChangesAsync();

        var doctors = await baseRepository.GetAll().ToListAsync();
        doctors.Should().HaveCount(Doctors.Count + 1);
        var createdDoctor = doctors.Single(i => i.DoctorId == 10);

        createdDoctor.LastName.Should().Be(doctor.LastName);
        createdDoctor.BirthDate.Should().Be(doctor.BirthDate);
        createdDoctor.Language.Should().Be(doctor.Language);
        createdDoctor.Rate.Should().Be(doctor.Rate);
        createdDoctor.YearStartPractice.Should().Be(doctor.YearStartPractice);
        createdDoctor.Offices.Should().BeEquivalentTo(doctor.Offices);
    }

    [Fact]
    public async Task TestInsertAsync_WhenEntityIsNull_MustThrowException()
    {
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
        var func = new Func<Task>(async () => await baseRepository.InsertAsync(null));
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.

        await func.Should().ThrowAsync<ArgumentNullException>();
    }

    [Fact]
    public void TestInsertWithSaveChange_WhenEverythingIsOk_MustAddedToContext()
    {
        var doctor = new DoctorTest
        {
            LastName = Name.FullName(),
            BirthDate = Identification.DateOfBirth(),
            Language = "Spanish",
            Speciality = Lorem.GetFirstWord(),
            YearStartPractice = RandomNumber.Next(),
            Rate = RandomNumber.Next(),
            Offices = new List<Office>
            {
                new()
                {
                    OfficeId = RandomNumber.Next(),
                    AddressDoctor = new AddressDoctor {AddressId = 10, StateName = Country.Name()}
                }
            },
            DoctorId = 10
        };

        baseRepository.InsertWithSaveChange(doctor);

        var doctors = baseRepository.GetAll();
        doctors.Should().HaveCount(Doctors.Count + 1);
        var createdDoctor = doctors.Single(i => i.DoctorId == 10);

        createdDoctor.LastName.Should().Be(doctor.LastName);
        createdDoctor.BirthDate.Should().Be(doctor.BirthDate);
        createdDoctor.Language.Should().Be(doctor.Language);
        createdDoctor.Rate.Should().Be(doctor.Rate);
        createdDoctor.YearStartPractice.Should().Be(doctor.YearStartPractice);
        createdDoctor.Offices.Should().BeEquivalentTo(doctor.Offices);
    }

    [Fact]
    public void TestInsertWithSaveChange_WhenEntityIsNull_MustThrowException()
    {
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
        var action = new Action(() => baseRepository.InsertWithSaveChange(null));
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.

        action.Should().Throw<ArgumentNullException>();
    }


    [Fact]
    public async Task TestInsertWithSaveChangeAsync_WhenEverythingIsOk_MustAddedToContext()
    {
        var doctor = new DoctorTest
        {
            LastName = Name.FullName(),
            BirthDate = Identification.DateOfBirth(),
            Language = "Spanish",
            Speciality = Lorem.GetFirstWord(),
            YearStartPractice = RandomNumber.Next(),
            Rate = RandomNumber.Next(),
            Offices = new List<Office>
                {
                    new()
                    {
                        OfficeId = 10,
                        AddressDoctor = new AddressDoctor {AddressId = 10, StateName = Country.Name()}
                    }
                },
            DoctorId = 10
        };

        await baseRepository.InsertWithSaveChangeAsync(doctor);

        var doctors = await baseRepository.GetAll().ToListAsync();
        doctors.Should().HaveCount(Doctors.Count + 1);
        var createdDoctor = doctors.Single(i => i.DoctorId == 10);

        createdDoctor.LastName.Should().Be(doctor.LastName);
        createdDoctor.BirthDate.Should().Be(doctor.BirthDate);
        createdDoctor.Language.Should().Be(doctor.Language);
        createdDoctor.Rate.Should().Be(doctor.Rate);
        createdDoctor.YearStartPractice.Should().Be(doctor.YearStartPractice);
        createdDoctor.Offices.Should().BeEquivalentTo(doctor.Offices);
    }


    [Fact]
    public async Task TestInsertWithSaveChangeAsync_WhenEntityIsNull_MustThrowException()
    {
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
        var func = new Func<Task>(async () => await baseRepository.InsertWithSaveChangeAsync(null));
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.

        await func.Should().ThrowAsync<ArgumentNullException>();
    }


    [Fact]
    public void TestUpdateWithSaveChange_WhenEverythingIsOkAndActivityTypeIsNull_MustUpdateSuccessfully()
    {
        var doctor = baseRepository.Get(1);
        doctor.LastName = Name.FullName();
        doctor.BirthDate = Identification.DateOfBirth();
        doctor.Language = Lorem.GetFirstWord();
        doctor.YearStartPractice = RandomNumber.Next();

        baseRepository.UpdateWithSaveChange(doctor);

        var updatedDoctor = baseRepository.Get(1);
        updatedDoctor.LastName.Should().Be(doctor.LastName);
        updatedDoctor.BirthDate.Should().Be(doctor.BirthDate);
        updatedDoctor.Language.Should().Be(doctor.Language);
        updatedDoctor.YearStartPractice.Should().Be(doctor.YearStartPractice);
    }


    [Fact]
    public void TestUpdateWithSaveChange_WhenEntityIsNull_MustThrowException()
    {
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
        var action = new Action(() => baseRepository.UpdateWithSaveChange(null));
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.

        action.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public async Task TestUpdateWithSaveChangeAsync_WhenEverythingIsOkAndActivityTypeIsNull_MustUpdateSuccessfully()
    {
        var doctor = await baseRepository.GetAsync(1);
        doctor.LastName = Name.FullName();
        doctor.BirthDate = Identification.DateOfBirth();
        doctor.Language = Lorem.GetFirstWord();
        doctor.YearStartPractice = RandomNumber.Next();

        await baseRepository.UpdateWithSaveChangeAsync(doctor);

        var updatedDoctor = await baseRepository.GetAsync(1);
        updatedDoctor.LastName.Should().Be(doctor.LastName);
        updatedDoctor.BirthDate.Should().Be(doctor.BirthDate);
        updatedDoctor.Language.Should().Be(doctor.Language);
        updatedDoctor.YearStartPractice.Should().Be(doctor.YearStartPractice);
    }


    [Fact]
    public async Task TestUpdateWithSaveChangeAsync_WhenEntityIsNull_MustThrowException()
    {
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
        var func = new Func<Task>(async () => await baseRepository.UpdateWithSaveChangeAsync(null));
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.

        await func.Should().ThrowAsync<ArgumentNullException>();
    }

    [Fact]
    public void TestRemoveWithSaveChange_WhenEverythingIsOk_MustRemoveSuccessfully()
    {
        baseRepository.RemoveWithSaveChange(Doctors.First().DoctorId);

        var doctors = baseRepository.GetAll();
        doctors.Should().HaveCount(3);
    }

    [Fact]
    public async Task TestRemoveWithSaveChangeAsync_WhenEverythingIsOk_MustRemoveSuccessfully()
    {
        await baseRepository.RemoveWithSaveChangeAsync(Doctors.First().DoctorId);

        var doctors = baseRepository.GetAll();
        doctors.Should().HaveCount(3);
    }


    [Fact]
    public void TestRemove_WhenEntityIsNull_MustException()
    {
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
        var action = new Action(() => baseRepository.Remove(null));
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
        action.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public async Task TestRemoveAsync_WhenEntityIsNull_MustException()
    {
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
        var func = new Func<Task>(async () => await baseRepository.RemoveAsync(null));
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
        await func.Should().ThrowAsync<ArgumentNullException>();

    }

}