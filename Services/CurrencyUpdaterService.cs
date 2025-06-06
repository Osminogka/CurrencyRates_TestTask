namespace CurrencyRates.Services
{
    public class CurrencyUpdaterService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<CurrencyUpdaterService> _logger;
        private readonly TimeSpan _updateInterval = TimeSpan.FromMinutes(5);

        public static List<Models.CurrencyRate> CachedRates { get; private set; } = new();

        public CurrencyUpdaterService(IServiceProvider serviceProvider, ILogger<CurrencyUpdaterService> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("CurrencyUpdaterService started.");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using var scope = _serviceProvider.CreateScope();
                    var currencyService = scope.ServiceProvider.GetRequiredService<CurrencyService>();

                    var rates = await currencyService.GetCurrencyRatesAsync();
                    CachedRates = rates;

                    _logger.LogInformation($"Currency rates updated at {DateTime.Now}.");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error updating currency rates.");
                }

                await Task.Delay(_updateInterval, stoppingToken);
            }

            _logger.LogInformation("CurrencyUpdaterService stopped.");
        }
    }
}
