using LoyalCode.Dmitriev.Ivan.Infrustructure.Jwt;
using LoyalCode.Dmitriev.Ivan.Infrustructure.Repositories;
using MediatR;

namespace LoyalCode.Dmitriev.Ivan.Application.User.LoginUser
{
    public class LoginUserCommandHandler : IRequestHandler<LoginUserCommand, LoginResponseDto>
    {
        private readonly IUserRepository _userRepository;
        private readonly ITokenService _tokenService;
        private readonly IPasswordService _passwordService;

        public LoginUserCommandHandler(
            IUserRepository userRepository,
            ITokenService tokenService,
            IPasswordService passwordService)
        {
            _userRepository = userRepository;
            _tokenService = tokenService;
            _passwordService = passwordService;
        }

        public async Task<LoginResponseDto> Handle(LoginUserCommand request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetByName(request.Name, cancellationToken);
            if (user == null)
            {
                throw new Exception("Неверное имя пользователя или пароль");
            }

            var isValid = _passwordService.VerifyPassword(request.Password, user.Password);
            if (!isValid)
            {
                throw new Exception("Неверное имя пользователя или пароль");
            }

            var token = _tokenService.GenerateToken(user);

            return new LoginResponseDto
            {
                Token = token,
                ExpiresAt = DateTime.UtcNow.AddHours(24)
            };
        }
    }
}
