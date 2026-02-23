using FinBookeAPI.Collections.AmountCollection;
using FinBookeAPI.Models.AmountManagement;
using FinBookeAPI.Services.AmountManagement;
using FinBookeAPI.Services.SecurityUtility;
using FinBookeAPI.Tests.Mocks.Collections;
using FinBookeAPI.Tests.Mocks.Services;
using FinBookeAPI.Tests.Records;
using Moq;

namespace FinBookeAPI.Tests.AmountManagement
{
    public partial class AmountManagementUnitTests
    {
        private readonly AmountManagementService _service;
        private readonly Mock<IAmountCollection> _collection;
        private readonly Mock<ISecurityUtilityService> _security;
        private readonly List<Amount> _database;
        private readonly Amount _amount;

        public AmountManagementUnitTests()
        {
            _amount = AmountRecord.GetTestAmount();
            _database = AmountRecord.GetTestAmounts();
            var logger = new Mock<ILogger<AmountManagementService>>();
            _security = MockISecurityUtilityService.GetMock();
            _collection = MockAmountCollection.GetMock(_database);

            _service = new AmountManagementService(
                _collection.Object,
                _security.Object,
                logger.Object
            );
        }
    }
}
