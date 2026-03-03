using LoyalCode.Dmitriev.Ivan.UserService.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LoyalCode.Dmitriev.Ivan.ApiGateway.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserServiceController : BaseController
    {
        private readonly HttpClient _httpClient;

        public UserServiceController(IHttpClientFactory httpClientFactory, ILogger<UserServiceController> logger) : base(httpClientFactory.CreateClient(nameof(UserServiceController)), logger)
        {
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] UserDto request, CancellationToken cancellationToken)
        {
            var res = await _httpClient.PostAsJsonAsync("api/UserApi/register", request, cancellationToken);
            
            var content = await res.Content.ReadAsStringAsync();

            return StatusCode(sta);
        }

        [HttpPost("login")]
        public Task<IActionResult> Login([FromBody] UserDto request)
        {
            return ProxyRequest("api/UserApi/login");
        }

        [HttpPost("logout")]
        public Task<IActionResult> Logout()
        {
            return ProxyRequest("api/UserApi/logout");
        }
    }
}
