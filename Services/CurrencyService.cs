using System.Globalization;
using System.Xml.Linq;
using CurrencyRates.Models;
using Microsoft.Extensions.Options;

namespace CurrencyRates.Services
{
    public class CurrencyService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<CurrencyService> _logger;
        private readonly string _primaryUrl;

        public CurrencyService(ILogger<CurrencyService> logger, IOptions<CurrencyRatesOptions> options)
        {
            _httpClient = new HttpClient();
            _logger = logger;
            _primaryUrl = options.Value.PrimaryUrl;
        }

        public async Task<List<CurrencyRate>> GetCurrencyRatesAsync()
        {
            var result = new List<CurrencyRate>();

            try
            {
                var response = await _httpClient.GetStringAsync(_primaryUrl);

                var document = XDocument.Parse(response);

                var cubeRoot = document.Descendants().FirstOrDefault(e => e.Name.LocalName == "Cube" && e.HasAttributes == false);
                var dateCube = cubeRoot?.Elements().FirstOrDefault();
                var date = DateTime.Now;

                if (dateCube?.Attribute("time") != null)
                {
                    DateTime.TryParse(dateCube.Attribute("time")?.Value, out date);
                }

                foreach (var currency in dateCube.Elements())
                {
                    result.Add(new CurrencyRate
                    {
                        Code = currency.Attribute("currency")?.Value ?? "N/A",
                        Name = GetCurrencyName(currency.Attribute("currency")?.Value ?? "N/A"),
                        Rate = decimal.Parse(currency.Attribute("rate")?.Value ?? "0", CultureInfo.InvariantCulture),
                        Date = date
                    });
                }
            }
            catch(Exception ex)
            {
                _logger.LogError(ex.Message);
            }

            return result;
        }

        private string GetCurrencyName(string code)
        {
            try
            {
                var culture = CultureInfo
                    .GetCultures(CultureTypes.SpecificCultures)
                    .Select(c => new RegionInfo(c.LCID))
                    .FirstOrDefault(r => r.ISOCurrencySymbol == code);

                return culture?.CurrencyEnglishName ?? code;
            }
            catch
            {
                return code;
            }
        }
    }
}
