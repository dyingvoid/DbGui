using System;
using System.Collections.Generic;
using System.Globalization;

namespace DbGui.Models;

public static class Extensions
{
    public static TNewCollection PureForEach<TCollection, TValue, TNewCollection, TNewValue>
        (this TCollection collection, Func<TValue, TNewValue> func)
    where TCollection : ICollection<TValue>, new()
    where TNewCollection : ICollection<TNewValue>, new()
    {
        if (collection == null)
            throw new NullReferenceException();
        if (func == null) 
            throw new NullReferenceException();

        var newCollection = new TNewCollection();
        foreach (var value in collection)
        {
            var newValue = func(value);
            newCollection.Add(newValue);
        }

        return newCollection;
    }

    // Three ToType methods are used in runtime by reflection in CsvTable
    public static T? ToTypeWithStructConstraint<T>(this string? item) where T : struct, IParsable<T>
    {
        if (item == null)
        {
            return null;
        }
        return T.Parse(item, CultureInfo.InvariantCulture);
    }

    public static TEnum? ToTypeEnumConstraint<TEnum>(this string? item) where TEnum : struct
    {
        if (item == null)
        {
            return null;
        }

        return (TEnum)Enum.Parse(typeof(TEnum), item);
    } 

    public static T? ToTypeWithClassConstraint<T>(this string? item) where T : class, IParsable<T>
    {
        if (item == null)
        {
            return null;
        }

        return T.Parse(item, CultureInfo.InvariantCulture);
    }

    public static bool IsEmptyOrWhiteSpace(this string? value)
    {
        return value is "" or " ";
    }
}