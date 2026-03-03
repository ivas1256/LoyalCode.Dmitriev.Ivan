using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Text.Json;

namespace LoyalCode.Dmitriev.Ivan.ApiGateway.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CurrencyApiController : ControllerBase
    {
        private readonly HttpClient _httpClient;
        public CurrencyApiController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient(nameof(CurrencyApiController));
        }

        [HttpGet("my-currencies")]
        public async Task<ActionResult<List<CurrencyDto>>> GetMyCurrencies()
        {
            try
            {

                // Проксируем токен авторизации из текущего запроса
                var authorizationHeader = Request.Headers["Authorization"].ToString();
                if (!string.IsNullOrEmpty(authorizationHeader))
                {
                    _httpClient.DefaultRequestHeaders.Add("Authorization", authorizationHeader);
                }
                else
                {
                    return Unauthorized();
                }

                var response = await _httpClient.GetAsync("api/CurrencyApi/my-currencies");

                if (response.IsSuccessStatusCode)
                {
                    var responseJson = await response.Content.ReadAsStringAsync();
                    var result = JsonSerializer.Deserialize<List<CurrencyDto>>(responseJson, new JsonSerializerOptions
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
    }

    public class CurrencyDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal Rate { get; set; }
    }
}
