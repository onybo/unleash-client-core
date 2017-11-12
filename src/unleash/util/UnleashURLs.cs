
using System;

namespace Olav.Unleash.Util
{
    public class UnleashURLs
    {
        public const string FetchTogglesUri = "/features";
        public const string ClientMetricsURL = "/client/metrics";
        public const string ClientRegisterURL = "/client/register";
        public UnleashURLs(Uri unleashAPI)
        {
            BaseURL = unleashAPI;
            // try
            // {
            // }
            // catch (UriFormatException)
            // {
            //     throw new ArgumentException("Unleash API is not a valid URL: " + unleashAPI);
            // }
        }

        public Uri BaseURL { get; private set; }
    }
}