using MAction.BaseClasses;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MAction.AspNetIdentity.EFCore.Domain;
public class ApplicationUser : IdentityUser<int>, IBaseEntity
{
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string FullName => $"{FirstName} {LastName}";

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
}
public class ApplicationUserConfiguration : IEntityTypeConfiguration<ApplicationUser>
{
    public void Configure(EntityTypeBuilder<ApplicationUser> builder)
    {
        builder.ToTable("Users", "Security");
        builder.Property(p => p.FirstName).HasMaxLength(64).IsRequired();
        builder.Property(p => p.LastName).HasMaxLength(64).IsRequired();
        builder.Property(p => p.Id).HasColumnName("UserId");


        builder.HasData(new ApplicationUser
        {
            Id = 1,
            FirstName = "Super",
            LastName = "Admin",
            UserName = "admin",
            NormalizedUserName = "ADMIN",
            Email = "admin@admin.com",
            NormalizedEmail = "ADMIN@ADMIN.COM",
            EmailConfirmed = true,
            PasswordHash = new PasswordHasher<ApplicationUser>().HashPassword(new ApplicationUser(), "adminadmin"),
            SecurityStamp = string.Empty,
            AccessFailedCount = 0
        });

    }
}