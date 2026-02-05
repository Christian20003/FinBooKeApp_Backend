using Microsoft.AspNetCore.Mvc;

namespace FinBookeAPI.Controllers.Helper;

public static class UrlGenerator
{
    /// <summary>
    /// This method generates a URL to a specific resource.
    /// </summary>
    /// <param name="request">
    /// The incoming http request.
    /// </param>
    /// <param name="url">
    /// A helper object to generate URLs.
    /// </param>
    /// <param name="endpoint">
    /// The name of the endpoint to get the resource.
    /// </param>
    /// <param name="values">
    /// Optional parameters that should be added to the final URL.
    /// </param>
    /// <returns>
    /// The complete URL to get a defined resource.
    /// </returns>
    public static string GenerateUrl(
        HttpRequest request,
        IUrlHelper url,
        string endpoint,
        object? values
    )
    {
        var path = url.Action(endpoint, values);
        System.Console.WriteLine(endpoint);
        return $"{request.Scheme}://{request.Host}{path}";
    }
}
