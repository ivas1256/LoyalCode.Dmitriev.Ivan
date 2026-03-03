using LoyalCode.Dmitriev.Ivan.Infrustructure.Repositories;
using MediatR;

namespace LoyalCode.Dmitriev.Ivan.Application.Currency.GetUserCurrencies
{
    public class GetUserCurrenciesQueryHandler : IRequestHandler<GetUserCurrenciesQuery, List<CurrencyDto>>
    {
        private readonly IUserFavoriteCurrencyRepository _favoriteCurrencyRepository;

        public GetUserCurrenciesQueryHandler(IUserFavoriteCurrencyRepository favoriteCurrencyRepository)
        {
            _favoriteCurrencyRepository = favoriteCurrencyRepository;
        }

        public async Task<List<CurrencyDto>> Handle(GetUserCurrenciesQuery request, CancellationToken cancellationToken)
        {
            var favoriteCurrencies = await _favoriteCurrencyRepository.GetFavoriteCurrenciesByUserId(request.UserId);

            return favoriteCurrencies.Select(x => new CurrencyDto
            {
                Id = x.Id,
                Name = x.Name,
                Rate = x.Rate
            }).ToList();
        }
    }
}
