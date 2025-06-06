using CurrencyRates.Models;

namespace CurrencyRates.Services
{
    public class CurrencyCacheService
    {
        public List<CurrencyRate> Rates => CurrencyUpdaterService.CachedRates;

        public Task<List<CurrencyRate>> GetLatestRatesAsync()
        {
            return Task.FromResult(Rates.ToList());
        }
    }

}
