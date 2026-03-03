using LoyalCode.Dmitriev.Ivan.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoyalCode.Dmitriev.Ivan.Infrustructure.Repositories
{
    public interface ICurrencyRepository
    {
        Task InsertOrUpdate(List<Currency> currencies, CancellationToken cancellationToken = default);
    }

    internal class CurrencyRepository : ICurrencyRepository
    {
        private readonly AppDbContext _context;
        private readonly ILogger<CurrencyRepository> _logger;

        public CurrencyRepository(AppDbContext context, ILogger<CurrencyRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task InsertOrUpdate(List<Currency> currencies, CancellationToken cancellationToken = default)
        {
            var currencyNames = currencies.Select(c => c.Name).ToList();
            var existingCurrencies = await _context.Currencies
                .Where(c => currencyNames.Contains(c.Name))
                .ToDictionaryAsync(c => c.Name, cancellationToken);

            foreach (var currency in currencies)
            {
                if (existingCurrencies.TryGetValue(currency.Name, out var existingCurrency))
                {
                    existingCurrency.Rate = currency.Rate;
                }
                else
                {
                    await _context.Currencies.AddAsync(currency, cancellationToken);
                }
            }

            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
