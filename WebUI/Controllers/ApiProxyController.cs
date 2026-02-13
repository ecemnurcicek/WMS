using Microsoft.AspNetCore.Mvc;
using System.Text;

namespace WebUI.Controllers
{
    /// <summary>
    /// WebUI'den gelen /api/* isteklerini WebAPI'ye proxy'ler.
    /// JS tarafından doğrudan /api/transfer gibi çağrılar yapılabilmesini sağlar.
    /// </summary>
    public class ApiProxyController : Controller
    {
        private readonly HttpClient _httpClient;

        public ApiProxyController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("WebAPI");
        }

        [Route("api/{**path}")]
        [HttpGet]
        public async Task<IActionResult> ProxyGet(string path)
        {
            var url = BuildUrl(path);
            var response = await _httpClient.GetAsync(url);
            return await BuildResult(response);
        }

        [Route("api/{**path}")]
        [HttpPost]
        public async Task<IActionResult> ProxyPost(string path)
        {
            var url = BuildUrl(path);
            var body = await ReadBodyAsync();
            var content = new StringContent(body, Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync(url, content);
            return await BuildResult(response);
        }

        [Route("api/{**path}")]
        [HttpPut]
        public async Task<IActionResult> ProxyPut(string path)
        {
            var url = BuildUrl(path);
            var body = await ReadBodyAsync();
            var content = new StringContent(body, Encoding.UTF8, "application/json");
            var response = await _httpClient.PutAsync(url, content);
            return await BuildResult(response);
        }

        [Route("api/{**path}")]
        [HttpDelete]
        public async Task<IActionResult> ProxyDelete(string path)
        {
            var url = BuildUrl(path);
            var response = await _httpClient.DeleteAsync(url);
            return await BuildResult(response);
        }

        private string BuildUrl(string path)
        {
            var queryString = HttpContext.Request.QueryString.Value ?? "";
            return $"/api/{path}{queryString}";
        }

        private async Task<string> ReadBodyAsync()
        {
            Request.Body.Position = 0;
            using var reader = new StreamReader(Request.Body, Encoding.UTF8, leaveOpen: true);
            var body = await reader.ReadToEndAsync();
            return body;
        }

        private async Task<IActionResult> BuildResult(HttpResponseMessage response)
        {
            var responseBody = await response.Content.ReadAsStringAsync();
            var statusCode = (int)response.StatusCode;

            if (string.IsNullOrEmpty(responseBody))
            {
                return StatusCode(statusCode);
            }

            return new ContentResult
            {
                StatusCode = statusCode,
                Content = responseBody,
                ContentType = "application/json; charset=utf-8"
            };
        }
    }
}
