using System.ComponentModel.DataAnnotations;
using FinBookeAPI.Attributes;

namespace FinBookeAPI.Models.Payment;

public class PaymentInstance
{
    [NonEmptyGuid(ErrorMessage = "Payment instance id is invalid")]
    public Guid Id { get; set; } = Guid.CreateVersion7();

    [StringLength(
        100,
        MinimumLength = 3,
        ErrorMessage = "Payment instance name must be between {2} and {1} characters long"
    )]
    public string Name { get; set; } = string.Empty;

    [MaxLength(
        1000,
        ErrorMessage = "Payment instance description can only have up to {1} characters"
    )]
    public string? Description { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime ModifiedAt { get; set; } = DateTime.UtcNow;

    public PaymentInstance Copy()
    {
        return new PaymentInstance
        {
            Id = Id,
            Name = Name,
            Description = Description,
            CreatedAt = CreatedAt,
            ModifiedAt = ModifiedAt,
        };
    }

    public override string ToString()
    {
        return $"PaymentInstance: {{ Id: {Id}, Name: {Name}, Description: {Description}, CreatedAt: {CreatedAt}, ModifiedAt: {ModifiedAt} }}";
    }
}
