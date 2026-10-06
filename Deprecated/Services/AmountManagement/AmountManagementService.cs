using FinBookeAPI.Collections.AmountCollection;
using FinBookeAPI.Services.CategoryType;

namespace FinBookeAPI.Services.AmountManagement;

public partial class AmountManagementService(
    IAmountCollection collection,
    ICategoryService category,
    ILogger<AmountManagementService> logger
) : IAmountManagementService
{
    private readonly IAmountCollection _collection = collection;

    private readonly ICategoryService _category = category;

    private readonly ILogger<AmountManagementService> _logger = logger;
}
