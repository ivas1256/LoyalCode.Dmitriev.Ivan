using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoyalCode.Dmitriev.Ivan.Application.User.LogoutUser
{
    public class LogoutUserCommand : IRequest
    {
        public string Token { get; set; }
    }
}
