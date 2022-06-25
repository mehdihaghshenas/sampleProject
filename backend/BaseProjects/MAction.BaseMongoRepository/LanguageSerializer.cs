using MAction.BaseClasses.Helpers;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Conventions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MAction.BaseMongoRepository
{
    public class LanguageSerializer : IBsonSerializer<LanguageEnum>
    {
        public Type ValueType => typeof(LanguageEnum);

        public void Serialize(BsonSerializationContext context, BsonSerializationArgs args, LanguageEnum value)
        {
            var bsonWriter = context.Writer;
            bsonWriter.WriteString(value.ToString() ?? "");
        }

        LanguageEnum IBsonSerializer<LanguageEnum>.Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
        {
            return Enum.Parse<LanguageEnum>(BsonSerializer.Deserialize<string>(context.Reader));
        }

        public void Serialize(BsonSerializationContext context, BsonSerializationArgs args, object value)
        {
            BsonSerializer.Serialize(context.Writer, value.ToString());
        }

        object IBsonSerializer.Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
        {
            return Enum.Parse<LanguageEnum>(BsonSerializer.Deserialize<string>(context.Reader));
        }
    }
    public static class MongoLanguageSerializerRegisteration
    {
        public static void Register()
        {
            BsonSerializer.RegisterSerializer(new LanguageSerializer());

        }
    }

}
