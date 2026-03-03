using LoyalCode.Dmitriev.Ivan.Application.User.LogoutUser;
using LoyalCode.Dmitriev.Ivan.Infrustructure.Jwt;
using Moq;


namespace LoyalCode.Dmitriev.Ivan.UnitTests.User
{
    [TestFixture]
    public class LogoutUserCommandHandlerTests
    {
        private Mock<ITokenService> _tokenServiceMock;
        private LogoutUserCommandHandler _handler;

        [SetUp]
        public void Setup()
        {
            _tokenServiceMock = new Mock<ITokenService>();
            _handler = new LogoutUserCommandHandler(_tokenServiceMock.Object);
        }

        [Test]
        public async Task Handle_ValidToken_AddsToBlacklist()
        {
            // Arrange
            var token = "valid.jwt.token";
            var tokenId = "test-token-id";

            var command = new LogoutUserCommand
            {
                Token = token
            };

            _tokenServiceMock
                .Setup(x => x.AddToBlacklist(It.IsAny<string>()))
                .Returns(Task.CompletedTask);

            // Act
            await _handler.Handle(command, CancellationToken.None);

            // Assert
            _tokenServiceMock.Verify(x => x.AddToBlacklist(It.IsAny<string>()), Times.Once);
        }

        [Test]
        public async Task Handle_EmptyToken_DoesNotAddToBlacklist()
        {
            // Arrange
            var command = new LogoutUserCommand
            {
                Token = string.Empty
            };

            // Act
            await _handler.Handle(command, CancellationToken.None);

            // Assert
            _tokenServiceMock.Verify(x => x.AddToBlacklist(It.IsAny<string>()), Times.Never);
        }
    }
}