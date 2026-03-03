using LoyalCode.Dmitriev.Ivan.Application.User.LoginUser;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoyalCode.Dmitriev.Ivan.Application.User.RegisterUser
{
    public class RegisterUserCommand : IRequest<RegisterUserResponseDto>
    {
        public string Name { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}
