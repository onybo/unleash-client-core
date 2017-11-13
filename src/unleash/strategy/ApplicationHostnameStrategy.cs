using System;
using System.Collections.Generic;
using System.Linq;

namespace Olav.Unleash.Strategy
{
    // import java.net.InetAddress;
    // import java.net.UnknownHostException;
    // import java.util.Arrays;
    // import java.util.Map;
    // import java.util.Optional;

    public class ApplicationHostnameStrategy : Strategy 
    {
        public static readonly string HOST_NAMES_PARAM = "hostNames";
        protected readonly string NAME = "applicationHostname";
        private readonly string _hostname;

        public ApplicationHostnameStrategy() 
        {
            _hostname = System.Net.Dns.GetHostName();
        }

        public override string Name => NAME;

        public override bool IsEnabled(Dictionary<string, string> parameters) 
        {
            return parameters.ContainsKey(HOST_NAMES_PARAM) && parameters[HOST_NAMES_PARAM] != null ?
                parameters[HOST_NAMES_PARAM]
                    .ToLower()
                    .Split(',')
                    .Select(n => n.Trim())
                    .Contains(_hostname.ToLower()) :
                false;
        }
    }    
}

