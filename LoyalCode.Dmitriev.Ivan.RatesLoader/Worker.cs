using LoyalCode.Dmitriev.Ivan.Domain;
using LoyalCode.Dmitriev.Ivan.Infrustructure;
using LoyalCode.Dmitriev.Ivan.Infrustructure.Repositories;
using System;
using System.Net.Http;
using System.Text;
using System.Xml.Linq;

namespace LoyalCode.Dmitriev.Ivan.RatesLoader
{
    public class Worker : BackgroundService
    {
        private readonly IServiceProvider _provider;
        private readonly HttpClient _httpClient = new();

        public Worker(IServiceProvider provider)
        {
            _provider = provider;
        }

        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                await UpdateRates();
                await Task.Delay(TimeSpan.FromHours(1), cancellationToken);
            }
        }

        private async Task UpdateRates()
        {
            using var scope = _provider.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            var resp = await _httpClient.GetAsync("http://www.cbr.ru/scripts/XML_daily.asp");
            var bytes = await resp.Content.ReadAsByteArrayAsync();
            var encoding = Encoding.GetEncoding("Windows-1251");
            var responseString = encoding.GetString(bytes, 0, bytes.Length);

            var doc = XDocument.Parse(responseString);

            var currencies = doc.Descendants("Valute")
                .Select(x => new Currency
                {
                    Name = x.Element("CharCode")!.Value,
                    Rate = decimal.Parse(x.Element("Value")!.Value)
                }).ToList();

            var repository = scope.ServiceProvider.GetRequiredService<ICurrencyRepository>();

            await repository.InsertOrUpdate(currencies.ToList());
        }
    }
}
