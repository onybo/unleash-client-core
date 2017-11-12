using System.Collections.Generic;
using System.Linq;

namespace Olav.Unleash
{

    // import java.util.HashMap;
    // import java.util.Map;

    public sealed class FakeUnleash : Unleash 
    {
        private bool _enableAll = false;
        private bool _disableAll = false;
        private Dictionary<string, bool> _features = new Dictionary<string, bool>();

        
        public override bool IsEnabled(string toggleName, UnleashContext context) 
        {
            return IsEnabled(toggleName, context, false);
        }

        public override bool IsEnabled(string toggleName, UnleashContext context, bool defaultSetting) 
        {
            if(_enableAll) 
                return true;
            if(_disableAll) 
                return false;
            
            return _features.ContainsKey(toggleName) ? _features[toggleName] : defaultSetting;
        }

        public void EnableAll() 
        {
            _disableAll = false;
            _enableAll = true;
            _features.Clear();
        }

        public void DisableAll() 
        {
            _disableAll = true;
            _enableAll = false;
            _features.Clear();
        }

        public void ResetAll() 
        {
            _disableAll = false;
            _enableAll = false;
            _features.Clear();
        }

        public void Enable(params string[] features) 
        {
            features
                .ToList()
                .ForEach(f => _features.Add(f, true));
        }

        public void Disable(params string[] features) 
        {
            features
                .ToList()
                .ForEach(f => _features.Add(f, false));
        }

        public void Reset(params string[] features) 
        {
            features
                .ToList()
                .ForEach(f => _features.Remove(f));
        }
    }
}