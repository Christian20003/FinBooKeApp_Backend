using FinBookeAPI.Models.Configuration;
using FinBookeAPI.Models.Exceptions;

namespace FinBookeAPI.Services.SecurityUtility;

public partial class SecurityUtilityService : ISecurityUtilityService
{
    public async Task<Guid> GenerateUniqueId(Guid id, Func<Guid, Task<bool>> exist)
    {
        LogVerifyId(id);
        var result = id;
        for (int i = 0; i < _idGenerationTrials; i++)
        {
            if (!await exist(result))
                return result;
            result = Guid.CreateVersion7();
        }
        LogVerifyIdFailed(id);
        throw new IdGenerationException("Could not generate a unique id");
    }

    [LoggerMessage(
        EventId = LogEvents.SecurityGenerateId,
        Level = LogLevel.Information,
        Message = "Security: Verify if id is unique - Id: {Id}"
    )]
    private partial void LogVerifyId(Guid id);

    [LoggerMessage(
        EventId = LogEvents.SecurityFailedIdGeneration,
        Level = LogLevel.Error,
        Message = "Security: Failed id generation - Id: {Id}"
    )]
    private partial void LogVerifyIdFailed(Guid id);
}
