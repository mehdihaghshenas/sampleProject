using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using MAction.BaseClasses;

namespace MAction.BaseProject.Tests;

public class DoctorTest : BaseEntityWithCreationInfo
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
}

public class Office :BaseEntityWithCreationInfo
{

    [Key]
    public int OfficeId { get; set; }
    public string Email { get; set; } = "";
    public string Phone { get; set; } = "";

    public int? AddressId { get; set; }
    [ForeignKey("AddressId")]
    public AddressDoctor? AddressDoctor { get; set; }
}

public class AddressDoctor :BaseEntityWithCreationInfo
{
    [Key]
    public int AddressId { get; set; }
    public string StateName { get; set; } = "";
    public string AptNumber { get; set; } = "";
}
