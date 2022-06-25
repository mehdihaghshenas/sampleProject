using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MAction.BaseEFRepository;

public interface IBaseEntityTypeConfiguration<T> where T : class
{
    void Configure(EntityTypeBuilder<T> builder);
}