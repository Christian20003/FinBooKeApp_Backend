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

    public IEnumerable<TYPE> Parse<TYPE>(string content)
        where TYPE : new()
    {
        if (content == string.Empty)
            throw new CsvException("Invalid csv format");
        var lines = content.Split(LINE_DELIMITER);
        var header = lines.First();
        var values = lines.Skip(1);

        return [.. values.Select(line => SetObject<TYPE>(line, header))];
    }

    private static OBJECT_TYPE SetObject<OBJECT_TYPE>(string line, string header)
        where OBJECT_TYPE : new()
    {
        var result = new OBJECT_TYPE();
        var type = typeof(OBJECT_TYPE);
        var properties = type.GetProperties();
        var csvProperties = GetCsvProperties(line, header);
        for (var index = 0; index < properties.Length; index++)
        {
            var property = properties.ElementAt(index);
            if (!csvProperties.TryGetValue(property.Name, out var csvProperty))
                continue;
            if (IsListType(property))
                SetProperty(csvProperty.Split(LIST_DELIMITER), property, result);
            else
                SetProperty(csvProperty, property, result);
        }
        return result;
    }

    private static Dictionary<string, string> GetCsvProperties(string line, string header)
    {
        using var parser = new TextFieldParser(new StringReader(line));
        parser.SetDelimiters($"{PROPERTY_DELIMITER}");
        parser.HasFieldsEnclosedInQuotes = true;

        var values = parser.ReadFields();
        var headers = header.Split(PROPERTY_DELIMITER);
        var result = new Dictionary<string, string>();
        if (values is null)
            return result;
        if (values.Length != headers.Length)
            throw new CsvException("Number of values unequal to number of headers");

        for (var index = 0; index < headers.Length; index++)
            result.Add(headers.ElementAt(index), values.ElementAt(index));

        return result;
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
