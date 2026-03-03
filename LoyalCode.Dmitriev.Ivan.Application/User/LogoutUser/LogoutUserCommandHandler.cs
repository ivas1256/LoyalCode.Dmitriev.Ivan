using LoyalCode.Dmitriev.Ivan.Infrustructure.Jwt;
using MediatR;
using System.IdentityModel.Tokens.Jwt;

namespace LoyalCode.Dmitriev.Ivan.Application.User.LogoutUser
{
    public class LogoutUserCommandHandler : IRequestHandler<LogoutUserCommand>
    {
        private readonly ITokenService _tokenService;

        public LogoutUserCommandHandler(ITokenService tokenService)
        {
            _tokenService = tokenService;
        }

        // Как один из вариантов сохраняем токен в мемори кэше 
        public async Task Handle(LogoutUserCommand request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(request.Token))
            {
                return;
            }

            var tokenHandler = new JwtSecurityTokenHandler();
            var jwtToken = tokenHandler.ReadJwtToken(request.Token);

            var tokenId = jwtToken.Id;

            if (!string.IsNullOrEmpty(tokenId))
            {
                await _tokenService.AddToBlacklist(tokenId);
            }
        }
    }
}
