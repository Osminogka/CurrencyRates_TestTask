using ClosedXML.Excel;
using CurrencyRates.Models;

namespace CurrencyRates.Services
{
    public class CurrencyExportService
    {
        public byte[] ExportToExcel(IEnumerable<CurrencyRate> rates)
        {
            using var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Currency Rates");

            // Заголовки
            worksheet.Cell(1, 1).Value = "Code";
            worksheet.Cell(1, 2).Value = "Name";
            worksheet.Cell(1, 3).Value = "Rate";
            worksheet.Cell(1, 4).Value = "Date";

            int row = 2;
            foreach (var rate in rates)
            {
                worksheet.Cell(row, 1).Value = rate.Code;
                worksheet.Cell(row, 2).Value = rate.Name;
                worksheet.Cell(row, 3).Value = rate.Rate;
                worksheet.Cell(row, 4).Value = rate.Date.ToString("yyyy-MM-dd");
                row++;
            }

            worksheet.Columns().AdjustToContents();

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            return stream.ToArray();
        }
    }
}
