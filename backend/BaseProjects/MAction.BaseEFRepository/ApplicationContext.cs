using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;
using MAction.BaseClasses.Extentions;

namespace MAction.BaseEFRepository
{
    public class ApplicationContext : DbContext
    {
        private Type _domainType;
        private readonly Type[] _domainTypes;

        public ApplicationContext(Type DomainType, Type[] DomainTypes = null)
        {
            _domainType = DomainType;
            _domainTypes = DomainTypes;
        }

        public ApplicationContext(DbContextOptions options, Type DomainType, Type[] DomainTypes = null) : base(options)
        {
            _domainType = DomainType;
            _domainTypes = DomainTypes;
        }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            DBContextExtentionAndConfiguration.OnModelCreating(modelBuilder, _domainType, _domainTypes?.ToList());
            DBContextExtentionAndConfiguration.OnModelCreatingAddLanguage(modelBuilder);

            base.OnModelCreating(modelBuilder);
            //using reflectyion to Map Data


        }
        #region replacestrignPersianBug
        public override int SaveChanges()
        {
            DBContextExtentionAndConfiguration.CleanString(ChangeTracker);
            return base.SaveChanges();
        }

        public override int SaveChanges(bool acceptAllChangesOnSuccess)
        {
            DBContextExtentionAndConfiguration.CleanString(ChangeTracker);
            return base.SaveChanges(acceptAllChangesOnSuccess);
        }

        public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
        {
            DBContextExtentionAndConfiguration.CleanString(ChangeTracker);
            return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            DBContextExtentionAndConfiguration.CleanString(ChangeTracker);
            return base.SaveChangesAsync(cancellationToken);
        }
        #endregion

    }
}
