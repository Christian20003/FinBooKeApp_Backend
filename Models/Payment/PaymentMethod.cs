using System.ComponentModel.DataAnnotations;
using FinBookeAPI.Attributes;

namespace FinBookeAPI.Models.Payment;

public class PaymentMethod
{
    [NonEmptyGuid(ErrorMessage = "Payment method id is not valid")]
    public Guid Id { get; set; } = Guid.NewGuid();

    [NonEmptyGuid(ErrorMessage = "Payment method user id is not valid")]
    public Guid UserId { get; set; } = Guid.NewGuid();

    [StringLength(
        100,
        MinimumLength = 3,
        ErrorMessage = "Payment method type must be between {2} and {1} characters long"
    )]
    public string Type { get; set; } = string.Empty;

    public List<PaymentInstance> Instances { get; set; } = [];

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime ModifiedAt { get; set; } = DateTime.UtcNow;

    public PaymentMethod() { }

    public PaymentMethod Copy()
    {
        return new PaymentMethod
        {
            Id = Id,
            UserId = UserId,
            Type = Type,
            Instances = [.. Instances.Select(instance => instance.Copy())],
            CreatedAt = CreatedAt,
            ModifiedAt = ModifiedAt,
        };
    }

    public override string ToString()
    {
        return $"PaymentMethod: {{ Id: {Id}, UserId: {UserId}, Type: {Type}, Instances: [{string.Join(", ", Instances)}], CreatedAt: {CreatedAt}, ModifiedAt: {ModifiedAt} }}";
    }
}
