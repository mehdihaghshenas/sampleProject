
using System;
using System.Linq;
using System.ComponentModel.DataAnnotations;

namespace MAction.BaseClasses
{
    public interface IBaseEntity
    {
        string GetPrimaryKeyPropertyName();
        object GetPrimaryKeyValue();
        Type GetPrimaryKeyType();
        void SetPrimaryKeyValue(object value);
    }



}
