using Newtonsoft.Json;

namespace FinBookeAPI.DTO.Error;

/// <summary>
/// This class represent a bad request response.
/// </summary>
public class BadRequestDTO : ErrorDTO
{
    /// <summary>
    /// A list of invalid or missing properties.
    /// </summary>
    [JsonProperty(PropertyName = "properties")]
    public Dictionary<string, List<string>> Properties { get; set; } = [];
}
