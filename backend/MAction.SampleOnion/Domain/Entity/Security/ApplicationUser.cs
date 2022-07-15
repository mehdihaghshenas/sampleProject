using AspNetCore.Identity.Mongo.Model;
using MAction.AspNetIdentity.Base.Entities;
using MAction.BaseClasses;
using MongoDB.Bson;
using System;

namespace MAction.SipOnline.Domain.Entity.Security;
public class ApplicationUser : MongoUser<ObjectId>, IBaseEntity, IEntityCreationInfo, IUser
{

    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string FullName => $"{FirstName} {LastName}";
    public string OrganizationName { get; set; }
    public DateTimeOffset CreateAt { get; set; }
    public string TimeZone { get; set; }
    public string UserCreationId { get; set; }
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
        if (value is ObjectId id)
            Id = id;
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