using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.RateLimiting;
using System.Threading.Tasks;

namespace LoyalCode.Dmitriev.Ivan.Application.Currency.GetUserCurrencies
{
    public class GetUserCurrenciesQuery : IRequest<List<CurrencyDto>>
    {
        public int UserId { get; set; }
    }
}
