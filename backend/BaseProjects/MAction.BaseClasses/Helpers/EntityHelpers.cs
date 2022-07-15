using MAction.BaseClasses.Language;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MAction.BaseClasses.Helpers
{
    public static class EntityHelpers
    {
        public static void SetRequiredDateForInsert<T>(this T entity, IBaseServiceDependencyProvider baseServiceDependencyProvider) where T : IBaseEntity
        {
            LanguageHelpers.SetLanguagePropertyInfo<T>(entity);
            if (typeof(IEntityCreationInfo).IsAssignableFrom(typeof(T)))
            {
                (entity as IEntityCreationInfo)!.CreateAt = baseServiceDependencyProvider.HasSystemPrivilege() ? DateTimeOffset.UtcNow : TimeZoneInfo.ConvertTime(DateTimeOffset.UtcNow, baseServiceDependencyProvider.TimeZoneConverterService.GetClientTimeZoneInfo());
                (entity as IEntityCreationInfo)!.TimeZone = baseServiceDependencyProvider.TimeZoneConverterService.GetClientTimeZoneInfo().StandardName; // TODO Get from basedependency provider
                (entity as IEntityCreationInfo)!.UserCreationId = baseServiceDependencyProvider.UserId?.ToString(); //TODO Get From base dependency
            }
        }
    }
}
