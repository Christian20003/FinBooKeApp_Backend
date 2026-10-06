using Newtonsoft.Json;

namespace FinBookeAPI.DTO.Error;

/// <summary>
/// This class represents an error response.
/// </summary>
public class ErrorDTO
{
    /// <summary>
    /// The type of the error (e.g. a specific exception type)
    /// </summary>
    [JsonProperty(PropertyName = "type")]
    public string Type { get; set; } = "";

    /// <summary>
    /// The title of this error.
    /// </summary>
    [JsonProperty(PropertyName = "title")]
    public string Title { get; set; } = "";

    /// <summary>
    /// A detailed description of the error.
    /// </summary>
    [JsonProperty(PropertyName = "detail")]
    public string Detail { get; set; } = "";

    /// <summary>
    /// The http status code of the error.
    /// </summary>
    [JsonProperty(PropertyName = "status")]
    public int Status { get; set; }

    /// <summary>
    /// The URI path were this error occurred.
    /// </summary>
    [JsonProperty(PropertyName = "instance")]
    public string Instance { get; set; } = "";

    /// <summary>
    /// The date where this error was created
    /// </summary>
    [JsonProperty(PropertyName = "createdAt")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
