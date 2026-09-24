using System.ComponentModel.DataAnnotations;

namespace FinBookeAPI.Attributes;

public class EnumRangeAttribute<T> : ValidationAttribute
    where T : struct, Enum
{
    private readonly long _min;
    private readonly long _max;

    public EnumRangeAttribute()
    {
        var values = Enum.GetValues<T>().Select(v => Convert.ToInt64(v));
        _min = values.Min();
        _max = values.Max();
    }

    public override bool IsValid(object? value)
    {
        if (value == null)
            return true;

        if (value is not T enumValue)
            return false;

        var longValue = Convert.ToInt64(enumValue);
        return longValue >= _min && longValue <= _max;
    }
}
