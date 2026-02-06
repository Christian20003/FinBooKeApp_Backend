using FinBookeAPI.Models.Payment;

namespace FinBookeAPI.DTO.Payment;

/// <summary>
/// This record represents a returning message with a payment method.
/// </summary>
public record GetPaymentMethodDTO
{
    public Guid Id { get; set; }

    public string Type { get; set; } = string.Empty;

    public List<GetPaymentInstanceDTO> Instances { get; set; } = [];

    public string? Url { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; }

    public DateTime ModifiedAt { get; set; }

    public GetPaymentMethodDTO(
        PaymentMethod method,
        IEnumerable<GetPaymentInstanceDTO> instances,
        string? url
    )
    {
        Id = method.Id;
        Type = method.Type;
        Instances = [.. instances];
        Url = url;
        CreatedAt = method.CreatedAt;
        ModifiedAt = method.ModifiedAt;
    }
}
