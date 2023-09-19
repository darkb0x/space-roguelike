using UnityEngine;

namespace Game.UI
{
    public interface IWindow
    {
        public WindowID ID { get; }
        public void Initialize(UIWindowService service);
        public void Open(bool notify = true);
        public void Close(bool notify = true);
    }
}
