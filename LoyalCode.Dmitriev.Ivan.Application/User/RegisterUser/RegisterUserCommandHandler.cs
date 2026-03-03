using LoyalCode.Dmitriev.Ivan.Infrustructure.Jwt;
using LoyalCode.Dmitriev.Ivan.Infrustructure.Repositories;
using MediatR;

namespace LoyalCode.Dmitriev.Ivan.Application.User.RegisterUser
{
    public class RegisterUserCommandHandler : IRequestHandler<RegisterUserCommand, RegisterUserResponseDto>
    {
        private readonly IUserRepository _userRepository;
        private readonly ITokenService _tokenService;
        private readonly IPasswordService _passwordService;

        public RegisterUserCommandHandler(
            IUserRepository userRepository,
            ITokenService tokenService,
            IPasswordService passwordService)
        {
            _userRepository = userRepository;
            _tokenService = tokenService;
            _passwordService = passwordService;
        }

        public async Task<RegisterUserResponseDto> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
        {
            var exists = await _userRepository.Exists(request.Name, cancellationToken);
            if (exists)
            {
                throw new Exception("Пользователь с таким именем уже существует");
            }

            var user = new Domain.User
            {
                Name = request.Name,
                Password = _passwordService.EncryptPassword(request.Password)
            };

            var createdUser = await _userRepository.Add(user, cancellationToken);
            var token = _tokenService.GenerateToken(createdUser);

            return new RegisterUserResponseDto
            {
                CreatedId = createdUser.Id,
            };
        }
    }
}
