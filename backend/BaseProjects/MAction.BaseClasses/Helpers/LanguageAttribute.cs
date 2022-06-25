using MAction.BaseClasses.Exceptions;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace MAction.BaseClasses.Helpers;

public enum LanguageEnum
{
    [DefaultLanguage]
    [EnumMember(Value = "En")]
    En = 1,
    [EnumMember(Value = "Fa")]
    Fa = 2,
    [EnumMember(Value = "Ar")]
    Ar = 3,
    [EnumMember(Value = "Fr")]
    Fr = 4
}

public class DefaultLanguage : Attribute
{
}

public class Translation
{
    public Translation()
    {
        translate = new();
    }
    private Dictionary<string, string> translate;

    public Dictionary<string, string> Translate
    {
        get
        {
            return translate;
        }
        set
        {
            foreach(var key in value.Keys)
            {
                if (!Enum.TryParse<LanguageEnum>(key, out _))
                    throw new BadRequestException(key + " is not valid language");
            }
            translate = value;
        }
    }
}

//TO DO Should be create middleware for not change Date in output and input
//private static void DisableChangeCalender()
//{
//    var clone = Thread.CurrentThread.CurrentCulture.Clone() as CultureInfo;
//    clone.DateTimeFormat = CultureInfo.GetCultureInfo("en").DateTimeFormat;

//    Thread.CurrentThread.CurrentCulture = clone;
//    Thread.CurrentThread.CurrentUICulture = clone;
//}