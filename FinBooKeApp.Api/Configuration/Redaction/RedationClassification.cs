using Microsoft.Extensions.Compliance.Classification;

namespace FinBooKeApp.Api.Configuration.Redaction;

public static class RedactionClassification
{
    private static readonly string _name = "RedactionClasses";

    public static DataClassification Private => new(_name, nameof(Private));
    public static DataClassification Personal => new(_name, nameof(Personal));
}
