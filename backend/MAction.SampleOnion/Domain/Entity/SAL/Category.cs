using System.ComponentModel.DataAnnotations;
using MAction.BaseClasses.Language;
using MAction.BaseClasses.Helpers;

namespace MAction.SampleOnion.Domain.Entity.SAL;

public class Category : BaseEntity
{
    [Key] 
    public int Id { get; set; }

    public string Key { get; set; }

    public Translation Translation { get; set; }
}