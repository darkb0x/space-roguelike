using System.Collections.Generic;
using UnityEngine;

namespace Game.UI.HUD
{
    public class HUDContainer : MonoBehaviour
    {
        [SerializeField] private List<HUDElement> HUDElements = new List<HUDElement>();

        private List<HUDElement> _initializedHudElements;

        public void Initialize(HUDConfig config)
        {
            _initializedHudElements = new List<HUDElement>();

            foreach (var hudElement in HUDElements)
            {
                if(!config.GetHudElementEnabled(hudElement.ID))
                {
                    hudElement.Disable();
                    continue;
                }

                hudElement.Enable();
                hudElement.Initialize();
                _initializedHudElements.Add(hudElement);
            }
        }

        public HUDElement[] GetHudElements()
            => _initializedHudElements.ToArray();
    }
}