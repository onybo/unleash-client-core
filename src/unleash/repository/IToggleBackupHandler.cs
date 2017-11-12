namespace Olav.Unleash.Repository
{
    public interface IToggleBackupHandler 
    {
        ToggleCollection Read();

        void Write(ToggleCollection toggleCollection);
    }
}
