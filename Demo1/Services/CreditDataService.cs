using Demo1.Models;

namespace Demo1.Services
{
    internal class CreditDataService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private const string dataSourceUrl = "https://raw.githubusercontent.com/StrategicFS/Recruitment/master/creditData.json";

        public CreditDataService(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
            CreditData = new Lazy<IList<CreditDatum>>(FetchCreditData, LazyThreadSafetyMode.PublicationOnly);
        }

        private IList<CreditDatum> FetchCreditData()
        {
            using var client = _httpClientFactory.CreateClient();

            // N.B.  I know this is not ideal, but I'm trying to prevent the Lazy property from being "infected" by async.
            var data = client.GetFromJsonAsync<ResponseContainer>(dataSourceUrl).Result;

            return data?.CreditReports ?? new List<CreditDatum>();
        }

        public Lazy<IList<CreditDatum>> CreditData { get; }

        private class ResponseContainer
        {
            public IList<CreditDatum> CreditReports { get; set; } = new List<CreditDatum>();
        }
    }
}
