using System.ComponentModel.DataAnnotations;
using FinBookeAPI.Attributes;

namespace FinBookeAPI.Models.AmountManagement;

/// <summary>
/// This model represents a single amount value that should be
/// stored in the database.
/// </summary>
public class Amount
{
    [NonEmptyGuid(ErrorMessage = "Amount id is not valid")]
    public Guid Id { get; set; } = Guid.NewGuid();

    [NonEmptyGuid(ErrorMessage = "User id is not valid")]
    public Guid UserId { get; set; } = Guid.NewGuid();

    [NonEmptyGuid(ErrorMessage = "Payment type id is not valid")]
    public Guid PaymentTypeId { get; set; } = Guid.NewGuid();

    [NonEmptyGuid(ErrorMessage = "Category id is not valid")]
    public Guid CategoryId { get; set; } = Guid.NewGuid();

    [Range(0, double.MaxValue, ErrorMessage = "Amount value must be between {1} and {2}")]
    public decimal Value { get; set; }

    public AmountType Type { get; set; }

    [MaxLength(1000, ErrorMessage = "Amount comment can only have at most {1} characters")]
    public string Comment { get; set; } = "";

    public string ReceiptFile { get; set; } = "";

    public string BankStatementFile { get; set; } = "";

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime ModifiedAt { get; set; } = DateTime.UtcNow;

    public Amount Copy()
    {
        return new Amount
        {
            Id = Id,
            UserId = UserId,
            PaymentTypeId = PaymentTypeId,
            CategoryId = CategoryId,
            Value = Value,
            Type = Type,
            Comment = Comment,
            ReceiptFile = ReceiptFile,
            BankStatementFile = BankStatementFile,
            CreatedAt = CreatedAt,
            ModifiedAt = ModifiedAt,
        };
    }
}
