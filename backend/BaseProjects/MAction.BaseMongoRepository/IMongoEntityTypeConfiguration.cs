using MongoDB.Bson.Serialization;
using System;

namespace MAction.BaseMongoRepository
{
    public interface IMongoEntityTypeConfiguration<T>
    {
        public Action<BsonClassMap<T>> Configure();
    }
}