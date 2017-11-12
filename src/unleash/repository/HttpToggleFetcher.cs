using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Olav.Unleash.Util;

namespace Olav.Unleash.Repository
{
    public sealed class HttpToggleFetcher : IToggleFetcher
    {
        public const int CONNECT_TIMEOUT = 10000;
        private string _etag = "";

        private UnleashConfig _unleashConfig;

        private static readonly HttpClient HttpClient;

        static HttpToggleFetcher()
        {
            var handler = new HttpClientHandler();

            HttpClient = new HttpClient(handler);
            HttpClient.DefaultRequestHeaders.Clear();
            HttpClient.DefaultRequestHeaders.Accept.Add(
                new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
        }

        public HttpToggleFetcher(UnleashConfig unleashConfig)
        {
            _unleashConfig = unleashConfig;
            HttpClient.BaseAddress = unleashConfig.UnleashURLs.BaseURL;
        }

        public async Task<FeatureToggleResponse> FetchToggles()
        {
            
            var request = new HttpRequestMessage(HttpMethod.Get, UnleashURLs.FetchTogglesUri);
            if (!string.IsNullOrEmpty(_etag))
                request.Headers.IfNoneMatch.Add(new EntityTagHeaderValue(_etag));
            //useCache(true)
            UnleashConfig.SetRequestProperties(request.Headers, _unleashConfig);
            
            var result = await HttpClient.SendAsync(request);
            return await (result.IsSuccessStatusCode ?
                    GetToggleResponse(result) :
                    Task.FromResult(FeatureToggleResponse.NotChanged()));
        }

        private async Task<FeatureToggleResponse> GetToggleResponse(HttpResponseMessage response)
        {
            _etag = response.Headers.ETag?.Tag;
            var content = await response.Content.ReadAsStringAsync();
            var toggles = JsonToggleParser.FromJson(content);
            return FeatureToggleResponse.Changed(toggles);
        }
    }
}
