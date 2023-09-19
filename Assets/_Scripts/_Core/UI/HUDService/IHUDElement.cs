namespace Game.UI.HUD
{
    public interface IHUDElement
    {
        public HUDElementID ID { get; }

        public void Initialize();
        public void Show();
        public void Hide();
    }
}
