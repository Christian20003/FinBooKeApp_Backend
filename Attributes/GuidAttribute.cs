using System.ComponentModel.DataAnnotations;

namespace FinBookeAPI.Attributes;

/// <summary>
/// This class represents a new data annotation for class properties, fields and parameters that
/// should be valid GUIDs. It should prevent that an invalid GUID is provided. It is implemented
/// for primitive types and all classes that implements the <c>IEnumerable</c> interface.
/// </summary>
[AttributeUsage(
    AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter,
    AllowMultiple = false
)]
public class GuidAttribute : ValidationAttribute
{
    /// <summary>
    /// This function proofs if the provided object is a valid GUID or an
    /// <c>IEnumerable</c> structure that contains valid GUIDs.
    /// </summary>
    /// <param name="value">The object that should be analyzed.</param>
    /// <returns>
    /// True, if the object is a valid GUID or contains valid GUIDs, otherwise false.
    /// If the value itself is <c>null</c> this function returns <c>true</c>.
    /// </returns>
    public override bool IsValid(object? value)
    {
        if (value is null)
            return true;
        // Check if object is iterable (Check each item)
        if (value is IEnumerable<object> list)
        {
            foreach (var item in list)
            {
                if (!IsValid(item))
                    return false;
            }
            return true;
        }
        // Check primitive element
        return Guid.TryParse(value.ToString(), out _);
    }
}
