using LoyalCode.Dmitriev.Ivan.Domain;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoyalCode.Dmitriev.Ivan.Infrustructure.Repositories
{
    public interface IUserFavoriteCurrencyRepository
    {
        Task<List<Currency>> GetFavoriteCurrenciesByUserId(int userId, CancellationToken cancellationToken = default);
    }

    internal class UserFavoriteCurrencyRepository : IUserFavoriteCurrencyRepository
    {
        private readonly AppDbContext _appDbContext;

        public UserFavoriteCurrencyRepository(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }

        public async Task<List<Currency>> GetFavoriteCurrenciesByUserId(int userId, CancellationToken cancellationToken = default)
        {
            return await _appDbContext.Favorites
                .Include(x => x.Currency)
                .Where(x => x.UserId == userId)
                .Select(x => x.Currency)
                .ToListAsync(cancellationToken);
        }
    }
}
