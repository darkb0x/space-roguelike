using UnityEngine;
using AYellowpaper.SerializedCollections;

namespace Game.UI.HUD
{
    [CreateAssetMenu(fileName = "HUD Config", menuName = "Game/Configs/new HUD Config")]
    public class HUDConfig : ScriptableObject
    {
        [SerializeField] private SerializedDictionary<HUDElementID, bool> HUDElementEnabled;

        public bool GetHudElementEnabled(HUDElementID id)
        {
            if (HUDElementEnabled.TryGetValue(id, out bool enabled))
                return enabled;

            return true;
        }
    }
}
