using NReco.Logging.File;

namespace FinBookeAPI.AppConfig.Documentation;

public static class Logging
{
    public static IServiceCollection AddLoggingConfig(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        services.AddLogging(loggingBuilder =>
        {
            var loggingSettings = configuration.GetSection("Logging");

            loggingBuilder.ClearProviders();
            loggingBuilder.EnableRedaction();

            loggingBuilder.AddFile(
                loggingSettings,
                fileLoggerOptions =>
                {
                    fileLoggerOptions.FormatLogEntry = (msg) =>
                    {
                        var builder = new System.Text.StringBuilder();
                        var writer = new StringWriter(builder);
                        var json = new Newtonsoft.Json.JsonTextWriter(writer);
                        json.WriteStartObject();
                        json.WritePropertyName("log");
                        json.WriteStartObject();
                        json.WritePropertyName("time");
                        json.WriteValue(DateTime.Now.ToString("O"));
                        json.WritePropertyName("level");
                        json.WriteValue(msg.LogLevel.ToString());
                        json.WritePropertyName("name");
                        json.WriteValue(msg.LogName);
                        json.WritePropertyName("event");
                        json.WriteValue(msg.EventId.Id);
                        json.WritePropertyName("message");
                        json.WriteValue(msg.Message);
                        json.WritePropertyName("exception");
                        json.WriteValue(msg.Exception?.ToString());
                        json.WriteEndObject();
                        json.WriteEndObject();
                        return builder.ToString();
                    };
                }
            );

            loggingBuilder.AddConsole();
        });
        return services;
    }
}
