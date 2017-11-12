namespace Olav.Unleash.Metric
{
    public class ToggleCount 
    {
        public void Register(bool active) 
        {
            if(active) 
                Yes++;
            else
                No++;
        }

        public long Yes { get; private set; }
        public long No { get; private set; }
    }
}