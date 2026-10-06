using FinBooKeApp.Api.Configuration.Redaction;
using Microsoft.Extensions.Compliance.Classification;

namespace FinBooKeApp.Api.Shared.Attributes;

public sealed class PrivateAttribute : DataClassificationAttribute
{
    public PrivateAttribute()
        : base(RedactionClassification.Private) { }
}
