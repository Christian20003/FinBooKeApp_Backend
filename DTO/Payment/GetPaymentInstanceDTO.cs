using FinBookeAPI.Models.Payment;

namespace FinBookeAPI.DTO.Payment;

/// <summary>
/// This record represents a returning message with a payment instance.
/// </summary>
public record GetPaymentInstanceDTO
{
    public Guid Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }

    public string? Url { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime ModifiedAt { get; set; }

    public GetPaymentInstanceDTO(PaymentInstance instance, string? url)
    {
        Id = instance.Id;
        Name = instance.Name;
        Description = instance.Description;
        Url = url;
        CreatedAt = instance.CreatedAt;
        ModifiedAt = instance.ModifiedAt;
    }
}
