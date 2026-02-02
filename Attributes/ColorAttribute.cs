using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace FinBookeAPI.Attributes;

/// <summary>
/// This class represents a new data annotation for class properties, fields and parameters that
/// should be a valid color encoding. It should prevent that an unsupported color encoding is provided.
/// The following encodings are supported:
/// <list type="bullet">
///     <item>
///         <b>Hexadecimal color coding:</b> Each value (red, green and blue) is encoded with a hexadecimal value.
///         For example - <c>#fa23a6</c>
///     </item>
///     <item>
///         <b>RGB color coding:</b> Each value (red, green and blue) is encoded with an integer value (max. 255).
///         For example - <c>rgb(43, 56, 255)</c>
///     </item>
///     <item>
///         <b>CMYK color coding:</b> Each value (cyan, magenta, yellow, black) is encoded with a percentage value.
///         For example - <c>cmyk(15, 55, 98, 30)</c>
///     </item>
///     <item>
///         <b>HSL color coding:</b> Expresses a color with hue, saturation and lightness.
///         For example - <c>hsl(250, 28%, 8%)</c>
///     </item>
/// </list>
/// </summary>
[AttributeUsage(
    AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter,
    AllowMultiple = false
)]
public class ColorAttribute : ValidationAttribute
{
    private readonly string _hexColorCode = @"^#[0-9a-fA-F]{6}$";
    private readonly string _rgbColorCode =
        @"^rgb\((25[0-5]|2[0-4][0-9]|[1]?[0-9]{1,2}),(25[0-5]|2[0-4][0-9]|[1]?[0-9]{1,2}),(25[0-5]|2[0-4][0-9]|[1]?[0-9]{1,2})\)$";
    private readonly string _cmykColorCode =
        @"^cmyk\((100|\d{1,2}),(100|\d{1,2}),(100|\d{1,2}),(100|\d{1,2})\)$";
    private readonly string _hslColorCode =
        @"^hsl\((360|3[0-5][0-9]|[1-2]?[0-9]{1,2}),(100|\d{1,2})%,(100|\d{1,2})%\)$";

    /// <summary>
    /// This function proofs if the provided color is a supported encoding.
    /// </summary>
    /// <param name="value">The object that should be analyzed.</param>
    /// <returns>
    /// True, if the object is a valid color, otherwise false.
    /// If the value itself is <c>null</c> this function returns <c>true</c>.
    /// </returns>
    public override bool IsValid(object? value)
    {
        if (value is null)
        {
            return false;
        }
        string data = Regex.Replace(value.ToString() ?? string.Empty, @"\s+", string.Empty);
        return Regex.IsMatch(data, _hexColorCode)
            || Regex.IsMatch(data, _rgbColorCode)
            || Regex.IsMatch(data, _cmykColorCode)
            || Regex.IsMatch(data, _hslColorCode);
    }
}
