using MongoDB.Bson;
using MAction.BaseClasses;
using AspNetCore.Identity.Mongo.Model;
using MAction.AspNetIdentity.Base.Entities;
using System;

namespace MAction.SipOnline.Domain.Entity.Security;

public class ApplicationRole : MongoRole<ObjectId>, IBaseEntity, IRole
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
