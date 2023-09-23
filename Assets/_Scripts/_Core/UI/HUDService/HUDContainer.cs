using System.Collections.Generic;
using UnityEngine;

namespace Game.UI.HUD
{
    public class HUDContainer : HUDElement
    {
        [SerializeField] private List<HUDElement> HUDElements = new List<HUDElement>();

        public override HUDElementID ID => HUDElementID.Container;

        private List<HUDElement> _initializedHudElements;

        public void Initialize(HUDConfig config, HUDService service, UIWindowService windowService)
        {
            _initializedHudElements = new List<HUDElement>();

            foreach (var hudElement in HUDElements)
            {
                if(!config.GetHudElementEnabled(hudElement.ID))
                {
                    hudElement.Disable();
                    continue;
                }

                hudElement.Enable(service, windowService, this);
                _initializedHudElements.Add(hudElement);
            }
            foreach (var element in _initializedHudElements)
            {
                element.Initialize();

                if(element is HUDContainer container)
                {
                    container.Initialize(config, service, windowService);
                }
            }
        }

        public HUDElement[] GetHudElements()
            => _initializedHudElements.ToArray();
    }
}