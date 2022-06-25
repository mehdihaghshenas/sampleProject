using MAction.SampleOnion.Domain.Entity.SAL;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MAction.SampleOnion.Repository.EntityConfigurations
{
    internal class EFCoreEntityConfigurations
    {
        public class SaleCompanyRegisterConfiguration : IEntityTypeConfiguration<SaleCompany>
        {
            public void Configure(EntityTypeBuilder<SaleCompany> builder)
            {
                throw new NotImplementedException();
            }
        }
    }
}
