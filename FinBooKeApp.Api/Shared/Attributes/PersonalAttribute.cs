using FinBooKeApp.Api.Configuration.Redaction;
using Microsoft.Extensions.Compliance.Classification;

namespace FinBooKeApp.Api.Shared.Attributes;

public sealed class PersonalAttribute : DataClassificationAttribute
{
    public PersonalAttribute()
        : base(RedactionClassification.Personal) { }
}
