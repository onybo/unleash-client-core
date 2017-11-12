using System.Collections.Generic;

namespace Olav.Unleash
{
    public class ActivationStrategy 
    {
        public ActivationStrategy(string name, Dictionary<string, string> parameters) 
        {
            Name = name;
            Parameters = parameters;
        }

        public string Name { get; private set; }
        public Dictionary<string, string> Parameters { get; private set; }
    }    
}
