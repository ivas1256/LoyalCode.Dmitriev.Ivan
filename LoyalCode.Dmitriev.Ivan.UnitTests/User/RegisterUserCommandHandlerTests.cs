using LoyalCode.Dmitriev.Ivan.Application.User.RegisterUser;
using LoyalCode.Dmitriev.Ivan.Infrustructure.Jwt;
using LoyalCode.Dmitriev.Ivan.Infrustructure.Repositories;
using Moq;


namespace LoyalCode.Dmitriev.Ivan.UnitTests.User
{
    [TestFixture]
    public class RegisterUserCommandHandlerTests
    {
        private Mock<IUserRepository> _userRepositoryMock;
        private Mock<ITokenService> _tokenServiceMock;
        private Mock<IPasswordService> _passwordServiceMock;
        private RegisterUserCommandHandler _handler;

        [SetUp]
        public void Setup()
        {
            _userRepositoryMock = new Mock<IUserRepository>();
            _tokenServiceMock = new Mock<ITokenService>();
            _passwordServiceMock = new Mock<IPasswordService>();

            _handler = new RegisterUserCommandHandler(
                _userRepositoryMock.Object,
                _tokenServiceMock.Object,
                _passwordServiceMock.Object);
        }

        [Test]
        public async Task Handle_ValidRegistration_ReturnsResponseWithCreatedId()
        {
            // Arrange
            var command = new RegisterUserCommand
            {
                Name = "testuser",
                Password = "password123"
            };

            var encryptedPassword = "encryptedPassword123";
            var createdUser = new Domain.User
            {
                Id = 1,
                Name = command.Name,
                Password = encryptedPassword
            };

            _userRepositoryMock
                .Setup(x => x.Exists(command.Name, It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);

            _passwordServiceMock
                .Setup(x => x.EncryptPassword(command.Password))
                .Returns(encryptedPassword);

            _userRepositoryMock
                .Setup(x => x.Add(It.Is<Domain.User>(u =>
                    u.Name == command.Name &&
                    u.Password == encryptedPassword),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(createdUser);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.IsNotNull(result);
            Assert.Equals(result.CreatedId, createdUser.Id);

            _userRepositoryMock.Verify(x => x.Exists(command.Name, It.IsAny<CancellationToken>()), Times.Once);
            _passwordServiceMock.Verify(x => x.EncryptPassword(command.Password), Times.Once);
            _userRepositoryMock.Verify(x => x.Add(It.IsAny<Domain.User>(), It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}