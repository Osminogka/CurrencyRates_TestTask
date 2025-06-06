using CurrencyRates.Models;
using CurrencyRates.Services;
using Radzen;

namespace CurrencyRates
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.Configure<CurrencyRatesOptions>(
                builder.Configuration.GetSection("CurrencyRates"));
            builder.Services.AddScoped<CurrencyService>();

            builder.Services.AddRazorPages();
            builder.Services.AddServerSideBlazor();

            builder.Services.AddScoped<DialogService>();
            builder.Services.AddScoped<NotificationService>();
            builder.Services.AddScoped<TooltipService>();
            builder.Services.AddScoped<ContextMenuService>();

            builder.Services.AddSingleton<CurrencyService>();
            builder.Services.AddSingleton<CurrencyExportService>();
            builder.Services.AddSingleton<CurrencyCacheService>();
            builder.Services.AddHostedService<CurrencyUpdaterService>();

            var app = builder.Build();

            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.MapBlazorHub();
            app.MapFallbackToPage("/_Host");

            app.Run();
        }
    }
}
