using MAction.BaseMongoRepository;
using MAction.SampleOnion.Domain.Entity.SAL;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using System;

namespace MAction.SampleOnion.Repository.EntityConfigurations
{
    public class CategoryConfiguration : IMongoEntityTypeConfiguration<Category>
    {
        public Action<BsonClassMap<Category>> Configure() => x =>
        {
            x.AutoMap();
            x.MapIdMember(c => c.Id);
        };

    }
}