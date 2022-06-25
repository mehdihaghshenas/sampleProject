using MongoDB.Bson.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MAction.BaseMongoRepository
{
    public class ConfigureMongoEntityTypes
    {
        /// <summary>
        ///public static void EntityTypeConfiguration()
        ///{
        ///    BsonClassMap.RegisterClassMap<SaleCompany>(cm =>
        ///    {
        ///        cm.AutoMap();
        ///        cm.MapIdMember(c => c.Id);
        ///    });
        ///}
        /// </summary>
        /// <param name="assembly"> apply ResgisterClassMap for all Entities in Selected Assemply</param>
        /// <param name="predicate"></param>
        public static void ApplyConfigurationsFromAssembly(
            Assembly assembly,
            Func<Type, bool>? predicate = null)
        {
            foreach (var type in assembly.GetExportedTypes().OrderBy(t => t.FullName))
            {
                // Only accept types that contain a parameterless constructor, are not abstract and satisfy a predicate if it was used.
                if (type.GetConstructor(Type.EmptyTypes) == null || (!predicate?.Invoke(type) ?? false))
                {
                    continue;
                }

                foreach (var @interface in type.GetInterfaces())
                {
                    if (!@interface.IsGenericType)
                    {
                        continue;
                    }

                    if (@interface.GetGenericTypeDefinition() == typeof(IMongoEntityTypeConfiguration<>))
                    {
                        var mapobj = Activator.CreateInstance(type);
                        var applyEntityConfigurationMethod = type.GetMethod("Configure")!;
                        var classMap = applyEntityConfigurationMethod.Invoke(mapobj, Type.EmptyTypes);
                        typeof(BsonClassMap).GetMethods(BindingFlags.Public | BindingFlags.Static).First(x=>x.Name=="RegisterClassMap" && x.GetParameters().Length==1 ).
                            MakeGenericMethod(@interface.GenericTypeArguments[0]).Invoke(null, new[] { classMap });
                    }
                }
            }
        }
    }
}
