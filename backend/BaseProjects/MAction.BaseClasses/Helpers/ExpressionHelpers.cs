using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MAction.BaseClasses.Helpers
{
    public static class ExpressionHelpers
    {
        public static  Expression<Func<T, bool>> GetIdFilter<T>(object keyValue) where T : IBaseEntity
        {
            var keyName = ((T)Activator.CreateInstance(typeof(T))!).GetPrimaryKeyPropertyName();

            ParameterExpression argParam = Expression.Parameter(typeof(T), "x");
            Expression nameProperty = Expression.Property(argParam, keyName);

            Expression exp = Expression.Equal(nameProperty, Expression.Constant(keyValue));
            var lambda = Expression.Lambda<Func<T, bool>>(exp, argParam);
            return lambda;
        }

        public static Expression<Func<T, bool>> GetConstantExpressionFromType<T>(PropertyInfo property, object value)
        {
            var keyName = property.Name;
            ParameterExpression argParam = Expression.Parameter(typeof(T), "x");
            Expression nameProperty = Expression.Property(argParam, keyName);

            Expression exp = Expression.Equal(nameProperty, Expression.Constant(value));
            var lambda = Expression.Lambda<Func<T, bool>>(exp, argParam);
            return lambda;
        }

    }
}
