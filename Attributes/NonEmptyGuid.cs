using System.ComponentModel.DataAnnotations;

namespace FinBookeAPI.Attributes;

/// <summary>
/// This class represents a new data annotation for class properties, fields and parameters that
/// should be non empty GUIDs. It should prevent that an empty GUID is provided. It is implemented
/// for primitive types and all classes that implements the <c>IEnumerable</c> interface.
/// </summary>
[AttributeUsage(
    AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter,
    AllowMultiple = false
)]
public class NonEmptyGuid : ValidationAttribute
{
    /// <summary>
    /// This method validates if the provided object is an empty GUID or an
    /// enumerable with empty GUIDs.
    /// </summary>
    /// <param name="value">
    /// The value that should be validated.
    /// </param>
    /// <returns>
    /// True if the object does not contain any empty GUID, otherwise false.
    /// If the object is <c>null</c> or a different type, this method will
    /// return true.
    /// </returns>
    public override bool IsValid(object? value)
    {
        if (value is null)
            return true;
        if (value is IEnumerable<Guid> list)
        {
            foreach (var elem in list)
            {
                if (elem == Guid.Empty)
                    return false;
            }
        }
        else if (value is Guid id)
        {
            return id != Guid.Empty;
        }
        return true;
    }
}
