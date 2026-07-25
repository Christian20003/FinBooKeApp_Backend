using System.Collections;
using System.Reflection;
using FinBooKeAPI.Models.Exceptions;
using Microsoft.VisualBasic.FileIO;

namespace FinBooKeAPI.Logic.Parsing;

public class CsvParser : IParser
{
    const char LINE_DELIMITER = '\n';
    const char PROPERTY_DELIMITER = ',';
    const char LIST_DELIMITER = ';';
    const int HEADER = 1;

    public IEnumerable<TYPE> Parse<TYPE>(string content)
        where TYPE : new()
    {
        var lines = content.Split(LINE_DELIMITER).Skip(HEADER);
        return lines.Select(SetObject<TYPE>);
    }

    private static OBJECT_TYPE SetObject<OBJECT_TYPE>(string line)
        where OBJECT_TYPE : new()
    {
        var result = new OBJECT_TYPE();
        var type = typeof(OBJECT_TYPE);
        var properties = type.GetProperties();
        var csvProperties = GetCsvProperties(line);
        if (properties.Length != csvProperties.Count)
            throw new CsvException("Element has invalid number of properties");
        for (var index = 0; index < properties.Length; index++)
        {
            var property = properties.ElementAt(index);
            var csvProperty = csvProperties.ElementAt(index);
            if (IsListType(property))
                SetProperty(csvProperty.Split(PROPERTY_DELIMITER), property, result);
            else
                SetProperty(csvProperty, property, result);
        }
        return result;
    }

    private static List<string> GetCsvProperties(string line)
    {
        using var parser = new TextFieldParser(new StringReader(line));
        parser.SetDelimiters($"{LIST_DELIMITER}");
        parser.HasFieldsEnclosedInQuotes = true;
        var fields = parser.ReadFields();
        if (fields is null)
            return [];
        return [.. fields];
    }

    private static bool IsListType(PropertyInfo property)
    {
        return property.PropertyType.IsGenericType
            && property.PropertyType.GetGenericTypeDefinition() == typeof(List<>);
    }

    private static void SetProperty<OBJECT_TYPE>(
        IEnumerable<string> values,
        PropertyInfo property,
        OBJECT_TYPE obj
    )
    {
        var type = property.PropertyType;
        var subType = type.GetGenericArguments().First();
        var list = (IList)Activator.CreateInstance(typeof(List<>).MakeGenericType(subType))!;
        foreach (var value in values)
        {
            if (subType == typeof(Guid))
                list.Add(Guid.Parse(value));
            else
                list.Add(Convert.ChangeType(value, subType));
        }
        property.SetValue(obj, list);
    }

    private static void SetProperty<OBJECT_TYPE>(
        string value,
        PropertyInfo property,
        OBJECT_TYPE obj
    )
    {
        if (property.PropertyType == typeof(Guid))
            property.SetValue(obj, Guid.Parse(value));
        else
            property.SetValue(obj, Convert.ChangeType(value, property.PropertyType));
    }
}
