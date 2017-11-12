using System.Collections.Generic;

namespace Olav.Unleash.Repository
{
    public sealed class FeatureToggleResponse
    {
        internal enum StatusType { NOT_CHANGED, CHANGED }


        private FeatureToggleResponse(StatusType status, ToggleCollection toggleCollection)
        {
            Status = status;
            ToggleCollection = toggleCollection;
        }

        private FeatureToggleResponse(StatusType status)
        {
            Status = status;
            ToggleCollection = ToggleCollection.EmptyCollection;
        }

        private StatusType Status { get; set; }

        public bool IsChanged => Status == StatusType.CHANGED;

        public ToggleCollection ToggleCollection  { get; private set; }

        public static FeatureToggleResponse NotChanged() => new FeatureToggleResponse(StatusType.NOT_CHANGED);
        public static FeatureToggleResponse Changed(ToggleCollection toggleCollection) => new FeatureToggleResponse(StatusType.CHANGED, toggleCollection);
    }
}
