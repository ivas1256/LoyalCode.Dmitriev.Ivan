using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Text;
using System.Text.Json;

namespace LoyalCode.Dmitriev.Ivan.ApiGateway.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserApiController : ControllerBase
    {
        private readonly HttpClient _httpClient;
        public UserApiController(ILogger<UserApiController> logger, IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient(nameof(UserApiController));
        }

        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<ActionResult<RegisterUserResponseDto>> Register(UserDto registerDto)
        {
            try
            {
                var jsonContent = JsonSerializer.Serialize(registerDto);
                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync("api/UserApi/register", content);

                if (response.IsSuccessStatusCode)
                {
                    var respJson = await response.Content.ReadAsStringAsync();
                    var result = JsonSerializer.Deserialize<RegisterUserResponseDto>(respJson, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });
                    return Ok(result);
                }

                var err = await response.Content.ReadAsStringAsync();
                return StatusCode((int)response.StatusCode, err);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<ActionResult<LoginResponseDto>> Login(UserDto loginDto)
        {
            try
            {
                var jsonContent = JsonSerializer.Serialize(loginDto);
                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync("api/UserApi/login", content);

                if (response.IsSuccessStatusCode)
                {
                    var responseJson = await response.Content.ReadAsStringAsync();
                    var result = JsonSerializer.Deserialize<LoginResponseDto>(responseJson, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });
                    return Ok(result);
                }

                var errorContent = await response.Content.ReadAsStringAsync();
                return StatusCode((int)response.StatusCode, errorContent);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpGet("logout")]
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            try
            {
                var authorizationHeader = Request.Headers["Authorization"].ToString();
                if (!string.IsNullOrEmpty(authorizationHeader))
                {
                    _httpClient.DefaultRequestHeaders.Add("Authorization", authorizationHeader);
                }

                var response = await _httpClient.GetAsync("/api/UserApi/logout");

                if (response.IsSuccessStatusCode)
                {
                    return Ok();
                }

                var errorContent = await response.Content.ReadAsStringAsync();
                return StatusCode((int)response.StatusCode, errorContent);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error");
            }
        }
    }

    public class LoginResponseDto
    {
        public string Token { get; set; }
        public DateTime ExpiresAt { get; set; }
    }

    public class RegisterUserResponseDto
    {
        public int CreatedId { get; set; }
    }

    public class UserDto
    {
        public string Name { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}
