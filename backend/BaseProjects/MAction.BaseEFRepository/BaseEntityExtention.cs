using MAction.BaseClasses;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MAction.BaseEFRepository
{
    public static class BaseEntityEFExtention 
    {
        public static void MapBase(this BaseEntityWithCreationInfo entity, EntityTypeBuilder builder)
        {
            builder.Property<DateTimeOffset>("CreateAt").HasColumnType("datetimeoffset(7)").HasDefaultValueSql("SYSDATETIMEOFFSET()");
        }
        public static void CallMapBaseForBaseEntityWithCreationInfoOnModelCreating(this ModelBuilder modelBuilder, Type type)
        {
            var MapBaseList = Assembly.GetAssembly(type).GetExportedTypes()
    .Where(c => c.IsClass && !c.IsAbstract && c.IsPublic && typeof(BaseEntityWithCreationInfo).IsAssignableFrom(c) && !c.GetCustomAttributes(typeof(NotMappedAttribute), true).Any());
            foreach(var t in MapBaseList)
            {
                var mapobj = t.GetConstructor(Array.Empty<Type>()).Invoke(Array.Empty<object>());

                (mapobj as BaseEntityWithCreationInfo).MapBase(modelBuilder.Entity(t.FullName));
            }
        }
    }
}
