namespace Olav.Unleash
{
    public abstract class Unleash 
    {
        // public abstract bool IsEnabled(string toggleName);

//        public abstract bool IsEnabled(string toggleName, bool defaultSetting);

        public virtual bool IsEnabled(string toggleName, UnleashContext context) 
        {
            return IsEnabled(toggleName, context, false);
        }

        public abstract bool IsEnabled(string toggleName, UnleashContext context, bool defaultSetting); 
        // {
        //     return IsEnabled(toggleName, defaultSetting);
        // }
    }
}
