using System.Collections;
using System.Reflection;
using System.Xml;
using System.Xml.Linq;

namespace FinBooKeAPI.Logic.Parsing;

public class XmlParser : IParser
{
    public IEnumerable<TYPE> Parse<TYPE>(string content)
        where TYPE : new()
    {
        XDocument doc = XDocument.Parse(content);
        if (doc.Root is null)
            throw new XmlException("Invalid xml schema");
        var typename = typeof(TYPE).Name;
        var xml = doc.Descendants(typename);
        return xml.Select(SetObject<TYPE>);
    }

    private static OBJECT_TYPE SetObject<OBJECT_TYPE>(XElement xml)
        where OBJECT_TYPE : new()
    {
        var result = new OBJECT_TYPE();
        var type = typeof(OBJECT_TYPE);
        var properties = type.GetProperties();
        foreach (var property in properties)
        {
            var xmlPropertyList = xml.Descendants(property.Name);
            if (!xmlPropertyList.Any())
                continue;
            if (xmlPropertyList.Count() > 1)
                throw new XmlException("A property name should have only a single element");

            var xmlProperty = xmlPropertyList.First();
            if (!xmlProperty.HasElements)
            {
                SetProperty(xmlProperty.Value, property, result);
                continue;
            }
            var elements = xmlProperty.Elements();
            SetProperty(elements.Select(element => element.Value), property, result);
        }
        return result;
    }

    private static void SetProperty<OBJECT_TYPE>(
        IEnumerable<string> values,
        PropertyInfo property,
        OBJECT_TYPE obj
    )
    {
        var type = property.PropertyType;
        if (!type.IsGenericType || type.GetGenericTypeDefinition() != typeof(List<>))
            throw new XmlException($"IEnumerable type {type} is not supported");
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
