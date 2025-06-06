using System.Globalization;
using System.Xml.Linq;
using CurrencyRates.Models;

namespace CurrencyRates.Services
{
    public class CurrencyService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<CurrencyService> _logger;

        public CurrencyService(ILogger<CurrencyService> logger)
        {
            _httpClient = new HttpClient();
            _logger = logger;
        }

        public async Task<List<CurrencyRate>> GetCurrencyRatesAsync()
        {
            var result = new List<CurrencyRate>();

            try
            {
                string url = "https://www.ecb.europa.eu/stats/eurofxref/eurofxref-daily.xml";
                var response = await _httpClient.GetStringAsync(url);

                var document = XDocument.Parse(response);
                var ns = XNamespace.Get("http://www.ecb.int/vocabulary/2002-08-01/eurofxref");

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
