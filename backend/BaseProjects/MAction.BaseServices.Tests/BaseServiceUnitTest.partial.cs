namespace MAction.BaseServices.Tests;

public partial class BaseServiceUnitTest
{
    private static List<DoctorTest> _list;

    internal static void ResetList()
    {
        _list = new List<DoctorTest>()
    {
        new DoctorTest
        {
            LastName = "Smith",
            BirthDate = DateTime.Parse("1980/05/08"),
            Language = "Spanish",
            Speciality = "Nosal surgeon",
            YearStartPractice = 2010,
            Rate = 4,
            Offices = new List<Office>
            {
                new()
                {
                    OfficeId = 1,
                    AddressDoctor = new AddressDoctor {AddressId = 1, StateName = "california"}
                }
            },
            DoctorId = 1
        },
        new DoctorTest
        {
            LastName = "Jack",
            BirthDate = DateTime.Parse("1970/09/08"),
            Language = "English",
            Speciality = "Nosal surgeon",
            YearStartPractice = 2000,
            Rate = 2,
            Offices = new List<Office>
            {
                new()
                {
                    OfficeId = 2,
                    AddressDoctor = new AddressDoctor {AddressId = 2, StateName = "california"}
                }
            },
            DoctorId = 2
        },
        new DoctorTest
        {
            LastName = "Michael",
            BirthDate = DateTime.Parse("1979/09/07"),
            Language = "English",
            Speciality = "Plastic surgeon",
            YearStartPractice = 2000,
            Rate = 3,
            Offices = new List<Office>
            {
                new()
                {
                    OfficeId = 3,
                    AddressDoctor = new AddressDoctor {AddressId = 3, StateName = "NewYork"}
                }
            },
            DoctorId = 3
        },
        new DoctorTest
        {
            LastName = "Sara",
            BirthDate = DateTime.Parse("1988/09/07"),
            Language = "French",
            Speciality = "Dentist",
            YearStartPractice = 2010,
            Rate = 3,
            Offices = new List<Office>
            {
                new()
                {
                    OfficeId = 4,
                    AddressDoctor = new AddressDoctor {AddressId = 4, StateName = "San Francisco"}
                }
            },
            DoctorId = 4
        }
        };
    }
    static BaseServiceUnitTest()
    {
        _list = new List<DoctorTest>();
        ResetList();
    }
    private static List<DoctorTest> Doctors { get => _list; }


}