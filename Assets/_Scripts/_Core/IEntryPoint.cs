namespace Game
{
    public interface IEntryPoint
    {
        public void InitializeComponents();
        public void RegisterServices();
        public void UnregisterServices();
    }
}
