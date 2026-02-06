using System.ComponentModel.DataAnnotations;
using FinBookeAPI.Models.Payment;

namespace FinBookeAPI.DTO.Payment;

/// <summary>
/// This record represents a message to post a payment method.
/// </summary>
public record PostPaymentMethodDTO
{
    [Required(ErrorMessage = "Payment method type is missing")]
    public string Type { get; set; } = string.Empty;

    public List<PostPaymentInstanceDTO> Instances { get; set; } = [];

    public PaymentMethod GetPaymentMethod(Guid userId)
    {
        return new PaymentMethod
        {
            UserId = userId,
            Type = Type,
            Instances = [.. Instances.Select(instance => instance.GetPaymentInstance())],
        };
    }

    public override string ToString()
    {
        return $"{{ Type: {Type}, Instances: {string.Join(", ", Instances)} }}";
    }
}
