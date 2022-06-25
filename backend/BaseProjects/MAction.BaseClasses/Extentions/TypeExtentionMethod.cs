using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MAction.BaseClasses.Extentions;

public static class TypeExtentionMethod
{
    public static bool IsAssignableToGenericType(this Type givenType, Type genericType)
    {
        var interfaceTypes = givenType.GetInterfaces();

        foreach (var it in interfaceTypes)
        {
            if (it.IsGenericType && it.GetGenericTypeDefinition() == genericType)
                return true;
        }

        if (givenType.IsGenericType && givenType.GetGenericTypeDefinition() == genericType)
            return true;

        Type baseType = givenType.BaseType;
        if (baseType == null)
            return false;

        return baseType.IsAssignableToGenericType(genericType);
    }
    public static object GetEnumValue(Type propertyType)
    {
        Random rnd = new Random();
        var lst = Enum.GetValues(propertyType);
        int arrnum = rnd.Next(lst.Length);
        return lst.GetValue(arrnum);
    }
}