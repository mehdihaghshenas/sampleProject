using MAction.BaseClasses.Helpers;
using System;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;

namespace MAction.BaseClasses.Language;

public static class LanguageHelpers
{
    public static void SetLanguagePropertyInfo<T>(T entity) where T : IBaseEntity
    {
        var languagePropertyInfo =
            typeof(T).GetProperties().FirstOrDefault(x => x.PropertyType == typeof(LanguageEnum));

        if (languagePropertyInfo == null) return;

        var value = (LanguageEnum)(languagePropertyInfo.GetValue(entity) ?? throw new InvalidOperationException());
        if ((int)value == 0)
            languagePropertyInfo.SetValue(entity, Enum.Parse<LanguageEnum>(
                CultureInfo.CurrentCulture.Name, ignoreCase: true));
    }

    public static Expression<Func<T, bool>> GetLanguageExpressionCondition<T>() where T : IBaseEntity
    {
        Expression<Func<T, bool>> langCondition = null;
        var languagePropertyInfo =
            typeof(T).GetProperties().FirstOrDefault(x => x.PropertyType == typeof(LanguageEnum));
        if (languagePropertyInfo != null)
        {
            langCondition = ExpressionHelpers.GetConstantExpressionFromType<T>(languagePropertyInfo,
                Enum.Parse<LanguageEnum>(
                    CultureInfo.CurrentCulture.Name, ignoreCase: true));

        }

        return langCondition;
    }

    //TO DO Should be create middleware for not change Date in output and input
    //private static void DisableChangeCalender()
    //{
    //    var clone = Thread.CurrentThread.CurrentCulture.Clone() as CultureInfo;
    //    clone.DateTimeFormat = CultureInfo.GetCultureInfo("en").DateTimeFormat;

    //    Thread.CurrentThread.CurrentCulture = clone;
    //    Thread.CurrentThread.CurrentUICulture = clone;
    //}
}

