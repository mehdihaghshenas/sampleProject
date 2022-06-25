using MAction.BaseClasses.Helpers;
using MAction.BaseMongoRepository;
using MAction.SampleOnion.Domain.Entity.SAL;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using System;

namespace MAction.SampleOnion.Repository.EntityConfigurations
{
    public class SaleCompanyConfiguration : IMongoEntityTypeConfiguration<SaleCompany>
    {
        public Action<BsonClassMap<SaleCompany>> Configure() => x =>
        {
            x.AutoMap();
            x.MapIdMember(c => c.Id);

            //Use IsteadOf         [BsonRepresentation(BsonType.String)]
            //x.MapMember(c => c.Language).SetSerializer(new EnumSerializer<LanguageEnum>(BsonType.String));
        };


    }
}