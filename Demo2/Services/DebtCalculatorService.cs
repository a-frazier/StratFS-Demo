using RemoteService;

namespace Demo2.Services
{
    public class DebtCalculatorService
    {
        private readonly ILogger<DebtCalculatorService> _logger;
        private readonly IDemo1 _demo1;

        private const string ABC = nameof(ABC);
        private const string EFX = nameof(EFX);
        private const string UNSECURED = nameof(UNSECURED);

        public DebtCalculatorService(ILogger<DebtCalculatorService> logger, IDemo1 demo1)
        {
            _logger = logger;
            _demo1 = demo1;
        }

        public async Task<DebtResponse?> CalculateDebtResponse(long applicationId, decimal annualIncome)
        {
            if (annualIncome == 0)
                throw new ArgumentOutOfRangeException(nameof(annualIncome), $"{nameof(annualIncome)} must not be zero.");

            var creditData = await _demo1.CreditDataAsync(applicationId, ABC, EFX);
            if (creditData.Count == 0)
                return null;

            var tradelines = creditData.SelectMany(d => d.Tradelines);
            var unsecuredTradelines = tradelines.Where(tl => string.Equals(tl.Type, UNSECURED, StringComparison.OrdinalIgnoreCase)).ToList();

            return new DebtResponse
            {
                UnsecuredTradelineCount = unsecuredTradelines.Count,
                UnsecuredDebtBalance = (decimal)unsecuredTradelines.Sum(tl => tl.Balance),
                DebtToIncomeRatio = (decimal)tradelines.Where(tl => !tl.IsMortgage).Sum(tl => tl.MonthlyPayment) / annualIncome / 12,
            };
        }
    }
}
