using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LoyalCode.Dmitriev.Ivan.ApiGateway.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CurrencyServiceController : BaseController
    {
        public CurrencyServiceController(HttpClient httpClient, ILogger<CurrencyServiceController> logger) : base(httpClient, logger)
        {
        }

        [HttpGet("my-currencies")]
        public Task<IActionResult> GetMyCurrencies()
        {
            return ProxyRequest("api/CurrencyApi/my-currencies");
        }

    }
}
