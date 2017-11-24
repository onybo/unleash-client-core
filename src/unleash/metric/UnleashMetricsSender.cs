using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Olav.Unleash.Logging;
using Olav.Unleash.Util;

namespace Olav.Unleash.Metric
{
    internal class UnleashMetricsSender
    {
        private const int CONNECT_TIMEOUT = 1000;

        private UnleashConfig _unleashConfig;
        private readonly Uri _baseURL;

        private static readonly HttpClient HttpClient;

        private static readonly ILog Logger = LogProvider.For<UnleashMetricsSender>();

        static UnleashMetricsSender()
        {
            var handler = new HttpClientHandler();
            
            HttpClient = new HttpClient(handler);
            HttpClient.DefaultRequestHeaders.Clear();
            HttpClient.DefaultRequestHeaders.Accept.Add(
                new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
        }
        
        internal UnleashMetricsSender() : this(
            new UnleashConfig(
                new Uri("http://localhost"),
                new Dictionary<string, string>(),
                "",
                "",
                "",
                "",
                0,
                10,
                true))
        {            
        }

        internal UnleashMetricsSender(Util.UnleashConfig unleashConfig)
        {
            _unleashConfig = unleashConfig;
            var urls = unleashConfig.UnleashURLs;
            _baseURL = urls.BaseURL;
            HttpClient.BaseAddress = _baseURL;
        }

        // static class DateTimeSerializer implements JsonSerializer<LocalDateTime> {
        //     @Override
        //     public JsonElement serialize(
        //             LocalDateTime localDateTime, Type type, JsonSerializationContext jsonSerializationContext) {
        //         return new JsonPrimitive(ISO_INSTANT.format(localDateTime.toInstant(ZoneOffset.UTC)));
        //     };
        // }

        internal virtual async Task RegisterClient(ClientRegistration registration)
        {
            if (!_unleashConfig.IsDisableMetrics)
            {
                await Post(UnleashURLs.ClientRegisterURL, registration, "register client");
            }
        }

        internal virtual async Task SendMetrics(ClientMetrics metrics)
        {
            if (!_unleashConfig.IsDisableMetrics)
            {
                 await Post(UnleashURLs.ClientMetricsURL, metrics, "send metrics");
            }
        }

        private async Task<HttpStatusCode> Post(string url, Object o, string operation)
        {
            

            var request = new HttpRequestMessage(HttpMethod.Post, UnleashURLs.FetchTogglesUri);
            UnleashConfig.SetRequestProperties(request.Headers, _unleashConfig);
            request.Headers.Add("Cache-Control", "no-cache"); //  CacheControl.NoCache = true;
            request.Content = new StringContent(JsonConvert.SerializeObject(o), Encoding.UTF8, "application/json");

            var result = await HttpClient.SendAsync(request);

            if (!result.IsSuccessStatusCode)
                Logger.WarnFormat("failed to {Operation}: statuscode {StatusCode}", operation, result.StatusCode);
            return result.StatusCode;
        }
    }
}