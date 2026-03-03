using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using LoyalCode.Dmitriev.Ivan.Application.Currency.GetUserCurrencies;
using LoyalCode.Dmitriev.Ivan.Domain;
using LoyalCode.Dmitriev.Ivan.Infrustructure.Repositories;

namespace LoyalCode.Dmitriev.Ivan.UnitTests.Application.Currency
{
    [TestFixture]
    public class GetUserCurrenciesQueryHandlerTests
    {
        private Mock<IUserFavoriteCurrencyRepository> _favoriteCurrencyRepositoryMock;
        private GetUserCurrenciesQueryHandler _handler;

        [SetUp]
        public void Setup()
        {
            _favoriteCurrencyRepositoryMock = new Mock<IUserFavoriteCurrencyRepository>();
            _handler = new GetUserCurrenciesQueryHandler(_favoriteCurrencyRepositoryMock.Object);
        }

        [Test]
        public async Task Handle_ExistingUserWithFavorites_ReturnsListOfCurrencyDtos()
        {
            // Arrange
            var query = new GetUserCurrenciesQuery
            {
                UserId = 1
            };

            var favoriteCurrencies = new List<Domain.Currency>
            {
                new Domain.Currency
                {
                    Id = 1,
                    Name = "USD",
                    Rate = 75.50m
                },
                new Domain.Currency
                {
                    Id = 2,
                    Name = "EUR",
                    Rate = 85.30m
                }
            };

            _favoriteCurrencyRepositoryMock
                .Setup(x => x.GetFavoriteCurrenciesByUserId(query.UserId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(favoriteCurrencies);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equals(result.Count,2);

            Assert.Equals(result[0].Id, 1);
            Assert.Equals(result[0].Name, "USD");
            Assert.Equals(result[0].Rate, 75.50m);

            Assert.Equals(result[1].Id, 2);
            Assert.Equals(result[1].Name, "EUR");
            Assert.Equals(result[1].Rate, 85.30m);

            _favoriteCurrencyRepositoryMock.Verify(
                x => x.GetFavoriteCurrenciesByUserId(query.UserId, It.IsAny<CancellationToken>()),
                Times.Once);
        }
    }
}