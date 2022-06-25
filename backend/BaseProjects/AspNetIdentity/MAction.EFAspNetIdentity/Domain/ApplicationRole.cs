using MAction.AspNetIdentity.Base;
using MAction.BaseClasses;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MAction.AspNetIdentity.EFCore.Domain;

public class ApplicationRole : IdentityRole<int>, IBaseEntity
{
    public string GetPrimaryKeyPropertyName()
    {
        return nameof(Id);
    }

    public Type GetPrimaryKeyType()
    {
        return Id.GetType();
    }

    public object GetPrimaryKeyValue()
    {
        return Id;
    }

    public void SetPrimaryKeyValue(object value)
    {
        if (value.GetType() == typeof(int))
            Id = (int)value;
        else
            Id = int.Parse(value.ToString() ?? "0");
    }
    public bool IsSystem { get; set; }

}
public class ApplicationRoleConfiguration : IEntityTypeConfiguration<ApplicationUser>
{
    public void Configure(EntityTypeBuilder<ApplicationUser> builder)
    {
        builder.ToTable("Roles", "Security");
        builder.Property(p => p.FirstName).HasMaxLength(64).IsRequired();
        builder.Property(p => p.LastName).HasMaxLength(64).IsRequired();
        builder.Property(p => p.Id).HasColumnName("RoleId");

        builder.HasData(new ApplicationRole { Id = (int)SystemRolesEnum.Admin, Name = nameof(SystemRolesEnum.Admin), NormalizedName = nameof(SystemRolesEnum.Admin).ToUpper(), IsSystem = true });
        builder.HasData(new ApplicationRole { Id = (int)SystemRolesEnum.User, Name = nameof(SystemRolesEnum.User), NormalizedName = nameof(SystemRolesEnum.User).ToUpper(), IsSystem = true });


    }
}
