using Microsoft.Extensions.DependencyInjection;
using System;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using MAction.BaseMongoRepository;
using static MAction.SampleOnion.Repository.EntityConfigurations.EFCoreEntityConfigurations;

namespace MAction.SampleOnion.Repository
{
    public partial class ServiceRegistration
    {
        public static void AddConfigureService(IServiceCollection services, IConfiguration configuration)
        {
            ConfigureMongoEntityTypes.ApplyConfigurationsFromAssembly(typeof(SaleCompanyRegisterConfiguration).Assembly);
            services.AddTransient(typeof(IMongoRepository<>), typeof(MongoRepository<>));
            var connectionBuilder = new MongoUrlBuilder(configuration.GetConnectionString("MactionMongoDBConnection"));
            services.AddTransient<IMongoDependencyProvider, TempMongoDependencyProvider>(x => new() { DatabaseName = connectionBuilder.DatabaseName });
            services.AddTransient<IMongoClient, MongoClient>(p =>
            {
                var uri = connectionBuilder.ToMongoUrl();
                return new MongoClient(uri);

            });


            //use instead of the below in Map
            //Use IsteadOf         [BsonRepresentation(BsonType.String)]
            //x.MapMember(c => c.Language).SetSerializer(new EnumSerializer<LanguageEnum>(BsonType.String));
            MongoLanguageSerializerRegisteration.Register();
        }

    }
}