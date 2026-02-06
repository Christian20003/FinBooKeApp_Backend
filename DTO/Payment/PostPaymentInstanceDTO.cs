using System.ComponentModel.DataAnnotations;
using FinBookeAPI.Models.Payment;

namespace FinBookeAPI.DTO.Payment;

/// <summary>
/// This record represents a message to post a payment instance.
/// </summary>
public record PostPaymentInstanceDTO
{
    [Required(ErrorMessage = "Payment instance name is missing")]
    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; } = null;

    public PaymentInstance GetPaymentInstance()
    {
        return new PaymentInstance { Name = Name, Description = Description };
    }

    public override string ToString()
    {
        return $"{{ Name: {Name}, Description: {Description} }}";
    }
}
