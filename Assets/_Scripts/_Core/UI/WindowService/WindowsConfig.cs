using AYellowpaper.SerializedCollections;
using UnityEngine;

namespace Game.UI
{
    [CreateAssetMenu(fileName = "Windows Config", menuName = "Game/Configs/new Windows config")]
    public class WindowsConfig : ScriptableObject
    {
        [SerializeField] private SerializedDictionary<WindowID, Window> Windows =
            new SerializedDictionary<WindowID, Window>() { { WindowID.Unknown, null } };

        public Window GetWindow(WindowID id)
        {
            if(Windows.TryGetValue(id, out var window))
                return window;

            return Windows[WindowID.Unknown];
        }
    }
}