using LoyalCode.Dmitriev.Ivan.Application.User.LoginUser;
using LoyalCode.Dmitriev.Ivan.Infrustructure.Jwt;
using LoyalCode.Dmitriev.Ivan.Infrustructure.Repositories;
using Moq;
using NUnit.Framework;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;


namespace LoyalCode.Dmitriev.Ivan.UnitTests.User
{

    [TestFixture]
    public class LoginUserCommandHandlerTests
    {
        private Mock<IUserRepository> _userRepositoryMock;
        private Mock<ITokenService> _tokenServiceMock;
        private Mock<IPasswordService> _passwordServiceMock;
        private LoginUserCommandHandler _handler;

        [SetUp]
        public void Setup()
        {
            _userRepositoryMock = new Mock<IUserRepository>();
            _tokenServiceMock = new Mock<ITokenService>();
            _passwordServiceMock = new Mock<IPasswordService>();

            _handler = new LoginUserCommandHandler(
                _userRepositoryMock.Object,
                _tokenServiceMock.Object,
                _passwordServiceMock.Object);
        }

        [Test]
        public async Task Handle_ValidCredentials_ReturnsTokenResponse()
        {
            // Arrange
            var command = new LoginUserCommand
            {
                Name = "testuser",
                Password = "password123"
            };

            var user = new Domain.User
            {
                Id = 1,
                Name = command.Name,
                Password = "encryptedPassword123"
            };

            var expectedToken = "jwt.token";
            var expectedExpiresAt = DateTime.UtcNow.AddHours(24);

            _userRepositoryMock
                .Setup(x => x.GetByName(command.Name, It.IsAny<CancellationToken>()))
                .ReturnsAsync(user);

            _passwordServiceMock
                .Setup(x => x.VerifyPassword(command.Password, user.Password))
                .Returns(true);

            _tokenServiceMock
                .Setup(x => x.GenerateToken(user))
                .Returns(expectedToken);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.IsNotNull(result);
            Assert.Equals(result.Token, expectedToken);
            Assert.That(result.ExpiresAt, Is.EqualTo(expectedExpiresAt).Within(TimeSpan.FromSeconds(1)));

            _userRepositoryMock.Verify(x => x.GetByName(command.Name, It.IsAny<CancellationToken>()), Times.Once);
            _passwordServiceMock.Verify(x => x.VerifyPassword(command.Password, user.Password), Times.Once);
            _tokenServiceMock.Verify(x => x.GenerateToken(user), Times.Once);
        }
    }
}