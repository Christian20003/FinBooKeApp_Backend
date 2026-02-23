using FinBookeAPI.Collections.AmountCollection;
using FinBookeAPI.Services.SecurityUtility;

namespace FinBookeAPI.Services.AmountManagement;

public partial class AmountManagementService(
    IAmountCollection collection,
    ISecurityUtilityService security,
    ILogger<AmountManagementService> logger
) : IAmountManagementService
{
    private readonly IAmountCollection _collection = collection;
    private readonly ISecurityUtilityService _security = security;
    private readonly ILogger<AmountManagementService> _logger = logger;
}
