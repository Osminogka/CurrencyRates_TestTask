namespace CurrencyRates.Models
{
    public class CurrencyRate
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public decimal Rate { get; set; }
        public DateTime Date { get; set; }
    }

}
