using System.Text.Json;
using AWE.VideoLister.BusinessLogic.DTO;
using AWE.VideoLister.BusinessLogic.Providers;
using Microsoft.Net.Http.Headers;

namespace AWE.VideoLister.BusinessLogic.Clients
{
    /// <inheritdoc />
    internal class AWEClient : IAWEClient
    {
        private readonly string baseAddress;
        private readonly HttpClient httpClient;
        private readonly ILoggingProvider loggingProvider;

        public AWEClient(IConfigurationProvider configurationProvider, ILoggingProvider loggingProvider)
        {
            baseAddress = configurationProvider.HttpClientBaseAddress;
            httpClient = new HttpClient();
            this.loggingProvider = loggingProvider;
        }

        /// <inheritdoc />
        public async Task<ContentListingDto> GetContentListAsync(string queryString)
        {
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, $"{baseAddress}/api/video-promotion/v1/client/list{queryString}");
            request.Headers.Add("X-Requested-With", "XMLHttpRequest");

            HttpResponseMessage response = default(HttpResponseMessage);
            try
            {
                response = await httpClient.SendAsync(request);
                response.EnsureSuccessStatusCode();
            }
            catch (HttpRequestException ex)
            {
                loggingProvider.LogError(ex.Message);
                return null;
            }

            string responseJson = await response.Content.ReadAsStringAsync();
            JsonSerializerOptions options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            return JsonSerializer.Deserialize<ContentListingDto>(responseJson, options);
        }

        /// <inheritdoc />
        public async Task<FileDataDto> GetFileAsync(string url)
        {
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, url);
            HttpResponseMessage response = default(HttpResponseMessage);

            try
            {
                response = await httpClient.SendAsync(request);
                response.EnsureSuccessStatusCode();
            }
            catch (HttpRequestException ex)
            {
                loggingProvider.LogError($"{ex.Message} - URL: {url}");
                return null;
            }

            return new FileDataDto()
            {
                Data = await response.Content.ReadAsByteArrayAsync(),
                MediaType = response.Content.Headers.GetValues(HeaderNames.ContentType).First()
            };
        }
    }
}
