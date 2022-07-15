using MAction.BaseClasses;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
namespace MAction.BaseEFRepository.Tests;

public class DoctorTest : BaseEntityWithCreationInfo, IBaseEntityTypeConfiguration<DoctorTest>
{
    [Key]
    public int DoctorId { get; set; }
    public string LastName { get; set; } = "";
    public string Speciality { get; set; } = "";
    public DateTime BirthDate { get; set; }
    public string Language { get; set; } = "EN";
    public int YearStartPractice { get; set; }
    public int Rate { get; set; }
    public ICollection<Office> Offices { get; set; }

    public DoctorTest()
    {
        Offices = new HashSet<Office>();
    }

    public void Configure(EntityTypeBuilder<DoctorTest> builder)
    {
        // builder.Property(x=>x.UserCreationId).HasConversion<int>();
        // builder.Property(x=>x.UserCreationId).HasColumnType("int");
    }
}

public class Office
{

    [Key]
    public int OfficeId { get; set; }
    public string Email { get; set; } = "";
    public string Phone { get; set; } = "";

    public int? AddressId { get; set; }
    [ForeignKey("AddressId")]
    public AddressDoctor? AddressDoctor { get; set; }
}

public class AddressDoctor
{
    [Key]
    public int AddressId { get; set; }
    public string StateName { get; set; } = "";
    public string AptNumber { get; set; } = "";
}
