using System.Collections.Generic;

namespace Olav.Unleash
{
    public class UnleashContext 
    {        
        public UnleashContext(string userId, string sessionId, string remoteAddress, Dictionary<string,string> properties) 
        {
            UserId = userId;
            SessionId = sessionId;
            RemoteAddress = remoteAddress;
            Properties = properties;
        }

        public string UserId { get; private set; } 

        public string SessionId { get; private set; } 

        public string RemoteAddress { get; private set; } 

        public Dictionary<string,string> Properties { get; private set; } 

        public static Builder CreateBuilder() 
        {
            return new Builder();
        }

        public class Builder 
        {
            private string _userId;
            private string _sessionId;
            private string _remoteAddress;

            private readonly Dictionary<string, string> _properties = new Dictionary<string, string>();

            public Builder UserId(string userId) 
            {
                _userId = userId;
                return this;
            }

            public Builder SessionId(string sessionId) 
            {
                _sessionId = sessionId;
                return this;
            }

            public Builder RemoteAddress(string remoteAddress) 
            {
                _remoteAddress = remoteAddress;
                return this;
            }

            public Builder AddProperty(string name, string value) 
            {
                _properties.Add(name, value);
                return this;
            }

            public UnleashContext Build() 
            {
                return new UnleashContext(_userId, _sessionId, _remoteAddress, _properties);
            }
        }
    }
}