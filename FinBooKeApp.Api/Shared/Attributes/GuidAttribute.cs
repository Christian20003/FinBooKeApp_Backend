using System.Collections;
using System.ComponentModel.DataAnnotations;

namespace FinBooKeApp.Api.Shared.Attributes;

[AttributeUsage(
    AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter,
    AllowMultiple = false
)]
public class GuidAttribute : ValidationAttribute
{
    public override bool IsValid(object? value)
    {
        if (value is null)
            return true;
        // Check if object is iterable (Check each item)
        if (value is IEnumerable list && list is not string)
        {
            foreach (var item in list)
            {
                if (!IsValid(item))
                    return false;
            }
            return true;
        }
        // Check primitive element
        return Guid.TryParse(value.ToString(), out var id) && id != Guid.Empty;
    }
}
