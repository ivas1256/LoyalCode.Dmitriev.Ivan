using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http;

namespace LoyalCode.Dmitriev.Ivan.ApiGateway.Controllers
{
    public class BaseController : ControllerBase
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger _logger;

        public BaseController(HttpClient httpClient, ILogger logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        protected async Task<IActionResult> ProxyRequest(string targetUrl)
        {
            try
            {
                using var requestMessage = new HttpRequestMessage(
                    new HttpMethod(Request.Method),
                    targetUrl);

                foreach (var header in Request.Headers)
                {
                    //requestMessage.Headers.Add(header.Key, header.Value.ToList());
                    requestMessage.Headers.TryAddWithoutValidation(header.Key, header.Value.ToList());
                }

                if (Request.Body != null && Request.Method != HttpMethod.Get.Method)
                {
                    var content = new StreamContent(Request.Body);
                    requestMessage.Content = content;
                }

                var response = await _httpClient.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead);

                var responseMessage = new HttpResponseMessage(response.StatusCode);

                foreach (var header in response.Headers)
                {
                    Response.Headers[header.Key] = header.Value.ToArray();
                }

                foreach (var header in response.Content.Headers)
                {
                    Response.Headers[header.Key] = header.Value.ToArray();
                }

                Response.StatusCode = (int)response.StatusCode;

                if (response.Content != null)
                {
                    var stream = await response.Content.ReadAsStreamAsync();
                    await stream.CopyToAsync(Response.Body);
                }

                return Empty;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error proxying request");
                return StatusCode(StatusCodes.Status500InternalServerError, new { error = "Internal gateway error" });
            }
        }

        HttpRequestMessage CreateProxyHttpRequest(HttpContext context, Uri uri)
        {
            var requestMessage = new HttpRequestMessage();

            requestMessage.RequestUri = uri;
            requestMessage.Method = new HttpMethod(context.Request.Method);

            // копируем body (для POST/PUT)
            if (context.Request.ContentLength > 0)
            {
                requestMessage.Content = new StreamContent(context.Request.Body);
            }

            // копируем заголовки (включая Authorization!)
            foreach (var header in context.Request.Headers)
            {
                if (!requestMessage.Headers.TryAddWithoutValidation(header.Key, header.Value.ToArray()))
                {
                    requestMessage.Content?.Headers.TryAddWithoutValidation(header.Key, header.Value.ToArray());
                }
            }

            return requestMessage;
        }
    }
}
