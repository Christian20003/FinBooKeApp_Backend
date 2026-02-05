namespace FinBookeAPI.DTO.Category.Input;

/// <summary>
/// This record represents the message of a category update request.
/// </summary>
public record UpdateCategoryDTO : CategoryBaseDTO
{
    public new SetLimitDTO? Limit => base.Limit as SetLimitDTO;

    public UpdateCategoryDTO(SetLimitDTO? limit)
    {
        base.Limit = limit;
    }
}
