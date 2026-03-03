using LoyalCode.Dmitriev.Ivan.Application.Currency.GetUserCurrencies;
using LoyalCode.Dmitriev.Ivan.Infrustructure.Jwt;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LoyalCode.Dmitriev.Ivan.CurrencyService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CurrencyApiController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ITokenService _tokenService;
        private readonly ILogger<CurrencyApiController> _logger;

        public CurrencyApiController(
            IMediator mediator,
            ITokenService tokenService,
            ILogger<CurrencyApiController> logger)
        {
            _mediator = mediator;
            _tokenService = tokenService;
            _logger = logger;
        }

        [HttpGet("my-currencies")]
        public async Task<ActionResult<List<CurrencyDto>>> GetMyCurrencies()
        {
            try
            {
                var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

                var userId = _tokenService.GetUserIdFromToken(token);
                if (!userId.HasValue)
                {
                    return Unauthorized();
                }

                var query = new GetUserCurrenciesQuery { UserId = userId.Value };
                var result = await _mediator.Send(query);

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting currencies");
                return BadRequest(new { error = ex.Message });
            }
        }
    }
}
