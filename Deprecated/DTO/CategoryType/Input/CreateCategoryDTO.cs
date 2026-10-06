using System.ComponentModel.DataAnnotations;

namespace FinBookeAPI.DTO.CategoryType.Input;

/// <summary>
/// This record respresents the message of a category create request.
/// </summary>
public record CreateCategoryDTO : CategoryBaseDTO
{
    [Required(ErrorMessage = "Category name is missing")]
    public new string Name => base.Name!;

    [Required(ErrorMessage = "Category color is missing")]
    public new string Color => base.Color!;

    public new SetLimitDTO? Limit => base.Limit as SetLimitDTO;

    public CreateCategoryDTO(string name, string color, SetLimitDTO? limit)
    {
        base.Name = name;
        base.Color = color;
        base.Limit = limit;
    }
}
