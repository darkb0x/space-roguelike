using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Game.UI
{
    [CreateAssetMenu(fileName = "Windows Config", menuName = "Game/Configs/new Windows config")]
    public class WindowsConfig : ScriptableObject
    {
        [System.Serializable]
        private struct WindowPair
        {
            public WindowID ID;
            public Window Window;
        }

        [SerializeField] private Window UnknownWindow;
        [SerializeField] private List<WindowPair> Windows = new List<WindowPair>();

        public Window GetWindow(WindowID id)
        {
            foreach (var window in Windows)
            {
                if (window.ID == id)
                    return window.Window;
            }

            return UnknownWindow;
        }
    }
}