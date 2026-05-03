namespace FinBookeAPI.AppConfig.Redaction;

public static class Redaction
{
    public static IServiceCollection AddRedactionConfig(this IServiceCollection collection)
    {
        collection.AddRedaction(redactionBuilder =>
        {
            redactionBuilder.SetRedactor<StarRedactor>(
                RedactionClassification.Private,
                RedactionClassification.Personal
            );
        });
        return collection;
    }
}
