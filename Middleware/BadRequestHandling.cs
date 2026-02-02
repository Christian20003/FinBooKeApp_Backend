using System.Net;
using FinBookeAPI.DTO.Error;
using Newtonsoft.Json;

namespace FinBookeAPI.Middleware;

/// <summary>
/// This middleware transforms .NET generated 'Bad request'
/// responses into a custom model.
/// </summary>
public class BadRequestHandling : IMiddleware
{
    private class InternalBadRequest
    {
        public string Type { get; set; } = "";
        public string Title { get; set; } = "";
        public int Status { get; set; }
        public Dictionary<string, List<string>> Errors { get; set; } = [];
        public string TraceId { get; set; } = "";
    };

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        // intercept stream
        var original = context.Response.Body;
        using var stream = new MemoryStream();
        context.Response.Body = stream;

        await next(context);

        // Proof if 400 response occur
        if (context.Response.StatusCode == (int)HttpStatusCode.BadRequest)
        {
            stream.Seek(0, SeekOrigin.Begin);
            using var reader = new StreamReader(stream);
            var content = await reader.ReadToEndAsync();
            var body = JsonConvert.DeserializeObject<InternalBadRequest>(content);

            if (body!.TraceId == string.Empty)
            {
                stream.Seek(0, SeekOrigin.Begin);
                await stream.CopyToAsync(original);
            }
            else
            {
                using var newStream = new MemoryStream();
                using var writer = new StreamWriter(newStream);
                // Read current content
                // Restructure the error message
                var newBody = new BadRequestDTO
                {
                    Type = "StructureException",
                    Title = "Faulty message structure",
                    Detail =
                        "The requested operation could not be executed due to invalid or missing properties",
                    Status = context.Response.StatusCode,
                    Instance =
                        context.Request.Scheme
                        + "://"
                        + context.Request.Host.Value
                        + context.Request.Path,
                    Properties = body!.Errors,
                };
                // Write it to the body
                await writer.WriteAsync(JsonConvert.SerializeObject(newBody));
                await writer.FlushAsync();
                context.Response.ContentLength = newStream.Length;
                newStream.Seek(0, SeekOrigin.Begin);
                await newStream.CopyToAsync(original);
            }
        }
        else
        {
            stream.Seek(0, SeekOrigin.Begin);
            await stream.CopyToAsync(original);
        }
    }
}
