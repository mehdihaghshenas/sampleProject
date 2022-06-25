using AspNetCore.Identity.Mongo.Model;
using MAction.BaseClasses;
using MongoDB.Bson;

namespace MAction.AspNetIdentity.Mongo.Domain;

public class ApplicationRole : MongoRole<ObjectId>, IBaseEntity
{
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
