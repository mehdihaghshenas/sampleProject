using AspNetCore.Identity.Mongo.Model;
using MAction.BaseClasses;
using MAction.BaseMongoRepository;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;

namespace MAction.AspNetIdentity.Mongo.Domain;
public class ApplicationUser : MongoUser<ObjectId>, IBaseEntity
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string FullName => $"{FirstName} {LastName}";

    public string OrganizationName { get; set; }
    public string GetPrimaryKeyPropertyName()
    {
        return nameof(Id);
    }

    public Type GetPrimaryKeyType()
    {
        return Id.GetType();
    }

    public object GetPrimaryKeyValue()
    {
        return Id;
    }

    public void SetPrimaryKeyValue(object value)
    {
        if (value.GetType() == typeof(ObjectId))
            Id = (ObjectId)value;
        else
            Id = ObjectId.Parse(value.ToString());
    }
}
//We not need this part for now
//public class ApplicationUserConfiguration : IMongoEntityTypeConfiguration<ApplicationUser>
//{
//    public Action<BsonClassMap<ApplicationUser>> Configure()
//    {
//        return x => new BsonClassMap<ApplicationUser>()
//        {
//        };
//    }
//}